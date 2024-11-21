using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Settings
{
    public int MaxResult { get; set; }
    public bool OldLogos { get; set; }
    public bool ShowFilesExtension { get; set; }
    
    // Keep setters to allow JSON deserialization
    public GlobalVaultSetting GlobalVaultSetting { get; set; } = new();
    public Dictionary<string, VaultSetting> VaultsSetting { get; set; } = new();
}
