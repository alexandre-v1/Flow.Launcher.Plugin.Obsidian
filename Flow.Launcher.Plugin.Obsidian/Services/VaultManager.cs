using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class VaultManager
{
    public bool HasOnlyOneVault => Vaults.Count is 1;
    public bool OneVaultHasAdvancedUri => Vaults.Any(vault => vault.HasAdvancedUri);
    public bool AllVaultsHaveAdvancedUri => Vaults.All(vault => vault.HasAdvancedUri);
    public HashSet<string> TagsList => new(_tagsList.Keys);

    public List<Vault> Vaults { get; private set; } = new();
    private readonly ConcurrentDictionary<string, byte> _tagsList = new();
    public readonly Settings Settings;

    public VaultManager(Settings settings)
    {
        Settings = settings;
    }

    public void UpdateVaultList(Settings? settings)
    {
        Vaults = new List<Vault>();
        string jsonString = System.IO.File.ReadAllText(Paths.VaultListJsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);

        JsonElement root = document.RootElement;
        JsonElement vaults = root.GetProperty("vaults");


        foreach (JsonProperty vault in vaults.EnumerateObject())
        {
            string vaultId = vault.Name;
            string? path = vault.Value.GetProperty("path").GetString();
            if (path is null) continue;

            Settings.VaultsSetting.TryGetValue(vaultId, out VaultSetting? vaultSetting);
            if (vaultSetting is null)
            {
                vaultSetting = new VaultSetting();
                Settings.VaultsSetting.Add(vaultId, vaultSetting);
            }

            Vaults.Add(new Vault(vaultId, path, vaultSetting, this));
        }

        if (!OneVaultHasAdvancedUri)
        {
            Settings.GlobalVaultSetting.OpenInNewTabByDefault = false;
        }
    }

    public List<File> GetAllFiles()
    {
        List<File> files = new();
        foreach (Vault vault in Vaults) files.AddRange(vault.Files);
        return files;
    }

    public List<File> GetAllFilesWithTag(string tag)
    {
        List<File> files = new();
        foreach (Vault vault in Vaults)
        {
            files.AddRange(vault.Files.Where(file => file.HasTag(tag)));
        }

        return files;
    }

    public Vault? GetVaultWithId(string vaultId) => Vaults.Find(vault => vault.Id == vaultId);

    public Vault? GetVaultWithName(string name)
    {
        Vault? vault = Vaults.Find(vault => vault.Name == name);
        return vault;
    }

    public void AddTagsToList(string[] tags)
    {
        foreach (string tag in tags)
        {
            _tagsList.TryAdd(tag, 0);
        }
    }

    public bool IsAnExistingTag(string tagToCheck) => TagsList.Any(tag => tag.IsSameString(tagToCheck));

    public string? GetExistingTag(string tagToGet)
    {
        return TagsList.FirstOrDefault(tag => tag.IsSameString(tagToGet));
    }
}
