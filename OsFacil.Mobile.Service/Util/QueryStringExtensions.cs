using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Util;

public static class QueryStringExtensions
{
    public static string AddQueryString<T>(this string uri, T query)
    {
        var dict = new Dictionary<string, string?>();

        foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var value = prop.GetValue(query);
            if (value is null) continue;

            var name =
                prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                ?? ToCamelCase(prop.Name);

            var stringValue = value switch
            {
                DateTime dt => dt.ToString("O", CultureInfo.InvariantCulture),
                DateTimeOffset dto => dto.ToString("O", CultureInfo.InvariantCulture),
                decimal d => d.ToString(CultureInfo.InvariantCulture),
                double d => d.ToString(CultureInfo.InvariantCulture),
                float f => f.ToString(CultureInfo.InvariantCulture),
                bool b => b ? "true" : "false",
                Enum e => Convert.ToInt64(e, CultureInfo.InvariantCulture)
                                        .ToString(CultureInfo.InvariantCulture),
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? ""
            };

            dict[name] = stringValue;
        }

        return QueryHelpers.AddQueryString(uri, dict);
    }

    private static string ToCamelCase(string s)
        => string.IsNullOrEmpty(s) ? s : char.ToLowerInvariant(s[0]) + s[1..];
}