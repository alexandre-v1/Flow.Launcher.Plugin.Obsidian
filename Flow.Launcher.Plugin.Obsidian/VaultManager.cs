using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Flow.Launcher.Plugin.Obsidian;

public static class VaultManager
{
    //Todo : Check the Path before use
    private static readonly string VaultListJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obsidian", "obsidian.json");

    private static List<Vault> Vaults { get; set; } = new();
    
    public static void UpdateVaultList()
    {
        Vaults = GetVaults();
        foreach (Vault vault in Vaults)
        {
            vault.Files = vault.GetMdFiles();
        }
    }

    private static List<Vault> GetVaults()
    {
        string jsonString = System.IO.File.ReadAllText(VaultListJsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);
        
        JsonElement root = document.RootElement;
        JsonElement vaults = root.GetProperty("vaults");
        
        foreach (JsonProperty vault in vaults.EnumerateObject())
        {
            string vaultId = vault.Name;
            string? path = vault.Value.GetProperty("path").GetString();

            if (path != null) Vaults.Add(new Vault(vaultId, path));
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
