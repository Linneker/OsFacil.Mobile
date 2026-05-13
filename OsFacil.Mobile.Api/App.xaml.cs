using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using OsFacil.Mobile.Api.Services;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Messages;
using OsFacil.Mobile.Api.Views;
using OsFacil.Mobile.Service.Https.Billing;

namespace OsFacil.Mobile.Api
{
    public partial class App : Application
    {
        private static readonly TimeSpan ResumeRevalidateAfter = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan ResumeThrottle = TimeSpan.FromSeconds(30);
        private static DateTime _lastResumeRevalidationUtc = DateTime.MinValue;

        private readonly IServiceProvider _sp;

        public App(IServiceProvider sp)
        {
            InitializeComponent();
            _sp = sp;

            // Subscribe billing events from the HTTP delegating handler.
            WeakReferenceMessenger.Default.Register<BillingExpiredMessage>(this, (_, _) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var root = _sp.GetRequiredService<IRootNavigator>();
                    root.ShowSubscription();
                });
            });

            WeakReferenceMessenger.Default.Register<MonthlyLimitExceededMessage>(this, (_, msg) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var toast = _sp.GetRequiredService<IToastService>();
                    var text = msg.Limit.HasValue
                        ? $"Limite mensal de {msg.Limit} OS atingido."
                        : "Limite mensal de OS atingido.";
                    await toast.ShowAsync(text);
                });
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var splash = new SplashPage();
            var window = new Window(splash);
            window.Activated += OnWindowActivated;
            window.Resumed += OnWindowResumed;

            _ = ResolveInitialPageAsync();
            return window;
        }

        private async Task ResolveInitialPageAsync()
        {
            var session = _sp.GetRequiredService<IAuthSession>();
            var root = _sp.GetRequiredService<IRootNavigator>();

            await session.LoadAsync();

            if (string.IsNullOrWhiteSpace(session.AccessToken))
            {
                MainThread.BeginInvokeOnMainThread(root.ShowLogin);
                return;
            }

            var billingHttp = _sp.GetRequiredService<IBillingHttp>();
            var cache = _sp.GetRequiredService<IBillingCacheService>();
            try
            {
                var status = await billingHttp.GetStatusAsync(session.AccessToken);
                if (status.IsSuccessStatusCode && status.Data is not null)
                {
                    await cache.CacheAsync(status.Data);
                    if (status.Data.Expired)
                    {
                        MainThread.BeginInvokeOnMainThread(root.ShowSubscription);
                        return;
                    }
                }
            }
            catch
            {
                // Network failure: fall through to Main; first protected request
                // that fails will trigger the BillingDelegatingHandler path.
            }

            MainThread.BeginInvokeOnMainThread(root.ShowMain);
        }

        private void OnWindowActivated(object? sender, EventArgs e)
        {
            // First activation also goes through here; the splash flow already
            // triggered the initial check, so just delegate to the resume path
            // which has its own throttle/TTL guard.
        }

        private void OnWindowResumed(object? sender, EventArgs e)
        {
            var now = DateTime.UtcNow;
            if (now - _lastResumeRevalidationUtc < ResumeThrottle)
                return;

            var cache = _sp.GetRequiredService<IBillingCacheService>();
            var lastCheck = cache.LastCheckUtc;
            if (lastCheck is not null && now - lastCheck.Value < ResumeRevalidateAfter)
                return;

            _lastResumeRevalidationUtc = now;
            _ = RevalidateBillingAsync();
        }

        private async Task RevalidateBillingAsync()
        {
            var session = _sp.GetRequiredService<IAuthSession>();
            if (string.IsNullOrWhiteSpace(session.AccessToken))
                return;

            var billingHttp = _sp.GetRequiredService<IBillingHttp>();
            var cache = _sp.GetRequiredService<IBillingCacheService>();
            try
            {
                var status = await billingHttp.GetStatusAsync(session.AccessToken);
                if (!status.IsSuccessStatusCode || status.Data is null)
                    return;

                await cache.CacheAsync(status.Data);
                if (status.Data.Expired)
                {
                    var root = _sp.GetRequiredService<IRootNavigator>();
                    MainThread.BeginInvokeOnMainThread(root.ShowSubscription);
                }
            }
            catch
            {
                // Ignore: handler will catch the next protected 403.
            }
        }
    }
}
