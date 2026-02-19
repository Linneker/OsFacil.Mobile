using OsFacil.Mobile.Service.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Service.Https.Login;

public interface ILoginHttp
{
    Task<ResponseHttps<LoginHttpResponse>> LoginAsync(LoginHttpRequest request, CancellationToken ct = default);
    Task<RegisterTenantHttpResponse> RegisterTenantAsync(RegisterTenantHttpRequest request, CancellationToken ct = default);
}
