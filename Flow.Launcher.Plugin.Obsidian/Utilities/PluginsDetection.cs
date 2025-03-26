using System.IO;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class PluginsDetection
{
    private const string AdvancedUriPluginName = "obsidian-advanced-uri";

    public static bool IsObsidianAdvancedUriPluginInstalled(string vaultPath)
    {
        string pluginsJsonPath = Paths.GetCommunityPluginsJsonPath(vaultPath);
        if (!File.Exists(pluginsJsonPath)) return false;

        string json = File.ReadAllText(pluginsJsonPath);
        return json.Contains(AdvancedUriPluginName);
    }
}
