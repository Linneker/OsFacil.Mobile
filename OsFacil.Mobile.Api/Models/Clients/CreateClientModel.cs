using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Models.Clients;

public partial class CreateClientModel: ObservableObject
{
    private const string DefaultBorder = "#E2E8F0";

    [ObservableProperty]private string name ;
    [ObservableProperty]private string phone ;
    [ObservableProperty]private string email;

    [ObservableProperty] private string nameBorderColor;
    [ObservableProperty] private string phoneBorderColor;
    [ObservableProperty] private string emailBorderColor;

    // errors
    [ObservableProperty] private string nameError = "";
    [ObservableProperty] private string phoneError = "";
    [ObservableProperty] private string emailError = "";

    public CreateClientModel()
    {
        NameBorderColor = DefaultBorder;
        PhoneBorderColor = DefaultBorder;
        EmailBorderColor = DefaultBorder;
    }

    public void Clean()
    {
        Name = "";
        Phone = "";
        Email = "";
        ClearErrors();
    }

    public void ClearErrors()
    {
        NameError = "";
        PhoneError = "";
        EmailError = "";

        NameBorderColor = DefaultBorder;
        PhoneBorderColor = DefaultBorder;
        EmailBorderColor = DefaultBorder;

    }
}
