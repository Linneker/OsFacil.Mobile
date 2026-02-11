using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Login;

public class LoginHttpRequest(string CompanySlug, string Email, string Password)
{
    //[JsonPropertyName("companySlug")]
    public string CompanySlug { get; } = CompanySlug;
    //[JsonPropertyName("email")]
    public string Email { get; } = Email;

    //[JsonPropertyName("password")]
    public string Password { get; } = Password;
}
