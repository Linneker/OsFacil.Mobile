using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsFacil.Mobile.Service.Https.Clients;
using OsFacil.Mobile.Service.Https.Dashboard;
using OsFacil.Mobile.Service.Https.Login;
using OsFacil.Mobile.Service.Https.Billing;
using OsFacil.Mobile.Service.Https.Workorders;
using System;

namespace OsFacil.Mobile.Service;

public static class DependencyInjectionHttp
{
    public static IServiceCollection AddHttpServices(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["UrlOsFacilApi"];
        if (string.IsNullOrWhiteSpace(url))
            url = "http://192.168.1.100:56901";

        url = url.Trim();
        if (!url.EndsWith("/", StringComparison.Ordinal))
            url += "/";

        if (!Uri.TryCreate(url, UriKind.Absolute, out var baseAddress))
            throw new InvalidOperationException("A configuração 'UrlOsFacilApi' não é uma URL absoluta válida.");

        services.AddHttpClient<ILoginHttp, LoginHttp>(client =>
        {
            client.BaseAddress = baseAddress;
            client.Timeout = TimeSpan.FromSeconds(3600);

        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AllowAutoRedirect = false
        });

        services.AddHttpClient<IClientHttp, ClientHttp>(client =>
        {
            client.BaseAddress = baseAddress;
            client.Timeout = TimeSpan.FromSeconds(3600);

        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AllowAutoRedirect = false
        });

        services.AddHttpClient<IMetricsHttp, MetricsHttp>(client =>
        {
            client.BaseAddress = baseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<IWorkspaceHttp, WorkspaceHttp>(client =>
        {
            client.BaseAddress = baseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<IBillingHttp, BillingHttp>(client =>
        {
            client.BaseAddress = baseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
