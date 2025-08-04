using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FileExtensionGroup(string name, HashSet<FileExtension> extensions, bool isActive = true)
{
    [JsonInclude]
    public string Name { get; set; } = name;

    public bool IsActive { get; set; } = isActive;

    [JsonInclude]
    public HashSet<FileExtension> Extensions { get; set; } = extensions;
}
