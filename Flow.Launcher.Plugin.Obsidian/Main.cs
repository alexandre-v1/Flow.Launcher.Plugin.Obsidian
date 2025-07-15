using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.ViewModels;
using Flow.Launcher.Plugin.Obsidian.Views;
using ContextMenu = Flow.Launcher.Plugin.Obsidian.Interactions.ContextMenu;

namespace Flow.Launcher.Plugin.Obsidian;

public class Obsidian : IAsyncPlugin, ISettingProvider, IAsyncReloadable, IContextMenu
{
    private VaultManager? _vaultManager;
    private QueryHandler? _queryHandler;
    private SettingsViewModel? _settingsViewModel;

    private IPublicAPI? _publicApi;
    private Settings? _settings;
    private IContextMenu? _contextMenu;

    public async Task InitAsync(PluginInitContext context)
    {
        _publicApi = context.API;
        _settings = _publicApi.LoadSettingJsonStorage<Settings>();
        _vaultManager = new VaultManager(_settings);

        //Todo: Make VaultManager be able to update one vault at the time
        //update only if change has been made
        //and check if the all is update only once at launch (ReloadDataAsync() is run at launch)
        await _vaultManager.UpdateVaultListAsync();

        _queryHandler = new QueryHandler(_publicApi, _settings);
        _contextMenu = new ContextMenu(this, _vaultManager);
        _settingsViewModel = new SettingsViewModel(this, _vaultManager);
    }

    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        if (_queryHandler is null || _vaultManager is null) return [];

        QueryData queryData = QueryData.Parse(query, _vaultManager.Vaults);

        return queryData.IsNoteCreationSearch()
            ? _queryHandler.HandleNoteCreation(queryData)
            : await _queryHandler.HandleQuery(queryData, token);
    }

    public async Task ReloadDataAsync()
    {
        if (_vaultManager is null) return;
        await _vaultManager.UpdateVaultListAsync();
    }

    public List<Result> LoadContextMenus(Result selectedResult) =>
        _contextMenu is not null ? _contextMenu.LoadContextMenus(selectedResult) : [];

    public Control CreateSettingPanel() =>
        _settingsViewModel is null
            ? new Control()
            : new SettingsView(_settingsViewModel);
}
