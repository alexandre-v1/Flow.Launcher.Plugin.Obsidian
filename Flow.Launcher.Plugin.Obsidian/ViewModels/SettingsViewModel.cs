using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class SettingsViewModel : BaseModel
{
    [UsedImplicitly] // For design-time data
    public SettingsViewModel()
    {
        VaultsListViewModel = new VaultsListViewModel();
        QueriesListViewModel = new QueriesListViewModel();
    }

    public SettingsViewModel(Settings settings, IAsyncReloadable reloadablePlugin, IVaultManager vaultManager,
        ISettingWindowManager settingWindowManager, IQueryService queryService)
    {
        ReloadablePlugin = reloadablePlugin;
        VaultsListViewModel = new VaultsListViewModel(vaultManager.GetVaults(), settingWindowManager);
        QueriesListViewModel =
            new QueriesListViewModel(settings.Queries, settingWindowManager, vaultManager, queryService);
    }

    public VaultsListViewModel VaultsListViewModel { get; }
    public QueriesListViewModel QueriesListViewModel { get; }
    private IAsyncReloadable? ReloadablePlugin { get; }

    public void OnUnloaded() => _ = ReloadPluginDataAsync();

    public Task? ReloadPluginDataAsync() => ReloadablePlugin?.ReloadDataAsync();
}
