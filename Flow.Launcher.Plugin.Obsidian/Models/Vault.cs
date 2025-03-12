using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Vault
{
    public readonly string Id;
    public readonly string Name;
    public readonly string VaultPath;

    public readonly VaultSetting VaultSetting;
    public List<File> Files { get; private set; }
    public bool HasAdvancedUri { get; set; }

    public Vault(string id, string vaultPath, VaultSetting vaultSetting, Settings settings)
    {
        Id = id;
        VaultPath = vaultPath;
        VaultSetting = vaultSetting;
        Name = Path.GetFileName(VaultPath);
        Files = GetFiles(settings);
        HasAdvancedUri = PluginsDetectionService.IsObsidianAdvancedUriPluginInstalled(VaultPath);
        if (!HasAdvancedUri) VaultSetting.OpenInNewTabByDefault = false;
    }

    public bool OpenNoteInNewTabByDefault(GlobalVaultSetting globalSetting)
    {
        if (!HasAdvancedUri) return false;
        return VaultSetting.UseGlobalSetting ? globalSetting.OpenInNewTabByDefault : VaultSetting.OpenInNewTabByDefault;
    }

    private List<File> GetFiles(Settings settings)
    {
        bool useAliases = settings.UseAliases;
        bool useTags = settings.UseTags;
        bool useObsidianProperties = settings.UseAliases || settings.UseTags;

        HashSet<string> extensions = VaultSetting.GetSearchableExtensions(settings);
        List<string> excludedPaths = VaultSetting.GetExcludedPaths(settings)
            .Select(excludedPath => Path.Combine(VaultPath, excludedPath))
            .ToList();

        List<File> files = Directory.EnumerateFiles(VaultPath, "*", SearchOption.AllDirectories)
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Where(file => extensions.Contains(Path.GetExtension(file))
                           && !excludedPaths.Any(file.StartsWith))
            .Select(filePath =>
                useObsidianProperties
                    ? new File(this, filePath).AddObsidianProperties(useAliases, useTags)
                    : new File(this, filePath))
            .ToList();

        return files;
    }
}
