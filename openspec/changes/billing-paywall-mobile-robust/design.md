## Context

O app .NET MAUI consome a API OSService que já implementa enforcement server-side via `LicenseGuardMiddleware`: qualquer request a endpoint protegido (não-`/api/auth`, não-`/api/tenants`, não-`/api/billing`, não-`/swagger`, não-`/health`) com subscription expirada recebe `403` + `{ error: "SUBSCRIPTION_EXPIRED", validUntil }` ou `{ error: "SUBSCRIPTION_NOT_FOUND" }`. O webhook do Mercado Pago atualiza `tenant.ValidUntil` síncrono — a próxima request vê o estado novo.

O mobile hoje ignora 403 desse middleware e faz checagens proativas via `ISubscriptionGuard.IsExpiredAsync` em 5 lugares (`LoginViewModel`, `FlyoutNavigationService` x2, `DashboardViewModel`, `ClientViewModel`, `WorkorderViewModel`). A confirmação de pagamento na WebView é por substring na URL (`url.Contains("/success")`), sem validação no servidor.

Constraints:
- Token JWT é único (sem refresh token); 401 = relogar
- `HttpClient.DefaultRequestHeaders.Authorization` é mutado por chamada (race condition latente — fora deste escopo, mas o handler ajuda)
- MAUI 10 com lifecycle de janela disponível (`Window.Resumed` / `Window.Activated`)
- Cache de billing usa `Preferences` (compartilhado entre features)

## Goals / Non-Goals

**Goals:**
- Servidor passa a ser fonte da verdade para estado de billing; cliente reage
- Pagamento confirmado é detectado em três caminhos independentes (qualquer um basta): redirect WebView, polling de invoice, próxima request HTTP qualquer
- Reduzir o "blast radius" de regressão — uma única peça (`BillingDelegatingHandler`) decide bloqueio, em vez de 5 lugares
- Eliminar caminhos onde usuário fica preso (cache de 24h após pagamento ou WebView sem redirect)
- Lifecycle: app voltando do background revalida automaticamente

**Non-Goals:**
- Refresh token / auto-relogin em 401 (escopo separado)
- Push notification de pagamento confirmado (escopo separado, F3 do plano)
- Tela de histórico de invoices
- Cache distribuído / sincronização entre instâncias do app
- Mudanças no contrato HTTP da API (nenhuma)

## Decisions

### D1. Bloqueio reativo (handler) vs proativo (guard espalhado)

**Decisão:** `BillingDelegatingHandler` único intercepta 403 do servidor.

**Alternativas:**
- (a) Manter `ISubscriptionGuard` em todos os ViewModels — frágil (esquece-se de adicionar em features novas), causa flash visual (renderiza tela e depois deslogga), e ainda assim não cobre o caso "cache diz ok mas servidor diz não".
- (b) Híbrido — guard só no `LoginViewModel` (pós-login UX) e no `FlyoutNavigationService` (não navegar pra tela que vai dar 403); resto via handler. **Escolhido.**

**Por quê:** servidor já é a fonte da verdade. Handler garante que QUALQUER request bloqueada dispara o paywall, mesmo de features futuras. Guard local só evita flash em transições conhecidas.

### D2. Distinguir 403 de billing vs 403 de outras causas

**Decisão:** ler body JSON e olhar campo `error`. Se `error` ∈ `{ "SUBSCRIPTION_EXPIRED", "SUBSCRIPTION_NOT_FOUND", "MONTHLY_LIMIT_EXCEEDED" }`, é billing; outros 403 passam adiante.

**Alternativas:**
- Header próprio `X-Billing-Reason` — mais limpo mas exigiria mudança no backend. Não vale.
- Status 402 Payment Required — semanticamente melhor, mas backend usa 403 e não vamos pedir mudança só por estética.

**Risco:** ler body consome o stream; precisa bufferizar. Resolver com `response.Content.LoadIntoBufferAsync()` antes de tentar deserializar, e devolver o response intacto se não for billing.

### D3. Confirmação de pagamento: três caminhos convergentes

**Decisão:** após `/success` na WebView, chamar `GET /api/billing/me` imediatamente; se ainda `expired=true`, polar `GET /api/billing/invoices/{id}` (3s × 20 tentativas = 60s) até `status=Paid`. Se timeout, mostrar "Pagamento em processamento — verifique em instantes" e voltar pra `SubscriptionPage` (não trava o usuário).

**Por quê três caminhos (handler, polling, lifecycle):**
1. Webhook chega antes da WebView fechar → polling 1ª tentativa já vê
2. Webhook atrasa → lifecycle revalida no resume
3. Usuário fecha app → próxima request bate 403 → handler dispara paywall (e dali pode tentar de novo)

### D4. Cache local: TTL e invalidação

**Decisão:** manter cache em `Preferences` (já existe), mas:
- TTL padrão reduzido de 24h → 1h (cache normal)
- `IBillingCacheService.Invalidate()` chamado pelo handler em 403 e pelo `PaymentWebViewVM` no sucesso
- `IBillingCacheService.MarkPendingPayment(invoiceId)` opcional: força fetch fresco enquanto invoice ainda está pendente

**Por quê:** 24h era frouxo demais. 1h equilibra economia de chamadas ao `/me` com latência de detectar expiração silenciosa (caso usuário não faça nenhuma request por horas).

### D5. Lifecycle: `Window.Resumed`

**Decisão:** subscrever em `Window.Resumed` (MAUI nativo) no `App.cs`. Ao acordar, se `BillingCacheService.LastCheckUtc` foi há >2min, dispara revalidação assíncrona em background (sem bloquear UI). Resultado: se expirou, `IRootNavigator.ShowSubscription()`; se ok, atualiza cache silencioso.

**Alternativas:**
- Plugin `Plugin.LifecycleEvents` — overhead desnecessário; MAUI já oferece nativo.
- Timer periódico em background — bateria; e cache TTL de 1h já cobre janela longa.

**Risco:** comportamento de `Resumed` pode variar por plataforma (iOS suspend vs background). Mitigação: testar nas 3 plataformas (Android, iOS, Windows) e ajustar handler se necessário.

### D6. App.CreateWindow async

**Decisão:** trocar `session.LoadAsync().GetAwaiter().GetResult()` (bloqueia UI thread) por padrão async com SplashPage temporária enquanto resolve session+billing. `CreateWindow` cria a Window com SplashPage; um `Task.Run` async resolve token + billing e dispara `IRootNavigator.ShowLogin/ShowMain/ShowSubscription`.

**Por quê:** `GetAwaiter().GetResult()` em UI thread é receita pra deadlock. Splash dá UX melhor que tela branca.

### D7. ViewModels não dependem mais de ISubscriptionGuard

**Decisão:** remover `ISubscriptionGuard` dos construtores de `DashboardViewModel`, `ClientViewModel`, `WorkorderViewModel`. Manter `LoginViewModel` (precisa pra decidir se vai pra Subscription ou Main pós-login) e `FlyoutNavigationService` (fast-path).

**Trade-off:** rompe assinatura de ctor (binário). Como tudo é interno ao app, sem impacto externo.

## Risks / Trade-offs

- **[R1] Body de 403 pode não ser JSON em rotas de erro genérico do ASP.NET** → handler usa try/catch ao deserializar; em falha, trata como 403 não-billing e propaga.
- **[R2] Polling de invoice pode hammer servidor se concorrente com webhook lento** → limite hard de 20 tentativas e backoff exponencial leve (3s, 3s, 4s, 5s, ...); cancelado se usuário sair da tela.
- **[R3] `Window.Resumed` em iOS pode disparar com cache válido em RAM mas sessão JWT expirada no servidor** → handler de 401 (separado) cobre; aqui só billing.
- **[R4] Race entre handler e ViewModel removendo guard** → handler usa `MainThread.BeginInvokeOnMainThread` para `ShowSubscription`, evitando race com ViewModel ainda processando response.
- **[R5] Pré-requisito backend (`MONTHLY_LIMIT_EXCEEDED`) pode atrasar** → tratar como gracioso: o handler reconhece o código quando aparecer; até lá, o app só mostra `expired` (que já existe). Sem hard-dependency.
- **[R6] `Preferences` é por-app sem isolamento por usuário** → ao deslogar, `BillingCacheService.Clear()` deve ser chamado (já é, via `AuthSession.ClearAsync` após T1.5).

## Migration Plan

1. Adicionar `BillingDelegatingHandler` e registrar em todos `AddHttpClient` — sem remover `IsExpiredAsync` ainda. Coexistem.
2. Trocar `PaymentWebViewVM` para confirmar no servidor + polling.
3. Adicionar `Window.Resumed` no `App.cs`.
4. Adicionar `RefreshView` no `SubscriptionPage`.
5. Refatorar `App.CreateWindow` para async + Splash.
6. Remover `ISubscriptionGuard` dos ViewModels de feature (Dashboard/Client/Workorder).
7. Limpar duplicação `Preferences.Remove("billing_*")` em `AuthSession.ClearAsync`.
8. Reduzir TTL de cache para 1h.

Rollback: cada passo é independente; reverter individualmente se causar regressão.

## Open Questions

- **OQ1.** Quando `MONTHLY_LIMIT_EXCEEDED` for adicionado no backend, app deve forçar `ShowSubscription` ou só toast e ficar na tela atual? **Sugestão:** toast + manter na tela; só forçar Subscription se for `EXPIRED`/`NOT_FOUND`.
- **OQ2.** Polling de invoice deve permitir cancelar via botão "Cancelar verificação" ou só timeout? **Sugestão:** botão visível durante polling.
- **OQ3.** `Window.Resumed` em Windows desktop dispara com qualquer foco — adicionar throttle de 30s além do cache TTL? **Sugestão:** sim, evita revalidar a cada Alt-Tab.
