using OsFacil.Mobile.Service.Https.Dashboard.Response;
using OsFacil.Mobile.Service.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Service.Https.Dashboard;

public interface IMetricsHttp
{
    Task<ResponseHttps<DashboardMetricsHttpResponse>> GetDashboardAsync(string accessToken, CancellationToken ct = default);
}
