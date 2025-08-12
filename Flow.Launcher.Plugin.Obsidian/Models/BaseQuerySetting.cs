using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

[JsonDerivedType(typeof(FilesQuerySetting), "FilesQuery")]
public abstract class BaseQuerySetting
{
    [JsonInclude]
    public required string Name { get; set; }

    [JsonInclude]
    public required string Keyword { get; set; }
}
