using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FileExtension(string name, string suffix, bool isActive = true, ObsidianPlugin? pluginNeeded = null)
{
    [JsonInclude]
    public string Name { get; set; } = name;

    [JsonInclude]
    public string Suffix { get; set; } = suffix;

    public bool IsActive { get; set; } = isActive;

    [JsonInclude]
    public ObsidianPlugin? PluginNeeded { get; set; } = pluginNeeded;
}
