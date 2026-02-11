using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.Views;

namespace OsFacil.Mobile.Api.Services.Navigation;

public interface IRootNavigator
{
    void ShowLogin();
    void ShowMain();
    Task Logout();

}
