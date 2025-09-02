using System.Diagnostics;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.Views;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class VaultViewModel : BaseModel
{
    private readonly ISettingWindowManager _settingWindowManager;
    private readonly Vault? _vault;

    private bool _isActive;

    public string DesignName = "Vault Name";
    public string DesignPath = "Vault Path";

    public VaultViewModel(Vault vault, ISettingWindowManager settingWindowManager)
    {
        _settingWindowManager = settingWindowManager;
        _vault = vault;
        _isActive = vault.IsActive;

        vault.VaultUpdated += OnVaultUpdated;
    }

    [UsedImplicitly] // For design-time data
    public VaultViewModel() => _settingWindowManager = null!;

    public static ImageSource Icon => IconCache.GetCachedImage(Paths.ObsidianLogo);
    public string Name => _vault?.Name ?? DesignName;
    public string VaultPath => _vault?.Path ?? DesignPath;
    public int FilesCount => _vault?.Files.Count ?? 0;

    public bool IsActive
    {
        get => _vault?.IsActive ?? _isActive;
        set
        {
            if (_vault is not null)
            {
                _vault.IsActive = value;
            }
            else
            {
                _isActive = value;
            }

            OnPropertyChanged();
        }
    }

    private void OnVaultUpdated()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(VaultPath));
        OnPropertyChanged(nameof(FilesCount));
    }

    [RelayCommand]
    private void OpenVaultSettings()
    {
        if (_vault is null)
        {
            Debug.WriteLine("Vault is null, can't open vault window");
            return;
        }

        VaultSettingsViewModel vaultSettingsViewModel = new(_vault);
        _settingWindowManager.ShowView<VaultSettingsView>(vaultSettingsViewModel);
    }
}
