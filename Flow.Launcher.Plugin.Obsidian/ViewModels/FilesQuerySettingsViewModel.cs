using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Views;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class FilesQuerySettingsViewModel : BaseModel
{
    public delegate void RequestCloseEventHandler();

    private readonly IQueryService _queryService;
    private readonly FilesQuerySetting _setting;
    private readonly ISettingsWindowManager _windowManager;

    public FilesQuerySettingsViewModel(FilesQuerySetting setting, IQueryService queryService,
        IVaultManager vaultManager, ISettingsWindowManager windowManager)
    {
        _setting = setting;
        _windowManager = windowManager;
        _queryService = queryService;

        Vaults = CreateVaultViewModels(vaultManager.GetVaults()).ToHashSet();

        FileExtensionListViewModel = new FileExtensionsListViewModel(_setting.FileExtensions);
        ExcludePathsViewModel = new ExcludePathsViewModel(_setting.RelativeExcludePaths);
    }

    [UsedImplicitly] // For design-time data
    public FilesQuerySettingsViewModel()
    {
        _windowManager = null!;
        _queryService = null!;

        Vaults =
        [
            new FilesQueryVaultViewModel
            {
                DesignName = "Sample Vault 1", DesignPath = @"C:\Vaults\Sample1", IsActive = true
            },
            new FilesQueryVaultViewModel
            {
                DesignName = "Sample Vault 2", DesignPath = @"C:\Vaults\Sample2", IsActive = false
            },
            new FilesQueryVaultViewModel
            {
                DesignName = "Sample Vault 3", DesignPath = @"C:\Vaults\Sample3", IsActive = true
            }
        ];
        _setting = new FilesQuerySetting();
        FileExtensionListViewModel = new FileExtensionsListViewModel();
        ExcludePathsViewModel = new ExcludePathsViewModel();
    }

    public FileExtensionsListViewModel FileExtensionListViewModel { get; }
    public ExcludePathsViewModel ExcludePathsViewModel { get; }

    public string Name
    {
        get => _setting.Name;
        set
        {
            _setting.Name = value;
            OnPropertyChanged();
        }
    }

    public string Keyword
    {
        get => _setting.Keyword;
        set
        {
            _setting.Keyword = value;
            OnPropertyChanged();
        }
    }

    public HashSet<FilesQueryVaultViewModel> Vaults { get; }
    public event RequestCloseEventHandler? RequestClose;

    private IEnumerable<FilesQueryVaultViewModel> CreateVaultViewModels(IEnumerable<Vault> vaults) => vaults
        .Select(vault => new FilesQueryVaultViewModel(_setting, vault, _windowManager));

    [RelayCommand]
    private void ReloadQuery() => _queryService.ReloadQuery(_setting);

    [RelayCommand]
    private void SetActionKeyword()
    {
        ActionKeywordDialog dialog = new(_setting, _queryService);
        dialog.ShowDialog();
        OnPropertyChanged(nameof(Keyword));
    }

    [RelayCommand]
    private void DeleteQuery()
    {
        _queryService.DeleteQuery(_setting);
        RequestClose?.Invoke();
    }
}
