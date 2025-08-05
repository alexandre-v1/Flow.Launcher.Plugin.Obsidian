using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class VaultSettingsViewModel : BaseModel
{
    private readonly Vault? _vault;
    private readonly IVaultManager? _vaultManager;

    public VaultSettingsViewModel()
    {
        FileExtensionListViewModel = new FileExtensionsListViewModel();
        ExcludePathsViewModel = new ExcludePathsViewModel();
    }

    public VaultSettingsViewModel(Vault vault, IVaultManager vaultManager)
    {
        _vault = vault;
        _vaultManager = vaultManager;

        VaultSetting setting = vault.Setting;
        FileExtensionListViewModel = new FileExtensionsListViewModel(setting.FileExtensions);
        ExcludePathsViewModel = new ExcludePathsViewModel(setting.RelativeExcludePaths);

        vault.VaultUpdated += OnVaultUpdated;
    }

    public FileExtensionsListViewModel FileExtensionListViewModel { get; }
    public ExcludePathsViewModel ExcludePathsViewModel { get; }

    public string Id => _vault?.Id ?? "Vault Id";
    public string Name => _vault?.Name ?? "Vault Name";
    public string Path => _vault?.Path ?? @"C:\Vaults\Vault Name";
    public string FilesCount => _vault is not null ? $"{_vault.FilesCount} Files" : "Files count";

    private void OnVaultUpdated()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Path));
        OnPropertyChanged(nameof(FilesCount));
    }

    [RelayCommand]
    private async Task ReloadVaultAsync()
    {
        if (_vault is null || _vaultManager is null)
        {
            return;
        }

        await _vaultManager.UpdateVaultAsync(_vault);
    }
}
