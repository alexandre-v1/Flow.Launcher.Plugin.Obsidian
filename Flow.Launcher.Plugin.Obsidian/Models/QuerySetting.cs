// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// Keep setters to allow JSON deserialization

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class QuerySetting
{
    public int MaxResult { get; set; }
    public bool UseAliases { get; set; } = true;
    public bool UseTags { get; set; } = true;
    public bool AddCheckBoxesToContext { get; set; } = true;
    public bool AddGlobalFolderExcludeToContext { get; set; } = true;
    public bool AddLocalFolderExcludeToContext { get; set; }
    public bool AddCreateNoteOptionOnAllSearch { get; set; } = true;
}
