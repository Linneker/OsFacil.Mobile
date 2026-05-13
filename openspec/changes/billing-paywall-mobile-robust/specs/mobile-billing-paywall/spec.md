## ADDED Requirements

### Requirement: HTTP 403 SUBSCRIPTION_EXPIRED triggers paywall navigation

The mobile app SHALL treat HTTP 403 responses with body `{ "error": "SUBSCRIPTION_EXPIRED" }` or `{ "error": "SUBSCRIPTION_NOT_FOUND" }` as authoritative billing-blocked signals from the server, regardless of which endpoint produced them.

#### Scenario: Authenticated request to protected endpoint returns billing 403

- **WHEN** any HTTP request from the app receives status 403 with JSON body containing `error` field equal to `SUBSCRIPTION_EXPIRED` or `SUBSCRIPTION_NOT_FOUND`
- **THEN** the app SHALL invalidate the local billing cache, navigate the root window to the SubscriptionPage, and not surface the 403 as a generic error to the calling ViewModel

#### Scenario: 403 with non-billing error code passes through

- **WHEN** an HTTP 403 response has a body whose `error` field is not a billing code (or whose body is not parseable JSON)
- **THEN** the app SHALL propagate the response unchanged to the calling code without navigating to SubscriptionPage

### Requirement: HTTP 403 MONTHLY_LIMIT_EXCEEDED shows specific feedback

The mobile app SHALL react to HTTP 403 responses with body `{ "error": "MONTHLY_LIMIT_EXCEEDED" }` by informing the user about the limit without forcing navigation away from the current screen.

#### Scenario: User attempts to create work order beyond monthly limit

- **WHEN** an HTTP request returns 403 with `error == "MONTHLY_LIMIT_EXCEEDED"`
- **THEN** the app SHALL show a toast message describing the limit and SHALL NOT change the root navigation; the user remains on the current page able to navigate to SubscriptionPage manually

### Requirement: Payment success is confirmed against the server

The mobile app SHALL NOT trust WebView URL substrings (such as `/success` or `/approved`) as proof of payment; it SHALL confirm subscription state by calling the server before unblocking the user.

#### Scenario: WebView reaches success URL and server confirms immediately

- **WHEN** the payment WebView navigates to a URL matching the success pattern
- **AND** a subsequent call to `GET /api/billing/me` returns `expired == false`
- **THEN** the app SHALL invalidate the billing cache, close the WebView, and navigate to the main app

#### Scenario: WebView reaches success URL but server still reports expired

- **WHEN** the payment WebView navigates to a URL matching the success pattern
- **AND** the immediate call to `GET /api/billing/me` returns `expired == true`
- **THEN** the app SHALL begin polling `GET /api/billing/invoices/{invoiceId}` at most 20 times with intervals of at least 3 seconds
- **AND** SHALL stop polling and navigate to main when the invoice `status == "Paid"`
- **AND** SHALL stop polling on user cancel or after the timeout, returning to SubscriptionPage with a toast "Pagamento em processamento"

#### Scenario: WebView reaches failure or cancellation URL

- **WHEN** the payment WebView navigates to a URL matching the failure or cancellation pattern
- **THEN** the app SHALL show a toast indicating the failure and pop the WebView without changing the subscription state

### Requirement: Billing state is revalidated on app resume

The mobile app SHALL revalidate billing state when the application window resumes from background, when the local cache is older than the configured TTL, and when the app starts.

#### Scenario: App returns from background after long idle

- **WHEN** the app's `Window.Resumed` event fires
- **AND** the time since the last billing check exceeds 2 minutes
- **THEN** the app SHALL call `GET /api/billing/me` asynchronously without blocking the UI
- **AND** SHALL navigate to SubscriptionPage if the response indicates the subscription is expired

#### Scenario: App startup with stored access token

- **WHEN** the app starts and `IAuthSession.AccessToken` is non-empty
- **THEN** the app SHALL call `GET /api/billing/me` before deciding the initial root page
- **AND** SHALL show SubscriptionPage as the initial root if `expired == true`, otherwise show the main FlyoutApp

#### Scenario: App startup with no stored token

- **WHEN** the app starts and `IAuthSession.AccessToken` is empty or missing
- **THEN** the app SHALL show LoginPage as the initial root and SHALL NOT call billing endpoints

### Requirement: Subscription page supports manual refresh

The SubscriptionPage SHALL allow the user to manually refresh billing state via a pull-to-refresh gesture.

#### Scenario: User pulls to refresh on SubscriptionPage

- **WHEN** the user performs a pull-to-refresh gesture on SubscriptionPage
- **THEN** the app SHALL call `GET /api/billing/me`, update the displayed plan, status, validUntil, monthly usage, and dismiss the refresh indicator on completion regardless of success or failure

### Requirement: Local billing cache is invalidated coherently

The mobile app SHALL ensure the local billing cache is invalidated by a single owner (`IBillingCacheService`) and that session clearing also clears the billing cache.

#### Scenario: User logs out

- **WHEN** `IAuthSession.ClearAsync` is invoked
- **THEN** the app SHALL invoke `IBillingCacheService.Clear()` and SHALL NOT call `Preferences.Remove` directly for billing keys

#### Scenario: HTTP handler invalidates cache on billing 403

- **WHEN** the BillingDelegatingHandler observes a 403 with a billing error code
- **THEN** it SHALL invoke `IBillingCacheService.Clear()` before triggering navigation

#### Scenario: Successful payment confirmed by server

- **WHEN** the payment confirmation flow observes that the server now reports the subscription as active
- **THEN** the app SHALL invoke `IBillingCacheService.Clear()` so the next read reflects the new state

### Requirement: Cache TTL keeps stale state bounded

The mobile app SHALL refresh the local billing cache from the server when its age exceeds 1 hour, even in the absence of explicit invalidation events.

#### Scenario: Cache is older than TTL when consulted

- **WHEN** any code path consults `IBillingCacheService` and the cache age is greater than 1 hour
- **THEN** the app SHALL prefer a fresh fetch from `GET /api/billing/me` and update the cache before returning

#### Scenario: Cache is fresh when consulted

- **WHEN** any code path consults `IBillingCacheService` and the cache age is at most 1 hour and `validUntil` has not passed
- **THEN** the app MAY return the cached value without contacting the server

### Requirement: Feature ViewModels do not perform proactive billing checks

ViewModels for `Dashboard`, `Clients`, and `Workorders` features SHALL NOT depend on `ISubscriptionGuard` for blocking decisions; they SHALL rely on the central HTTP handler and root navigation.

#### Scenario: Dashboard, Client, or Workorder VM loads

- **WHEN** `LoadAsync` is invoked on `DashboardViewModel`, `ClientViewModel`, or `WorkorderViewModel`
- **THEN** the VM SHALL NOT call `ISubscriptionGuard.IsExpiredAsync` and SHALL NOT navigate to SubscriptionPage based on cache; if the underlying HTTP call returns a billing 403, the BillingDelegatingHandler is responsible for navigation

#### Scenario: User logs in successfully

- **WHEN** `LoginViewModel.Logging` succeeds and a token is stored
- **THEN** the VM MAY call `GET /api/billing/me` to choose between SubscriptionPage and FlyoutApp as the post-login destination
