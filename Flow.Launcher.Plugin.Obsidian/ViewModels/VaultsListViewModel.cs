using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class VaultsListViewModel : BaseModel
{
    private readonly ISettingWindowManager? _settingWindowManager;

    private readonly IVaultManager? _vaultManager;

    public VaultsListViewModel() => Vaults = LoadDesignTimeVaults();

    public VaultsListViewModel(
        IVaultManager vaultManager,
        ISettingWindowManager settingWindowManager
    )
    {
        _vaultManager = vaultManager;
        _settingWindowManager = settingWindowManager;
        Vaults = LoadVaults() ?? [];
    }

    public HashSet<VaultViewModel> Vaults { get; }

    private static HashSet<VaultViewModel> LoadDesignTimeVaults() =>
    [
        new() { DesignName = "Sample Vault 1", DesignPath = @"C:\Vaults\Sample1", IsActive = true },
        new() { DesignName = "Sample Vault 2", DesignPath = @"C:\Vaults\Sample2", IsActive = false },
        new() { DesignName = "Sample Vault 3", DesignPath = @"C:\Vaults\Sample3", IsActive = true }
    ];

    private HashSet<VaultViewModel>? LoadVaults()
    {
        if (_vaultManager is null || _settingWindowManager is null)
        {
            return null;
        }

        return _vaultManager
            ?.Vaults.Select(vault => new VaultViewModel(vault, _settingWindowManager, _vaultManager))
            .ToHashSet();
    }
}
