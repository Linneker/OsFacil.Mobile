using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Services.Session;

public interface IAuthSession
{
    string? AccessToken { get; }
    Task SetTokenAsync(string token);
    Task ClearAsync();
    Task LoadAsync();
}
