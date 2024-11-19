using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian;

public class Settings
{
    public int MaxResult { get; set; }
    public bool OldLogos { get; set; }
    public bool ShowFilesExtension { get; set; }
    
    public GlobalVaultSetting GlobalVaultSetting { get; } = new();
    public Dictionary<string, VaultSetting> VaultsSetting { get; } = new();
}
