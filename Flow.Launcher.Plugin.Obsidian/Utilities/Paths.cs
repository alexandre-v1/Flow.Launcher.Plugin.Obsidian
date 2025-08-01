using System;
using System.IO;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class Paths
{
    public static readonly string ObsidianLogo = Path.Combine("Icons", "Core", "obsidian-logo.png");

    public static readonly string VaultListJsonPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "obsidian",
        "obsidian.json"
    );

    public static string GetCommunityPluginsJsonPath(string vaultPath) =>
        Path.Combine(vaultPath, ".obsidian", "community-plugins.json");
}
