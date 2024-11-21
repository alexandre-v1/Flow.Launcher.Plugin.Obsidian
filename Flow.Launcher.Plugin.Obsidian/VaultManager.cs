using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Flow.Launcher.Plugin.Obsidian.Models;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian;

public static class VaultManager
{
    private static readonly string VaultListJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obsidian", "obsidian.json");

    public static List<Vault> Vaults { get; private set; } = new();
    
    public static void UpdateVaultList(Settings settings)
    {
        Vaults = GetVaults(settings);
        foreach (Vault vault in Vaults)
        {
            vault.Files = vault.GetMdFiles();
        }
    }

    private static List<Vault> GetVaults(Settings settings)
    {
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
            
            Vaults.Add(new Vault(vaultId, path, vaultSetting));
        }
        return Vaults;
    }

    public static List<File> GetAllFiles()
    {
        var files = new List<File>();
        foreach (Vault vault in Vaults)
        {
            files.AddRange(vault.Files);
        }
        return files;
    }
}
