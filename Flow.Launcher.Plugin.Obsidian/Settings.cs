using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian;

public class Settings
{
    public int MaxResult { get; set; } = 0;
    public bool OldLogos { get; set; } = false;
    public bool ShowFilesExtension { get; set; } = false;
    
    public GlobalVaultSetting GlobalVaultSetting { get; set; } = new();
    public List<VaultSetting> VaultsSetting { get; set; } = new();
}
