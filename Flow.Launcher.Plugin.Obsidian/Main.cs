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

    private IPublicAPI? _publicApi;
    private Settings? _settings;
    private IContextMenu? _contextMenu;

    public Task InitAsync(PluginInitContext context)
    {
        _publicApi = context.API;
        _settings = _publicApi.LoadSettingJsonStorage<Settings>();
        _vaultManager = new VaultManager(_settings);

        _queryHandler = new QueryHandler(_publicApi, _settings);
        _contextMenu = new ContextMenu(this, _vaultManager);
        return Task.CompletedTask;
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
        _vaultManager is null
            ? new Control()
            : new SettingsView(_vaultManager, this);
}
