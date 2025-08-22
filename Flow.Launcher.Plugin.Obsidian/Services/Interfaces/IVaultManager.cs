using System.Collections.Generic;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface IVaultManager
{
    IEnumerable<Vault> GetVaults();

    IEnumerable<Vault> GetActiveVaults();

    Task ReloadVaultsAsync();

    Task UpdateVaultAsync(Vault vault);

    Vault? GetVaultWithId(string vaultId);

    IEnumerable<Vault> GetVaultsWithIds(IEnumerable<string> vaultIds);
}
