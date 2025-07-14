using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Vault : BaseModel
{
    public readonly string Id;
    public readonly string Name;
    public ImageSource Icon { get; }
    public readonly string VaultPath;
    public readonly VaultSetting VaultSetting;
    public HashSet<string> Tags { get; } = [];
    public bool HasAdvancedUri { get; }
    public List<File> Files { get; private set; } = [];

    private readonly VaultManager _vaultManager;

    public Vault(string id, string vaultPath, VaultSetting vaultSetting, VaultManager vaultManager)
    {
        Id = id;
        VaultPath = vaultPath;
        VaultSetting = vaultSetting;
        Name = Path.GetFileName(VaultPath);
        _vaultManager = vaultManager;
        HasAdvancedUri = PluginsDetection.IsObsidianAdvancedUriPluginInstalled(VaultPath);
        if (!HasAdvancedUri)
        {
            VaultSetting.OpenInNewTabByDefault = false;
        }

        IsActive = true;
        Icon = IconCache.GetCachedImage(Paths.ObsidianLogo);
    }

    public bool OpenInNewTabByDefault()
    {
        if (!HasAdvancedUri) return false;
        return VaultSetting.UseGlobalSetting
            ? _vaultManager.Settings.GlobalVaultSetting.OpenInNewTabByDefault
            : VaultSetting.OpenInNewTabByDefault;
    }

    public async Task LoadFilesAsync() => Files = await GetFilesAsync();

    public bool TagExists(string tag) => Tags.Any(t => t.EqualsIgnoreCase(tag));

    public bool IsVaultName(string vaultName) => Name.EqualsIgnoreCase(vaultName);

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
                .Where(file => extensions.Contains(Path.GetExtension(file)) && !excludedPaths.Any(file.StartsWith))
                .Select(filePath =>
                {
                    File file = new(this, filePath);
                    if (!useObsidianProperties) return file;
                    file = file.AddObsidianProperties(useAliases, useTags);
                    if (file.Tags is not null) Tags.UnionWith(file.Tags);
                    return file;
                })
                .ToList();
        });
    }
}
