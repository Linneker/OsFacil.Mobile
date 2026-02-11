using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Models.Workorders;

public partial class WorkorderModel : ObservableObject
{
    [ObservableProperty] private Guid id;
    [ObservableProperty] private string clientName;
    [ObservableProperty] private string title;
    [ObservableProperty] private string status;
    [ObservableProperty] private decimal amount;
}
