using Microsoft.Extensions.DependencyInjection;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.Views;

namespace OsFacil.Mobile.Api
{
    public partial class App : Application
    {
        private readonly IServiceProvider _sp;

        public App(IServiceProvider sp)
        {
            InitializeComponent();
            _sp = sp;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var root = _sp.GetRequiredService<IRootNavigator>();
            var session = _sp.GetRequiredService<IAuthSession>();

            // Inicia sempre no Login
            Page rootPage =
                !string.IsNullOrWhiteSpace(session.AccessToken)
                ? _sp.GetRequiredService<FlyoutApp>()                   // pós-login
                : new NavigationPage(_sp.GetRequiredService<LoginPage>());   // pré-login

            // Retorna uma Window vazia (a Page foi setada no ShowLogin)
            return new Window(rootPage);

        }
    }
}