using System.Text.Json.Serialization;
using OsFacil.Mobile.Api.Models;

namespace OsFacil.Mobile.Api.Data;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(SeedData))]
[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(List<Project>))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(List<ProjectTask>))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(List<Tag>))]
public partial class JsonContext : JsonSerializerContext
{
}
