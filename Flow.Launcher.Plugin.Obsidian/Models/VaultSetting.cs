using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class VaultSetting : GlobalVaultSetting
{
    public bool UseGlobalSetting { get; set; } = true;
    public bool UseGlobalExcludedPaths { get; set; } = true;

    public override HashSet<string> GetSearchableExtensions(Settings settings)
    {
        return UseGlobalSetting ? 
            settings.GlobalVaultSetting.GetSearchableExtensions(settings) : base.GetSearchableExtensions(settings);
    }

    public override List<string> GetExcludedPaths(Settings settings)
    {
        var excludedPaths = new List<string>();
        if (UseGlobalExcludedPaths)
            excludedPaths.AddRange(settings.GlobalVaultSetting.GetExcludedPaths(settings));
        excludedPaths.AddRange(base.GetExcludedPaths(settings));
        return excludedPaths;
    }
}
