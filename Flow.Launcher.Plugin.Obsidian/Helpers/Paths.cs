using System.IO;

namespace Flow.Launcher.Plugin.Obsidian.Helpers;

public static class Paths
{
    public static readonly string ObsidianLogo = Path.Combine("Icons", "obsidian-logo.png");

    public static string GetCommunityPluginsJsonPath(string vaultPath)
    {
        return Path.Combine(vaultPath, ".obsidian", "community-plugins.json");
    }
}
