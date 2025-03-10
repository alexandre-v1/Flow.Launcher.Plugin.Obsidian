using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Settings
{
    public int MaxResult { get; set; }
    public bool UseAliases { get; set; } = true;
    public bool UseFilesExtension { get; set; }
    public bool AddCheckBoxesToContext { get; set; } = true;
    public bool AddGlobalFolderExcludeToContext { get; set; } = true;
    public bool AddLocalFolderExcludeToContext { get; set; }
    public bool AddCreateNoteOptionOnAllSearch { get; set; } = true;

    // Keep setters to allow JSON deserialization
    public GlobalVaultSetting GlobalVaultSetting { get; set; } = new();
    public Dictionary<string, VaultSetting> VaultsSetting { get; set; } = new();
}
