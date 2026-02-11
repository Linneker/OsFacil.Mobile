# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Restore dependencies
dotnet restore

# Build for specific platform
dotnet build OsFacil.Mobile.Api -f net10.0-android
dotnet build OsFacil.Mobile.Api -f net10.0-windows10.0.19041.0
dotnet build OsFacil.Mobile.Api -f net10.0-ios
dotnet build OsFacil.Mobile.Api -f net10.0-maccatalyst

# Run on Windows
dotnet run --project OsFacil.Mobile.Api -f net10.0-windows10.0.19041.0
```

No test projects exist in the solution.

## Architecture

This is a **.NET 10 MAUI** cross-platform mobile app ("Os Facil") for managing service orders. App ID: `br.com.acmesistemas.osfacil`. Solution file: `OsFacil.Mobile.slnx`.

### Two-Project Structure

- **OsFacil.Mobile.Api** — The MAUI application: views, view models, models, services, platform code, and resources.
- **OsFacil.Mobile.Service** — Class library containing HTTP client abstractions (interfaces, typed HttpClients, request/response DTOs).

### MVVM with CommunityToolkit.Mvvm

All ViewModels and Models inherit from `ObservableObject`. Use `[ObservableProperty]` for bindable properties and `[RelayCommand]` for ICommand generation. Each feature has a triad: **View** (XAML page) → **ViewModel** → **Model** (observable state), all registered as scoped services in `MauiProgram.cs`.

### Navigation

Two navigation layers, both service-based (not Shell):
- **IRootNavigator** — Switches between login and main app (`App.CreateWindow` checks `IAuthSession.AccessToken` to decide initial page).
- **IFlyoutNavigationService** — Routes between feature pages inside `FlyoutApp` (FlyoutPage). Routes: `"dashboard"`, `"clients"`, `"workorders"`.

### HTTP Services

Typed HttpClients registered via `DependencyInjectionHttp.AddHttpServices()`. Each feature domain (Login, Clients, Metrics, Workorders) has an interface + implementation pair. Responses use generic wrappers: `ResponseHttps<T>` (single item) and `ResponsesHttps<T>` (list with `PagedResponse` metadata).

Base URL configured in `appsettings.json` (`UrlOsFacilApi`), loaded as an embedded resource at startup.

### Auth

Token stored in platform-native `SecureStorage` via `IAuthSession`. Login flow: `LoginHttp` → store token → `IRootNavigator.ShowMain()`.

## Key Dependencies

- **CommunityToolkit.Mvvm** (8.4.0) — MVVM source generators
- **CommunityToolkit.Maui** (14.0.0) — Toast notifications, UI extensions
- **Syncfusion.Maui.Toolkit** (1.0.8) — UI controls
- **Microsoft.Data.Sqlite.Core** — Local SQLite database (config in `Data/Constants.cs`)
- **Microsoft.Extensions.Http / Http.Polly** — HttpClient factory and resilience

## Conventions

- All DI registrations are **Scoped** lifetime.
- Configuration loaded from embedded `appsettings.json` via `Assembly.GetManifestResourceStream`.
- Portuguese-language comments and some method names (e.g., `Logging` for login action).
- XAML styles split across `Colors.xaml`, `Typography.xaml`, `Components.xaml`, `AppStyles.xaml`.
- Fonts: OpenSans, SegoeUI-Semibold, FluentSystemIcons (alias `FluentUI.FontFamily`).
- Windows platform has a workaround for `CollectionView` keyboard focus in `MauiProgram.cs`.
