using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.Views;
using ContextMenu = Flow.Launcher.Plugin.Obsidian.Services.ContextMenu;

namespace Flow.Launcher.Plugin.Obsidian;

public class Obsidian : IAsyncPlugin, ISettingProvider, IAsyncReloadable, IContextMenu
{
    private VaultManager? _vaultManager;
    private QueryHandler? _queryHandler;

    private IPublicAPI? _publicApi;
    private Settings? _settings;
    private IContextMenu? _contextMenu;

    public List<Result> LoadContextMenus(Result selectedResult) =>
        _contextMenu is null ? [] : _contextMenu.LoadContextMenus(selectedResult);

    public async Task InitAsync(PluginInitContext context)
    {
        _publicApi = context.API;
        _settings = _publicApi.LoadSettingJsonStorage<Settings>();
        _vaultManager = new VaultManager(_settings);

        var searchService = new SearchService(_vaultManager);
        var tagSearchService = new TagSearchService(_vaultManager, searchService, _publicApi);
        var noteCreatorService = new NoteCreatorService(_vaultManager, tagSearchService, _publicApi);
        _queryHandler = new QueryHandler( searchService, tagSearchService, noteCreatorService, _settings);
        _contextMenu = new ContextMenu(this, _vaultManager);
        await ReloadDataAsync();
    }

    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        if (_queryHandler is null || QueryTypeDetector.ShouldReturnEmptyResults(query, token)) return [];

        if (QueryTypeDetector.IsNoteCreationSearch(query.Search))
            return _queryHandler.HandleNoteCreation(query);

        if (_queryHandler.IsTagSearchEnabled && QueryTypeDetector.IsTagSearch(query.Search))
            return _queryHandler.HandleTagSearch(query);

        return _queryHandler.HandleRegularSearch(query);
    }

    public async Task ReloadDataAsync()
    {
        if (_vaultManager is null) return;
        await _vaultManager.UpdateVaultListAsync();
    }

    public Control CreateSettingPanel() =>
        _vaultManager is null ? new Control() : new SettingsView(_vaultManager, this);
}
