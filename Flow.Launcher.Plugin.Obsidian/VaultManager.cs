using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Flow.Launcher.Plugin.Obsidian;

public static class VaultManager
{
    //Todo : Check the Path before use
    private static readonly string VaultListJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obsidian", "obsidian.json");

    public static List<Vault> Vaults { get; set; } = new();
    
    public static void UpdateVaultList(IPublicAPI publicApi)
    {
        Vaults = GetVault();
        foreach (Vault vault in Vaults)
        {
            if (vault.VaultPath != null) 
                vault.Files = Vault.GetMdFiles(vault.VaultPath);
        }
    }

    private static List<Vault> GetVault()
    {
        string jsonString = File.ReadAllText(VaultListJsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);
        
        JsonElement root = document.RootElement;
        JsonElement vaults = root.GetProperty("vaults");
        
        foreach (JsonProperty vault in vaults.EnumerateObject())
        {
            string vaultId = vault.Name;
            string? path = vault.Value.GetProperty("path").GetString();
            
            Vaults.Add(new Vault(vaultId, path));
        }
        return Vaults;
    }
}
