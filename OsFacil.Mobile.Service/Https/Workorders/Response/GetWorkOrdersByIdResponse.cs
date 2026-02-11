using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Workorders.Response;

public class GetWorkOrdersByIdResponse: IResponseHttp
{
    [JsonPropertyName("workOrder")]
    public WorkorderResponse WorkOrder { get; set; } = new();

    [JsonPropertyName("photos")]
    public List<PhotoResponse> Photos { get; set; } = [];
}
