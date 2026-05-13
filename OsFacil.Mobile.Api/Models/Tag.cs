using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Api.Models;

public class Tag
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = "#2563EB";
    public bool IsSelected { get; set; }

    [JsonIgnore]
    public Brush ColorBrush => new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(Color));

    public override string ToString() => Title;
}
