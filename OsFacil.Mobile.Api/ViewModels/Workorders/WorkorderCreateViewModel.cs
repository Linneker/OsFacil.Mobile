using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OsFacil.Mobile.Api.Models.Workorders;
using OsFacil.Mobile.Api.Services;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Workorders.Messages;
using OsFacil.Mobile.Service.Https.Clients;
using OsFacil.Mobile.Service.Https.Clients.Response;
using OsFacil.Mobile.Service.Https.Workorders;
using System.Collections.ObjectModel;
using System.Globalization;

namespace OsFacil.Mobile.Api.ViewModels.Workorders;

public partial class WorkorderCreateViewModel : ObservableObject
{
    private readonly IWorkspaceHttp _workspaceHttp;
    private readonly IClientHttp _clientHttp;
    private readonly IAuthSession _session;
    private readonly IToastService _toast;
    private readonly IFlyoutNavigationService _navagation;
    private const string ErrorBorder = "#DC2626";

    public WorkorderCreateViewModel(IWorkspaceHttp workspaceHttp, IClientHttp clientHttp, IAuthSession session, IToastService toast, IFlyoutNavigationService navagation)
    {
        _workspaceHttp = workspaceHttp;
        _clientHttp = clientHttp;
        _session = session;
        _toast = toast;
        _navagation = navagation;
    }

    [ObservableProperty] private CreateWorkorderModel workorderModel = new();
    [ObservableProperty] private ObservableCollection<ClientHttpResponse> clients = new();
    [ObservableProperty] private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    public bool HasClientError => !string.IsNullOrWhiteSpace(workorderModel.ClientError);
    public bool HasTitleError => !string.IsNullOrWhiteSpace(workorderModel.TitleError);
    public bool HasDescriptionError => !string.IsNullOrWhiteSpace(workorderModel.DescriptionError);
    public bool HasAmountError => !string.IsNullOrWhiteSpace(workorderModel.AmountError);

    public string SaveButtonText => IsBusy ? "Salvando..." : "Salvar";

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    [RelayCommand]
    private async Task LoadClientsAsync()
    {
        try
        {
            var token = _session.AccessToken ?? "";
            var response = await _clientHttp.GetClientsAsync(token, "", 1, 100);
            if (response.Data is not null)
            {
                Clients.Clear();
                foreach (var c in response.Data)
                    Clients.Add(c);
            }
        }
        catch
        {
            await _toast.ShowAsync("Não foi possível carregar os clientes.");
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        WorkorderModel.ClearErrors();

        if (!Validate())
        {
            OnPropertyChanged(nameof(HasClientError));
            OnPropertyChanged(nameof(HasTitleError));
            OnPropertyChanged(nameof(HasDescriptionError));
            OnPropertyChanged(nameof(HasAmountError));
            return;
        }

        try
        {
            IsBusy = true;

            decimal? amount = null;
            if (!string.IsNullOrWhiteSpace(WorkorderModel.AmountText))
                TryParseDecimal(WorkorderModel.AmountText, out amount);

            var token = _session.AccessToken ?? "";
            var response = await _workspaceHttp.CreateWorkOrderAsync(token, new()
            {
                ClientId = Guid.Parse(WorkorderModel.SelectedClient.Id),
                Title = WorkorderModel.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(WorkorderModel.Description) ? null : WorkorderModel.Description.Trim(),
                Amount = amount,
            });

            var reaload = true;
            if (response.IsSuccessStatusCode)
            {
                await _toast.ShowAsync("Ordem de serviço cadastrada com sucesso!");
            }
            else
            {
                reaload = false;
                await _toast.ShowAsync(response.Error);
            }

            await BackAsync(reaload);
        }
        catch (Exception ex)
        {
            await _toast.ShowAsync("Não foi possível cadastrar a ordem de serviço.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task BackAsync(bool reaload = true)
    {
        WeakReferenceMessenger.Default.Send(new WorkordersChangedMessage(reaload));
        WorkorderModel.Clean();
        return _navagation.PopAsync();
    }

    private bool Validate()
    {
        var ok = true;

        if (WorkorderModel.SelectedClient is null)
        {
            WorkorderModel.ClientError = "Selecione um cliente.";
            WorkorderModel.ClientBorderColor = ErrorBorder;
            ok = false;
        }

        if (string.IsNullOrWhiteSpace(WorkorderModel.Title))
        {
            WorkorderModel.TitleError = "Informe o título da ordem de serviço.";
            WorkorderModel.TitleBorderColor = ErrorBorder;
            ok = false;
        }

        if (!string.IsNullOrWhiteSpace(WorkorderModel.AmountText))
        {
            if (!TryParseDecimal(WorkorderModel.AmountText, out _))
            {
                WorkorderModel.AmountError = "Valor inválido.";
                WorkorderModel.AmountBorderColor = ErrorBorder;
                ok = false;
            }
        }

        OnPropertyChanged(nameof(HasClientError));
        OnPropertyChanged(nameof(HasTitleError));
        OnPropertyChanged(nameof(HasDescriptionError));
        OnPropertyChanged(nameof(HasAmountError));

        return ok;
    }

    private static bool TryParseDecimal(string? text, out decimal? value)
    {
        value = null;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var t = text.Trim();

        if (decimal.TryParse(t, NumberStyles.Number, new CultureInfo("pt-BR"), out var parsed))
        {
            value = parsed;
            return true;
        }

        if (decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
        {
            value = parsed;
            return true;
        }

        return false;
    }
}
