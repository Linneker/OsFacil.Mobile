## Why

O paywall do mobile hoje depende de checagens espalhadas (`ISubscriptionGuard.IsExpiredAsync` em 5 lugares) e de detecção de pagamento por substring de URL na WebView do Mercado Pago, o que é frágil: se o redirect do MP falhar, se o usuário fechar a WebView antes da URL `/success` aparecer, ou se o webhook chegar 30s depois, o cliente pode ficar travado em cache até 24h ou liberar antes do servidor confirmar. O backend já retorna 403 com `{ error: "SUBSCRIPTION_EXPIRED" }` via `LicenseGuardMiddleware` para qualquer endpoint protegido — o app ignora esse sinal autoritativo e confia em sua própria heurística.

## What Changes

- Substituir checagem cliente-only por reação a 403 do servidor:
  - Adicionar `BillingDelegatingHandler` que intercepta toda response HTTP
  - Quando vier 403 com body `{ error: "SUBSCRIPTION_EXPIRED" | "SUBSCRIPTION_NOT_FOUND" }`, invalidar cache de billing e disparar `IRootNavigator.ShowSubscription()`
  - Quando vier 403 com `{ error: "MONTHLY_LIMIT_EXCEEDED" }` (depende da change `enforce-monthly-workorder-limit` no backend), exibir toast específico
- Confirmar pagamento no servidor após `/success` na WebView:
  - Trocar "URL contém /success → libera" por chamada a `GET /api/billing/me`
  - Se ainda `expired=true`, fazer polling em `GET /api/billing/invoices/{id}` (até 60s, intervalo 3s) esperando `status=Paid`
- Revalidar billing em eventos de ciclo de vida:
  - `Window.Resumed` (implementar — não existe hoje): se passou >2min do último check, força revalidação
  - `App.CreateWindow`: revalidar billing junto com token antes de decidir tela inicial
  - Pull-to-refresh em `SubscriptionPage`
- Remover `IsExpiredAsync` espalhado em `DashboardViewModel`, `ClientViewModel`, `WorkorderViewModel`. Manter checagem só em `LoginViewModel` (UX pós-login) e em `FlyoutNavigationService` como fast-path opcional
- Eliminar duplicação: `AuthSession.ClearAsync` deve chamar `IBillingCacheService.Clear()` em vez de `Preferences.Remove("billing_data"/"billing_date")` direto
- **BREAKING (interno)**: ViewModels deixam de depender de `ISubscriptionGuard` no construtor; assinaturas mudam

## Capabilities

### New Capabilities
- `mobile-billing-paywall`: comportamento do app frente a estado de assinatura — bloqueio reativo a 403 do backend, confirmação ativa de pagamento, revalidação em ciclo de vida, e cache local apenas como otimização de UX

### Modified Capabilities
<!-- nenhuma; não há specs existentes neste repo (specs/ está vazio) -->

## Impact

- **Código mobile afetado**:
  - `OsFacil.Mobile.Api/Services/Billing/` — novo `BillingDelegatingHandler`; `BillingCacheService` ganha método para invalidação seletiva pós-checkout
  - `OsFacil.Mobile.Api/App.xaml.cs` — `CreateWindow` async; novo handler para `Window.Resumed`
  - `OsFacil.Mobile.Api/MauiProgram.cs` — registrar `BillingDelegatingHandler` em todos os `AddHttpClient`
  - `OsFacil.Mobile.Api/Services/Session/AuthSession.cs` — usar `IBillingCacheService.Clear()`
  - `OsFacil.Mobile.Api/ViewModels/Billing/PaymentWebViewViewModel.cs` — confirmação ativa + polling
  - `OsFacil.Mobile.Api/ViewModels/Billing/SubscriptionViewModel.cs` — comando refresh
  - `OsFacil.Mobile.Api/Views/Billing/SubscriptionPage.xaml` — `RefreshView`
  - `OsFacil.Mobile.Api/ViewModels/Dashboard/DashboardViewModel.cs`, `Clients/ClientViewModel.cs`, `Workorders/WorkorderViewModel.cs` — remover `ISubscriptionGuard`
- **API consumida** (sem mudanças no mobile, contrato existente):
  - `GET /api/billing/me` — campo `expired`, `validUntil`, `monthlyWorkOrderRemaining`
  - `GET /api/billing/invoices/{id}` — campo `status` (`Pending`/`Paid`)
  - 403 com body `{ error, validUntil? }` em endpoints protegidos
- **Pré-requisito**: change `enforce-monthly-workorder-limit` no backend para que `MONTHLY_LIMIT_EXCEEDED` exista. Sem ela, a parte de limite mensal é apenas UX preventiva (botão desabilitado), sem segurança real
- **Risco**: `Window.Resumed` não está implementado — precisa ser adicionado e testado por plataforma (Android/iOS/Windows)
