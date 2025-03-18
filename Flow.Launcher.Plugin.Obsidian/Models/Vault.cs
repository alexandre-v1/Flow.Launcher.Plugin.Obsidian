using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Vault
{
    public readonly string Id;
    public readonly string Name;
    public readonly string VaultPath;
    public readonly VaultSetting VaultSetting;
    public List<File> Files { get; private set; } = [];
    public bool HasAdvancedUri { get; set; }

    private readonly VaultManager _vaultManager;

    public Vault(string id, string vaultPath, VaultSetting vaultSetting, VaultManager vaultManager)
    {
        Id = id;
        VaultPath = vaultPath;
        VaultSetting = vaultSetting;
        Name = Path.GetFileName(VaultPath);
        _vaultManager = vaultManager;
        HasAdvancedUri = PluginsDetection.IsObsidianAdvancedUriPluginInstalled(VaultPath);
        if (!HasAdvancedUri) VaultSetting.OpenInNewTabByDefault = false;
    }

    public bool OpenInNewTabByDefault()
    {
        if (!HasAdvancedUri) return false;
        return VaultSetting.UseGlobalSetting ? _vaultManager.Settings.GlobalVaultSetting.OpenInNewTabByDefault : VaultSetting.OpenInNewTabByDefault;
    }

    public async Task LoadFilesAsync() => Files = await GetFilesAsync();

    private async Task<List<File>> GetFilesAsync()
    {
        Settings settings = _vaultManager.Settings;
        bool useAliases = settings.UseAliases;
        bool useTags = settings.UseTags;
        bool useObsidianProperties = useAliases || useTags;

        HashSet<string> extensions = VaultSetting.GetSearchableExtensions(settings);
        List<string> excludedPaths = VaultSetting.GetExcludedPaths(settings)
            .Select(excludedPath => Path.Combine(VaultPath, excludedPath))
            .ToList();

        return await Task.Run(() =>
        {
            return Directory.EnumerateFiles(VaultPath, "*", SearchOption.AllDirectories)
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Where(file => extensions.Contains(Path.GetExtension(file))
                               && !excludedPaths.Any(file.StartsWith))
                .Select(filePath =>
                {
                    File file = new(this, filePath);
                    if (!useObsidianProperties) return file;
                    file = file.AddObsidianProperties(useAliases, useTags);
                    if (file.Tags is not null) _vaultManager.AddTagsToList(file.Tags);
                    return file;
                })
                .ToList();
        });
    }
}
