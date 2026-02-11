using System;
using OsFacil.Mobile.Service.Https.Clients.Response;
using OsFacil.Mobile.Service.Https.Workorders.Request;
using OsFacil.Mobile.Service.Https.Workorders.Response;
using OsFacil.Mobile.Service.Util;

namespace OsFacil.Mobile.Service.Https.Workorders;

public interface IWorkspaceHttp
{
    Task<ResponsesHttps<GetWorkorderHttpResponse>> GetWorkOrdersPaginatedAsync(string token, GetWorkOrdersPaginatedRequest getWorkOrdersPaginatedRequest, CancellationToken ct = default);

    Task<ResponseHttps<GetWorkOrdersByIdResponse>> GetWorkOrdersByIdAsync(string token, Guid id, CancellationToken ct = default);

    Task<ResponseHttps<CreateWorkOrderResponse>> CreateWorkOrderAsync(string token, CreateWorkOrderRequest request, CancellationToken ct = default);

    Task<ResponseHttps<CreateWorkOrderResponse>> UpadateWorkOrderAsync(string token, Guid id, UpdateWorkOrderHttp request, CancellationToken ct = default);

    Task<ResponseHttps<UploadFileResponse>> UploadFileAsync(string token, Guid id, int kind, Stream fileStream, string fileName,
        string contentType,
        Dictionary<string, string>? fields = null, CancellationToken ct = default);

}