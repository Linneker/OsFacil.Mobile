using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OsFacil.Mobile.Api.Models;
using OsFacil.Mobile.Api.Models.Billing;
using OsFacil.Mobile.Api.Models.Clients;
using OsFacil.Mobile.Api.Models.Dashboard;
using OsFacil.Mobile.Api.Models.Workorders;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels;
using OsFacil.Mobile.Api.ViewModels.Billing;
using OsFacil.Mobile.Api.ViewModels.Clients;
using OsFacil.Mobile.Api.ViewModels.Workorders;
using OsFacil.Mobile.Api.Views;
using OsFacil.Mobile.Api.Views.Billing;
using OsFacil.Mobile.Api.Views.Clients;
using OsFacil.Mobile.Api.Views.Dashboard;
using OsFacil.Mobile.Api.Views.Workorders;
using OsFacil.Mobile.Service;
using Syncfusion.Maui.Toolkit.Hosting;
using System.Reflection;

namespace OsFacil.Mobile.Api
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var assembly = Assembly.GetExecutingAssembly();
            // Substitua "NomeDoSeuProjeto" pelo namespace do seu projeto
            using var stream = assembly.GetManifestResourceStream("OsFacil.Mobile.Api.appsettings.json");

            if (stream != null)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

                builder.Configuration.AddConfiguration(config);
            }


            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
    				{
    					handler.PlatformView.SingleSelectionFollowsFocus = false;
    				});

#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif


            builder.Services.AddScoped<IAuthSession, AuthSession>();
            builder.Services.AddScoped<IRootNavigator, RootNavigator>();



            builder.Services.AddScoped<ModalErrorHandler>();
            builder.Services.AddScoped<IToastService, ToastService>();

            builder.Services.AddScoped<LoginViewModel>();
            builder.Services.AddScoped<LoginModel>();
            builder.Services.AddScoped<LoginPage>();

            builder.Services.AddScoped<RegisterViewModel>();
            builder.Services.AddScoped<RegisterModel>();
            builder.Services.AddScoped<RegisterPage>();

            builder.Services.AddScoped<ClientViewModel>();
            builder.Services.AddScoped<ClientModel>();
            builder.Services.AddScoped<ClientPage>();

            builder.Services.AddScoped<ClientCreatePage>();
            builder.Services.AddScoped<ClientCreateViewModel>();
            builder.Services.AddScoped<CreateClientModel>();

            builder.Services.AddScoped<DashboardPage>();
            builder.Services.AddScoped<DashboardViewModel>();
            builder.Services.AddScoped<DashboardModel>();


            builder.Services.AddScoped<WorkorderPage>();
            builder.Services.AddScoped<WorkorderViewModel>();
            builder.Services.AddScoped<WorkorderModel>();

            builder.Services.AddScoped<WorkorderCreatePage>();
            builder.Services.AddScoped<WorkorderCreateViewModel>();
            builder.Services.AddScoped<CreateWorkorderModel>();

            builder.Services.AddScoped<WorkorderEditPage>();
            builder.Services.AddScoped<WorkorderEditViewModel>();
            builder.Services.AddScoped<EditWorkorderModel>();

            builder.Services.AddScoped<IBillingCacheService, BillingCacheService>();
            builder.Services.AddScoped<SubscriptionPage>();
            builder.Services.AddScoped<SubscriptionViewModel>();
            builder.Services.AddScoped<SubscriptionModel>();
            builder.Services.AddScoped<PaymentWebViewPage>();
            builder.Services.AddScoped<PaymentWebViewViewModel>();

            builder.Services.AddHttpServices(builder.Configuration);


            builder.Services.AddScoped<IFlyoutNavigationService, FlyoutNavigationService>();

            builder.Services.AddScoped<MenuPage>();
            builder.Services.AddScoped<FlyoutApp>();
            builder.Services.AddScoped<MenuViewModel>();
            builder.Services.AddScoped<CategoryModel>();
            

            return builder.Build();
        }
    }
}
