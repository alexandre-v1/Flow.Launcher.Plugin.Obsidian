using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

[JsonDerivedType(typeof(FilesQuerySetting), "FilesQuery")]
public class ObsidianQuerySetting
{
    [JsonInclude]
    public string Name { get; set; } = "Query Name";

    [JsonInclude]
    public string Keyword { get; set; } = "*";

    [JsonInclude]
    public bool IsActive { get; set; } = true;
}
