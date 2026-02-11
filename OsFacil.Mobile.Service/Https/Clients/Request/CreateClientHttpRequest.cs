using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Clients.Request;

public class CreateClientHttpRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("phone")] 
    public string? Phone{ get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
