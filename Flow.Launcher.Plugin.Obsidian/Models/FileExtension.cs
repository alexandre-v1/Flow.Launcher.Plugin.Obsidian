// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// Keep setters to allow JSON deserialization

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FileExtension(
    string name,
    string suffix,
    bool isActive = true,
    ObsidianPlugin? pluginNeeded = null
)
{
    public string Name { get; set; } = name;

    public string Suffix { get; set; } = suffix;

    // This has no use when inside a group
    public bool IsActive { get; set; } = isActive;

    public ObsidianPlugin? PluginNeeded { get; set; } = pluginNeeded;
}
