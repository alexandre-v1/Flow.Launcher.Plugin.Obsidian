using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class VaultSetting
{
    public bool IsActive { get; set; } = true;

    [JsonInclude]
    public bool UseNoteProperties { get; set; } = true;

    public bool OpenInNewTabByDefault { get; set; }

    [JsonInclude]
    public FileExtensionsSetting FileExtensions { get; set; } = new();

    [JsonInclude]
    public IList<string> RelativeExcludePaths { get; set; } = [".obsidian", ".trash"];
}
