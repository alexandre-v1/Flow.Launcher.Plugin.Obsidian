using System.Diagnostics;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.Views;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class FilesQueryVaultViewModel : BaseModel
{
    private readonly FilesQuerySetting _querySetting;
    private readonly ISettingWindowManager _settingWindowManager;
    private readonly Vault? _vault;

    public string DesignName = "Vault Name";
    public string DesignPath = "Vault Path";

    public FilesQueryVaultViewModel(FilesQuerySetting querySetting, Vault vault,
        ISettingWindowManager settingWindowManager)
    {
        _querySetting = querySetting;
        _settingWindowManager = settingWindowManager;
        _vault = vault;

        vault.VaultUpdated += OnVaultUpdated;
    }

    [UsedImplicitly] // For design-time data
    public FilesQueryVaultViewModel()
    {
        _querySetting = new FilesQuerySetting();
        _settingWindowManager = null!;
    }

    private string VaultId => _vault?.Id ?? string.Empty;

    public static ImageSource Icon => IconCache.GetCachedImage(Paths.ObsidianLogo);
    public string Name => _vault?.Name ?? DesignName;
    public string VaultPath => _vault?.Path ?? DesignPath;
    public int FilesCount => _vault?.Files.Count ?? 0;

    public bool IsActive
    {
        get => _querySetting.VaultIsActive(VaultId);
        set
        {
            _querySetting.SetVaultActiveState(VaultId, value);
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
