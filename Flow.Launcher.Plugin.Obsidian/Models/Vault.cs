using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Vault
{
    public readonly string Id;
    public readonly string Name;
    public readonly string VaultPath;

    public readonly VaultSetting VaultSetting;
    public IEnumerable<File> Files = System.Array.Empty<File>();

    public Vault(string id, string vaultPath, VaultSetting vaultSetting)
    {
        Id = id;
        VaultPath = vaultPath;
        VaultSetting = vaultSetting;
        Name = Path.GetFileName(VaultPath);
    }

    public IEnumerable<File> GetFiles(Settings settings)
    {
        bool useExtensions = settings.UseFilesExtension;
        var extensions = VaultSetting.GetSearchableExtensions(settings);
        var excludedPaths = VaultSetting.GetExcludedPaths(settings)
            .Select(excludedPath => Path.Combine(VaultPath, excludedPath))
            .ToList();

        var files = Directory.EnumerateFiles(VaultPath, "*", SearchOption.AllDirectories)
            .Where(file => extensions.Contains(Path.GetExtension(file)) 
                           && !excludedPaths.Any(file.StartsWith))
            .Select(filePath => new File(this, filePath, useExtensions));

        return files;
    }
}
