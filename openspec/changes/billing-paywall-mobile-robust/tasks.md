## 1. HTTP delegating handler

- [x] 1.1 Create `OsFacil.Mobile.Service/Http/BillingDelegatingHandler.cs` (new file in Service project) implementing `DelegatingHandler` that buffers response, attempts to deserialize JSON `{ error, validUntil? }` on 403, and exposes a callback for billing events
- [x] 1.2 Define `IBillingEventBus` (or use `WeakReferenceMessenger.Default` with `BillingExpiredMessage` / `MonthlyLimitExceededMessage`) so the handler can signal the Api project without taking a hard dependency
- [x] 1.3 Register `BillingDelegatingHandler` as transient in `MauiProgram.cs` and chain it via `.AddHttpMessageHandler<BillingDelegatingHandler>()` on every `AddHttpClient` call inside `OsFacil.Mobile.Service/DependencyInjectionHttp.cs`
- [x] 1.4 In `App.xaml.cs` (or a startup hook), subscribe to `BillingExpiredMessage` and call `IRootNavigator.ShowSubscription()` on the main thread
- [x] 1.5 Subscribe to `MonthlyLimitExceededMessage` to show a toast via `IToastService` without changing root navigation

## 2. Cache and session cleanup

- [x] 2.1 Add `IBillingCacheService.LastCheckUtc` (DateTime?) and update `BillingCacheService` to expose it from `Preferences[DateKey]`
- [x] 2.2 Reduce default TTL constant in `BillingCacheService.NeedsRefresh()` from 24 hours to 1 hour
- [x] 2.3 In `Services/Session/AuthSession.cs`, replace `Preferences.Remove("billing_data")` and `Preferences.Remove("billing_date")` with a call to `IBillingCacheService.Clear()` (inject `IBillingCacheService`)
- [x] 2.4 Confirm `BillingCacheService.Clear()` removes both keys and is idempotent

## 3. Payment confirmation flow

- [x] 3.1 Add `GetInvoiceAsync(string token, Guid invoiceId, CancellationToken)` to `IBillingHttp` and implement in `BillingHttp` calling `GET /api/billing/invoices/{id}`
- [x] 3.2 Create response DTO `BillingInvoiceResponse` with fields `invoiceId`, `plan`, `months`, `amountCents`, `workOrderLimit`, `status`, `preferenceId`, `paymentId`
- [x] 3.3 In `SubscriptionViewModel.PayAsync`, store the `invoiceId` returned from `RenewAsync` so `PaymentWebViewViewModel` can use it (pass via constructor parameter or `WeakReferenceMessenger`)
- [x] 3.4 In `PaymentWebViewViewModel.HandleNavigationAsync`, on success URL match: call `IBillingHttp.GetStatusAsync` first; if `expired==true`, start polling `IBillingHttp.GetInvoiceAsync` (max 20 attempts, 3s interval)
- [x] 3.5 Add `[ObservableProperty] bool IsConfirmingPayment` and bind a "Cancelar verificação" button visible during polling to a `CancelPollingCommand`
- [x] 3.6 On polling timeout, navigate back to `SubscriptionPage` with toast "Pagamento em processamento — verifique em instantes"
- [x] 3.7 On successful confirmation (server says active), call `IBillingCacheService.Clear()`, pop WebView, call `IRootNavigator.ShowMain()`

## 4. Lifecycle and startup

- [x] 4.1 In `App.xaml.cs`, override `CreateWindow` to return a Window initially bound to a new `SplashPage` and start an async task that resolves session + billing
- [x] 4.2 Create minimal `Views/SplashPage.xaml` with logo and ActivityIndicator (no ViewModel)
- [x] 4.3 In the async startup task: load token; if empty → `ShowLogin()`; else call `GetStatusAsync`; if `expired` → `ShowSubscription()`; else → `ShowMain()`
- [x] 4.4 In `App.xaml.cs`, hook `Window.Resumed` (or `Application.Current.Windows[0].Resumed`); on fire, check `IBillingCacheService.LastCheckUtc`; if older than 2 minutes, call `GetStatusAsync` and navigate to SubscriptionPage if `expired`
- [x] 4.5 Add 30s throttle to the resume handler (track last invocation timestamp in a static field) so rapid focus changes do not hammer the server
- [ ] 4.6 Test resume behavior on Android, Windows, and iOS (manual verification — note any platform-specific quirks in `design.md` Open Questions section)

## 5. SubscriptionPage UX

- [x] 5.1 Wrap the existing `<ScrollView>` of `SubscriptionPage.xaml` in a `<RefreshView>` bound to `IsRefreshing` and `RefreshCommand`
- [x] 5.2 Add `[ObservableProperty] bool isRefreshing` and `[RelayCommand] async Task RefreshAsync()` to `SubscriptionViewModel` that forces a fresh fetch from `GetStatusAsync` regardless of cache
- [ ] 5.3 Verify pull-to-refresh works on Android and Windows (RefreshView is fully supported)

## 6. Remove proactive guards from feature ViewModels

- [x] 6.1 Remove `ISubscriptionGuard _guard` field, constructor parameter, and `IsExpiredAsync` calls from `ViewModels/Dashboard/DashboardViewModel.cs`
- [x] 6.2 Same for `ViewModels/Clients/ClientViewModel.cs`
- [x] 6.3 Same for `ViewModels/Workorders/WorkorderViewModel.cs`
- [x] 6.4 Keep `ISubscriptionGuard` in `LoginViewModel` (post-login routing) and in `FlyoutNavigationService` (fast-path UX)
- [x] 6.5 Verify all ViewModel constructor parameters in `MauiProgram.cs` registrations still compile and resolve

## 7. Verification

- [ ] 7.1 Manual smoke test: log in with a valid subscription → enter Dashboard → revoke subscription server-side (set `validUntil` to past) → next navigation must show SubscriptionPage via handler
- [ ] 7.2 Manual smoke test: from expired state, complete a payment in the WebView → confirm app reaches Main without manual restart
- [ ] 7.3 Manual smoke test: from expired state, close the app during pending payment → reopen → app starts on SubscriptionPage; after webhook confirms, pull-to-refresh moves to Main
- [ ] 7.4 Manual smoke test: send app to background for 5 minutes → resume → app revalidates billing automatically
- [x] 7.5 Run `openspec validate billing-paywall-mobile-robust` and resolve any reported issues
