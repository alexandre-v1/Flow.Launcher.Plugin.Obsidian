using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class QuerySetting
{
    [JsonInclude]
    public FileExtensionsSetting FileExtensions { get; set; } = new();

    [JsonInclude]
    public int MaxResult { get; set; }

    public bool UseAliases { get; set; } = true;

    [JsonInclude]
    public bool UseTags { get; set; } = true;

    [JsonInclude]
    public bool AddCheckBoxesToContext { get; set; } = true;

    [JsonInclude]
    public bool AddGlobalFolderExcludeToContext { get; set; } = true;

    [JsonInclude]
    public bool AddLocalFolderExcludeToContext { get; set; }

    [JsonInclude]
    public bool AddCreateNoteOptionOnAllSearch { get; set; } = true;
}
