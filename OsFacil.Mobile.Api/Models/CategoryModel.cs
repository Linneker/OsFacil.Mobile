using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Models;

public partial class CategoryModel: ObservableObject
{
    [ObservableProperty]
    private string name;
    [ObservableProperty]
    private string description;
    [ObservableProperty]
    private List<CompomentsModel> components;

}
