using System.Collections.Generic;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface IVaultManager
{
    HashSet<Vault> Vaults { get; }

    Task<Vault?> GetUpdatedVaultAsync(string vaultId);

    Task UpdateVaultListAsync();

    Task UpdateVaultAsync(Vault vault);

    Vault? GetVaultWithId(string vaultId);
}
