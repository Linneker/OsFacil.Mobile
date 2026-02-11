using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Models.Workorders;

public partial class WorkorderFiltersModel : ObservableObject
{
    [ObservableProperty] private string amountText = "";
    [ObservableProperty] private string clientNameOrSlug = "";

    // DatePicker não aceita null: guardamos datas na UI e só aplicamos se o usuário apertar Aplicar
    [ObservableProperty] private DateTime startCreatedAtDate = DateTime.Today;
    [ObservableProperty] private DateTime finishedCreatedAtDate = DateTime.Today;
    [ObservableProperty] private DateTime startFinishedAtDate = DateTime.Today;
    [ObservableProperty] private DateTime finesedFinishedAtDate = DateTime.Today;

    // flags se o usuário quer filtrar por data
    [ObservableProperty] private bool useCreatedAtRange;
    [ObservableProperty] private bool useFinishedAtRange;

}
