using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Services;

public interface IToastService
{
    Task ShowAsync(string message, CancellationToken ct = default);
}
