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

    public Vault(string id, string vaultPath, VaultSetting vaultSetting, Settings settings)
    {
        Id = id;
        VaultPath = vaultPath;
        VaultSetting = vaultSetting;
        Name = Path.GetFileName(VaultPath);
        Files = GetFiles(settings).ToList();
    }

    private IEnumerable<File> GetFiles(Settings settings)
    {
        bool useExtensions = settings.UseFilesExtension;
        bool useAliases = settings.UseAliases;
        
        var extensions = VaultSetting.GetSearchableExtensions(settings);
        var excludedPaths = VaultSetting.GetExcludedPaths(settings)
            .Select(excludedPath => Path.Combine(VaultPath, excludedPath))
            .ToList();

        var files = Directory.EnumerateFiles(VaultPath, "*", SearchOption.AllDirectories)
            .Where(file => extensions.Contains(Path.GetExtension(file)) 
                           && !excludedPaths.Any(file.StartsWith))
            .Select(delegate(string filePath)
            {
                string[]? aliases = null;
                if (useAliases)
                    aliases = AliasesService.GetAliases(filePath);
                return new File(this, filePath, useExtensions, aliases);
            });

        return files;
    }
}
