using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class SettingsViewModel : BaseModel
{
    // For design-time data
    public SettingsViewModel() => VaultsListViewModel = new VaultsListViewModel();

    public SettingsViewModel(
        IAsyncReloadable reloadablePlugin,
        IVaultManager vaultManager,
        ISettingWindowManager settingWindowManager
    )
    {
        ReloadablePlugin = reloadablePlugin;
        VaultsListViewModel = new VaultsListViewModel(vaultManager, settingWindowManager);
    }

    public VaultsListViewModel VaultsListViewModel { get; }
    private IAsyncReloadable? ReloadablePlugin { get; }

    public void OnUnloaded() => _ = ReloadPluginDataAsync();

    public Task? ReloadPluginDataAsync() => ReloadablePlugin?.ReloadDataAsync();
}
