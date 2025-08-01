// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// Keep setters to allow JSON deserialization

using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class VaultSetting
{
    public bool IsActive { get; set; } = true;
    public bool UseNoteProperties { get; set; }
    public bool OpenInNewTabByDefault { get; set; }
    public FileExtensionsSetting FileExtensions { get; set; } = new();
    public IList<string> RelativeExcludePaths { get; set; } = [".obsidian", ".trash"];
}
