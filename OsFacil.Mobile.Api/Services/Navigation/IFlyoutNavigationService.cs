using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Services.Navigation;

public interface IFlyoutNavigationService
{
    void SetHost(FlyoutPage host);
    Task NavigateToAsync(string key);
    void OpenMenu();
    void CloseMenu();

    Task PushAsync(string key);           // empilha página
    Task PopAsync();                      // volta

}
