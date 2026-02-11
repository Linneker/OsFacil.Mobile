using CommunityToolkit.Mvvm.ComponentModel;
using OsFacil.Mobile.Service.Https.Clients.Response;

namespace OsFacil.Mobile.Api.Models.Workorders;

public partial class CreateWorkorderModel : ObservableObject
{
    private const string DefaultBorder = "#E2E8F0";

    [ObservableProperty] private ClientHttpResponse selectedClient;
    [ObservableProperty] private string title;
    [ObservableProperty] private string description;
    [ObservableProperty] private string amountText;

    [ObservableProperty] private string clientBorderColor;
    [ObservableProperty] private string titleBorderColor;
    [ObservableProperty] private string descriptionBorderColor;
    [ObservableProperty] private string amountBorderColor;

    // errors
    [ObservableProperty] private string clientError = "";
    [ObservableProperty] private string titleError = "";
    [ObservableProperty] private string descriptionError = "";
    [ObservableProperty] private string amountError = "";

    public CreateWorkorderModel()
    {
        ClientBorderColor = DefaultBorder;
        TitleBorderColor = DefaultBorder;
        DescriptionBorderColor = DefaultBorder;
        AmountBorderColor = DefaultBorder;
    }

    public void Clean()
    {
        SelectedClient = null;
        Title = "";
        Description = "";
        AmountText = "";
        ClearErrors();
    }

    public void ClearErrors()
    {
        ClientError = "";
        TitleError = "";
        DescriptionError = "";
        AmountError = "";

        ClientBorderColor = DefaultBorder;
        TitleBorderColor = DefaultBorder;
        DescriptionBorderColor = DefaultBorder;
        AmountBorderColor = DefaultBorder;
    }
}
