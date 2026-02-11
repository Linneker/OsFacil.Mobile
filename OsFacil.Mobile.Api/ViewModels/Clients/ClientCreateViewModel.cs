using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OsFacil.Mobile.Api.Models.Clients;
using OsFacil.Mobile.Api.Services;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Clients.Messagens;
using OsFacil.Mobile.Service.Https.Clients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OsFacil.Mobile.Api.ViewModels.Clients;

public partial class ClientCreateViewModel : ObservableObject
{
    private readonly IClientHttp _clientHttp;
    private readonly IAuthSession _session;
    private readonly IToastService _toast;
    private readonly IFlyoutNavigationService _navagation;
    private const string ErrorBorder = "#DC2626";

    public ClientCreateViewModel(IClientHttp clientHttp, IAuthSession session, IToastService toast, IFlyoutNavigationService navagation)
    {
        _clientHttp = clientHttp;
        _session = session;

        _toast = toast;
        _navagation = navagation;
    }


    [ObservableProperty] private CreateClientModel clientModel = new();

    [ObservableProperty] private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    public bool HasNameError => !string.IsNullOrWhiteSpace(clientModel.NameError);
    public bool HasPhoneError => !string.IsNullOrWhiteSpace(clientModel.PhoneError);
    public bool HasEmailError => !string.IsNullOrWhiteSpace(clientModel.EmailError);


    public string SaveButtonText => IsBusy ? "Salvando..." : "Salvar";

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        ClientModel.ClearErrors();

        if (!Validate())
        {
            // atualiza flags computed
            OnPropertyChanged(nameof(HasNameError));
            OnPropertyChanged(nameof(HasPhoneError));
            OnPropertyChanged(nameof(HasEmailError));
            return;
        }

        try
        {
            IsBusy = true;

            var token = _session.AccessToken ?? "";
            var response = await _clientHttp.CreateClientAsync(token, new()
            {
                Name = ClientModel.Name.Trim(),
                Phone = ClientModel.Phone.Trim(),
                Email = ClientModel.Email.Trim(),
            });
            var reaload = true;
            if (response.IsSuccessStatusCode) { 
                // feedback simples (depois a gente coloca toast padrão)
                await _toast.ShowAsync("Cliente cadastrado com sucesso!");
            }
            else
            {
                reaload = false;
                await _toast.ShowAsync(response.Error);
            }
            // volta
            await BackAsync(reaload);
        }
        catch (Exception ex)
        {
            await _toast.ShowAsync("Não foi possível cadastrado o cliente.");
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    private Task BackAsync(bool reaload = true)
    {
        WeakReferenceMessenger.Default.Send(new ClientsChangedMessage(reaload));
        ClientModel.Clean();
        return _navagation.PopAsync();
    }

    private bool Validate()
    {
        var ok = true;

        if (string.IsNullOrWhiteSpace(ClientModel.Name))
        {
            ClientModel.NameError = "Informe o nome do cliente.";
            ClientModel.NameBorderColor = ErrorBorder;
            ok = false;
        }

        // telefone (bem simples por enquanto)
        if (string.IsNullOrWhiteSpace(ClientModel.Phone))
        {
            ClientModel.PhoneError = "Informe um telefone.";
            ClientModel.PhoneBorderColor = ErrorBorder;
            ok = false;
        }

        if (!string.IsNullOrWhiteSpace(ClientModel.Email))
        {
            // validação simples
            if (!Regex.IsMatch(ClientModel.Email.Trim(), @"^\S+@\S+\.\S+$"))
            {
                ClientModel.EmailError = "Email inválido.";
                ClientModel.EmailBorderColor = ErrorBorder;
                ok = false;
            }
        }

        // atualiza computed
        OnPropertyChanged(nameof(HasNameError));
        OnPropertyChanged(nameof(HasPhoneError));
        OnPropertyChanged(nameof(HasEmailError));

        return ok;
    }

}
