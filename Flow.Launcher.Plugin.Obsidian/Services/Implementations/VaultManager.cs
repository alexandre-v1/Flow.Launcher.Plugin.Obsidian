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
    private HashSet<Vault> _vaults = [];

    public IEnumerable<Vault> GetVaults() => _vaults;

    public IEnumerable<Vault> GetActiveVaults() => _vaults.Where(vault => vault.IsActive);

    public async Task UpdateVaultAsync(Vault vault)
    {
        vault.Path = await GetVaultPathAsync(vault.Id) ?? vault.Path;
        vault.UpdateVault();
    }

    public async Task ReloadVaultsAsync()
    {
        Dictionary<string, string> vaultsToLoad = await GetVaultsFromJson(Paths.VaultListJsonPath);

        foreach (var vault in _vaults)
        {
            string id = vault.Id;
            bool vaultStillExists = vaultsToLoad.ContainsKey(id);

            if (vaultStillExists)
            {
                vault.Path = vaultsToLoad[id];
                vault.UpdateVault();
                vaultsToLoad.Remove(id);
            }
            else
            {
                _vaults.Remove(vault);
            }
        }

        foreach ((string id, string path) in vaultsToLoad)
        {
            VaultSetting vaultSetting = settings.LoadVaultOrDefault(id);
            Vault newVault = new(id, path, vaultSetting);
            _vaults.Add(newVault);
        }
    }

    public Vault? GetVaultWithId(string vaultId) =>
        _vaults.FirstOrDefault(vault => vault.Id == vaultId);

    public IEnumerable<Vault> GetVaultsWithIds(IEnumerable<string> vaultIds) =>
        vaultIds.Select(GetVaultWithId).OfType<Vault>().ToList();

    private static async Task<Dictionary<string, string>> GetVaultsFromJson(string jsonPath)
    {
        string jsonString = await File.ReadAllTextAsync(jsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);

        const string vaultJsonElement = "vaults";
        JsonElement vaultsJson = document.RootElement.GetProperty(vaultJsonElement);

        Dictionary<string, string> vaults = [];

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

            string id = vaultJson.Name;
            vaults[id] = path;
        }

        return vaults;
    }

    private static async Task<string?> GetVaultPathAsync(string vaultId)
    {
        Dictionary<string, string> vaults = await GetVaultsFromJson(Paths.VaultListJsonPath);
        return vaults[vaultId];
    }
}
