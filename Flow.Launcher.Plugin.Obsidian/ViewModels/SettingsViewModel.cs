using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class SettingsViewModel : BaseModel
{
    public VaultsListsViewModel VaultsListsViewModel { get; }
    private IAsyncReloadable? ReloadablePlugin { get; }
    private Settings? Settings { get; }

    // Parameterless constructor for design-time data
    public SettingsViewModel() => VaultsListsViewModel = new VaultsListsViewModel();

    public SettingsViewModel(IAsyncReloadable reloadablePlugin, VaultManager vaultManager)
    {
        ReloadablePlugin = reloadablePlugin;
        Settings = vaultManager.Settings;
        VaultsListsViewModel = new VaultsListsViewModel(vaultManager);
    }

    public Task? ReloadPluginDataAsync() => ReloadablePlugin?.ReloadDataAsync();
}
