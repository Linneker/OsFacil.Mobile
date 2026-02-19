namespace OsFacil.Mobile.Service.Https.Login;

public class RegisterTenantHttpRequest(
    string companyName,
    string companySlug,
    string adminName,
    string adminEmail,
    string adminPassword)
{
    public string CompanyName { get; } = companyName;
    public string CompanySlug { get; } = companySlug;
    public string AdminName { get; } = adminName;
    public string AdminEmail { get; } = adminEmail;
    public string AdminPassword { get; } = adminPassword;
}
