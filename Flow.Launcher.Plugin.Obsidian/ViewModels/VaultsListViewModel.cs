using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class VaultsListViewModel : BaseModel
{
    private readonly ISettingWindowManager _settingWindowManager;
    private readonly IVaultManager _vaultManager;

    public VaultsListViewModel(IEnumerable<Vault> vaults, ISettingWindowManager settingWindowManager,
        IVaultManager vaultManager)
    {
        _settingWindowManager = settingWindowManager;
        _vaultManager = vaultManager;
        Vaults = CreateVaultViewModels(vaults);
    }

    [UsedImplicitly] // For design-time data
    public VaultsListViewModel()
    {
        _settingWindowManager = null!;
        _vaultManager = null!;
        Vaults =
        [
            new VaultViewModel { DesignName = "Sample Vault 1", DesignPath = @"C:\Vaults\Sample1", IsActive = true },
            new VaultViewModel { DesignName = "Sample Vault 2", DesignPath = @"C:\Vaults\Sample2", IsActive = false },
            new VaultViewModel { DesignName = "Sample Vault 3", DesignPath = @"C:\Vaults\Sample3", IsActive = true }
        ];
    }

    public HashSet<VaultViewModel> Vaults { get; }

    private HashSet<VaultViewModel> CreateVaultViewModels(IEnumerable<Vault> vaults) => vaults
        .Select(vault => new VaultViewModel(vault, _settingWindowManager, _vaultManager)).ToHashSet();
}
