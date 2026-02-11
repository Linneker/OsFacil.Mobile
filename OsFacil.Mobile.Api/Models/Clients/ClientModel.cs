using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Models.Clients;

public partial class ClientModel: ObservableObject
{
    [ObservableProperty]
    private string id = string.Empty;
    [ObservableProperty]
    private string name = string.Empty;
    [ObservableProperty]
    private string phone = string.Empty;
    [ObservableProperty]
    private string email = string.Empty;

}
