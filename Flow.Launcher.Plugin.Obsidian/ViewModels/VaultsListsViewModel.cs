using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class VaultsListsViewModel : BaseModel
{
    public HashSet<VaultViewModel> Vaults { get; }
    private readonly VaultManager? _vaultManager;

    public VaultsListsViewModel()
    {
        Vaults = LoadDesignTimeVaults();
    }

    public VaultsListsViewModel(VaultManager vaultManager)
    {
        _vaultManager = vaultManager;
        Vaults = LoadVaults() ?? [];
    }

    private static HashSet<VaultViewModel> LoadDesignTimeVaults() =>
    [
        new() { DesignName = "Sample Vault 1", DesignPath = @"C:\Vaults\Sample1", IsActive = true },
        new() { DesignName = "Sample Vault 2", DesignPath = @"C:\Vaults\Sample2", IsActive = false },
        new() { DesignName = "Sample Vault 3", DesignPath = @"C:\Vaults\Sample3", IsActive = true }
    ];

    private HashSet<VaultViewModel>? LoadVaults() =>
        _vaultManager?.Vaults.Select(vault => new VaultViewModel(vault)).ToHashSet();
}
