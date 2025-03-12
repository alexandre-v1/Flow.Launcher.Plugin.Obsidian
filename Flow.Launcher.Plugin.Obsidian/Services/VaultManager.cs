using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Flow.Launcher.Plugin.Obsidian.Models;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class VaultManager
{
    public static bool HasOnlyOneVault => Vaults.Count == 1;
    public static bool OneVaultHasAdvancedUri => Vaults.Any(vault => vault.HasAdvancedUri);
    public static bool AllVaultsHaveAdvancedUri => Vaults.All(vault => vault.HasAdvancedUri);
    public static HashSet<string> TagsList => new(_tagsList.Keys);

    public static List<Vault> Vaults { get; private set; } = new();
    private static readonly ConcurrentDictionary<string, byte> _tagsList = new();

    private static readonly string VaultListJsonPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obsidian", "obsidian.json");

    public static void UpdateVaultList(Settings settings)
    {
        Vaults = new List<Vault>();
        string jsonString = System.IO.File.ReadAllText(VaultListJsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);

        JsonElement root = document.RootElement;
        JsonElement vaults = root.GetProperty("vaults");


        foreach (JsonProperty vault in vaults.EnumerateObject())
        {
            string vaultId = vault.Name;
            string? path = vault.Value.GetProperty("path").GetString();
            if (path is null) continue;

            settings.VaultsSetting.TryGetValue(vaultId, out VaultSetting? vaultSetting);
            if (vaultSetting is null)
            {
                vaultSetting = new VaultSetting();
                settings.VaultsSetting.Add(vaultId, vaultSetting);
            }

            Vaults.Add(new Vault(vaultId, path, vaultSetting, settings));
        }

        if (!OneVaultHasAdvancedUri)
        {
            settings.GlobalVaultSetting.OpenInNewTabByDefault = false;
        }
    }

    public static List<File> GetAllFiles()
    {
        List<File> files = new();
        foreach (Vault vault in Vaults) files.AddRange(vault.Files);
        return files;
    }

    public static List<File> GetAllFilesWithTag(string lowerTag)
    {
        List<File> files = new();
        foreach (Vault vault in Vaults)
        {
            files.AddRange(vault.Files.Where(file => file.HasTag(lowerTag)));
        }

        return files;
    }

    public static Vault? GetVaultWithId(string vaultId) => Vaults.Find(vault => vault.Id == vaultId);

    public static Vault? GetVaultWithName(string name)
    {
        Vault? vault = Vaults.Find(vault => vault.Name == name);
        return vault;
    }

    public static void AddTagsToList(List<string> tags)
    {
        foreach (string tag in tags)
        {
            _tagsList.TryAdd(tag, 0);
        }
    }
}
