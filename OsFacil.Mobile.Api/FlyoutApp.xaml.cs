using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Views;
using OsFacil.Mobile.Api.Views.Clients;
using OsFacil.Mobile.Api.Views.Dashboard;

namespace OsFacil.Mobile.Api;

public partial class FlyoutApp : FlyoutPage
{
    public FlyoutApp(
        MenuPage menuPage,
        IFlyoutNavigationService nav,
        DashboardPage startPage)
    {
        InitializeComponent();

        // liga o host (FlyoutPage) no servico de navegacao
        nav.SetHost(this);
        Title = "OsFácil"; 

        Flyout = menuPage;
        Detail = new NavigationPage(startPage);

        // opcional: manter menu fechado no start
        IsPresented = false;
    }

}