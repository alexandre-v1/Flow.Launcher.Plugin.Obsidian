using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class VaultSetting : GlobalVaultSetting
{
    public bool UseGlobalSetting { get; set; }
    public bool UseGlobalExcludedPaths { get; set; }

    public override HashSet<string> GetSearchableExtensions(Settings settings)
    {
        return UseGlobalSetting ? 
            settings.GlobalVaultSetting.GetSearchableExtensions(settings) : base.GetSearchableExtensions(settings);
    }
}
