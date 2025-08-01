// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// Keep setters to allow JSON deserialization

using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FileExtensionGroup(
    string name,
    HashSet<FileExtension> fileExtensions,
    bool isActive = true
)
{
    public string Name { get; set; } = name;
    public bool IsActive { get; set; } = isActive;
    public HashSet<FileExtension> FileExtensions { get; set; } = fileExtensions;
}
