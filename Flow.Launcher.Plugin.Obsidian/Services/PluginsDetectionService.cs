using System.IO;
using Flow.Launcher.Plugin.Obsidian.Helpers;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class PluginsDetectionService
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
