using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Api.Models;

public class Project
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Category? Category { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public List<ProjectTask> Tasks { get; set; } = [];

    [JsonIgnore]
    public bool IsNew => ID == 0;
}
