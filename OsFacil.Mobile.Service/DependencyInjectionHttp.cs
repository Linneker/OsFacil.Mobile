using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OsFacil.Mobile.Service.Https.Clients;
using OsFacil.Mobile.Service.Https.Dashboard;
using OsFacil.Mobile.Service.Https.Login;
using OsFacil.Mobile.Service.Https.Billing;
using OsFacil.Mobile.Service.Https.Workorders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Service;

public static class DependencyInjectionHttp
{
    public static IServiceCollection AddHttpServices(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration.GetSection("UrlOsFacilApi")?.Value ?? "http://192.168.1.100:56901";
        services.AddHttpClient<ILoginHttp, LoginHttp>(client =>
        {
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(3600);

        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AllowAutoRedirect = false
        });

        services.AddHttpClient<IClientHttp, ClientHttp>(client =>
        {
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(3600);

        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AllowAutoRedirect = false
        });

        services.AddHttpClient<IMetricsHttp, MetricsHttp>(client =>
        {
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<IWorkspaceHttp, WorkspaceHttp>(client =>
        {
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<IBillingHttp, BillingHttp>(client =>
        {
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
