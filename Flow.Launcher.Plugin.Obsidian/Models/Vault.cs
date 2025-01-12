using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Services;
using YamlDotNet.Serialization;

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
        Files = GetFiles(settings);
    }

    private List<File> GetFiles(Settings settings)
    {
        bool useAliases = settings.UseAliases;

        var extensions = VaultSetting.GetSearchableExtensions(settings);
        var excludedPaths = VaultSetting.GetExcludedPaths(settings)
            .Select(excludedPath => Path.Combine(VaultPath, excludedPath))
            .ToList();

        var files = Directory.EnumerateFiles(VaultPath, "*", SearchOption.AllDirectories)
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Where(file => extensions.Contains(Path.GetExtension(file)) 
                           && !excludedPaths.Any(file.StartsWith))
            .Select(filePath =>
            {
                string[]? aliases = null;
                if (!useAliases) return new File(this, filePath, aliases);
                var deserializer = new Deserializer();
                aliases = AliasesService.GetAliases(filePath, deserializer);
                return new File(this, filePath, aliases);
            })
            .ToList();

        return files;
    }
}
