using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using File = System.IO.File;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class VaultManager(Settings settings) : IVaultManager
{
    public HashSet<Vault> Vaults { get; private set; } = [];

    public async Task<Vault?> GetUpdatedVaultAsync(string vaultId)
    {
        Vault? vault = GetVaultWithId(vaultId);
        if (vault is null)
        {
            return null;
        }

        await UpdateVaultAsync(vault);
        return vault;
    }

    public async Task UpdateVaultAsync(Vault vault)
    {
        vault.Path = await GetVaultPathAsync(vault.Id) ?? vault.Path;
        vault.UpdateVault();
    }

    public async Task UpdateVaultListAsync()
    {
        Vaults = [];
        List<(string id, string path)> vaults = await GetVaultsFromJson(Paths.VaultListJsonPath);

        foreach ((string id, string path) in vaults)
        {
            VaultSetting vaultSetting = settings.LoadVaultOrDefault(id);
            Vault newVault = new(id, path, vaultSetting);
            Vaults.Add(newVault);
        }
    }

    public Vault? GetVaultWithId(string vaultId) =>
        Vaults.FirstOrDefault(vault => vault.Id == vaultId);

    private static async Task<List<(string id, string path)>> GetVaultsFromJson(string jsonPath)
    {
        string jsonString = await File.ReadAllTextAsync(jsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);

        const string vaultJsonElement = "vaults";
        JsonElement vaultsJson = document.RootElement.GetProperty(vaultJsonElement);

        List<(string id, string path)> vaults = [];

        foreach (JsonProperty vaultJson in vaultsJson.EnumerateObject())
        {
            const string pathJsonProperty = "path";
            if (!vaultJson.Value.TryGetProperty(pathJsonProperty, out JsonElement pathElement))
            {
                continue;
            }

            string? path = pathElement.GetString();
            if (string.IsNullOrWhiteSpace(path))
            {
                continue;
            }

            vaults.Add((vaultJson.Name, path));
        }

        return vaults;
    }

    private static async Task<string?> GetVaultPathAsync(string vaultId)
    {
        List<(string id, string path)> vaults = await GetVaultsFromJson(Paths.VaultListJsonPath);

        foreach ((string id, string path) in vaults)
        {
            if (vaultId == id)
            {
                return path;
            }
        }

        return null;
    }
}
