using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Services;

public sealed class ToastService : IToastService
{
    public async Task ShowAsync(string message, CancellationToken ct = default)
    {
        // garante execução na UI thread
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var toast = Toast.Make(message, ToastDuration.Long);
            await toast.Show(ct);
        });
    }
}
