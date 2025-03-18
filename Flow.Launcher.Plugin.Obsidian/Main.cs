using System.Collections.Generic;
using System.Linq;
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
    private SearchService? _searchService;
    private NoteCreatorService? _noteCreatorService;
    private TagSearchService? _tagSearchService;

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

        _searchService = new SearchService(_vaultManager);
        _tagSearchService = new TagSearchService(_vaultManager, _searchService, _publicApi);
        _noteCreatorService = new NoteCreatorService(_vaultManager, _tagSearchService, _publicApi);
        _contextMenu = new ContextMenu(this, _vaultManager);
        await ReloadDataAsync();
    }

    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        List<Result> results = [];
        if (_searchService is null || _tagSearchService is null || _noteCreatorService is null) return results;

        string search = query.Search;
        if (string.IsNullOrEmpty(search)) return results;

        if (token.IsCancellationRequested) return results;

        if (QueryTypeDetector.IsNoteCreationQuery(search))
        {
            return _noteCreatorService.BuildNoteCreationResults(query).ToList();
        }

        if (_settings is { UseTags: true } && QueryTypeDetector.IsTagSearchQuery(search))
        {
            results = _tagSearchService.ExecuteTagQuery(query);
            if (string.IsNullOrEmpty(query.SecondSearch)) return results;
            if (_settings is not { AddCreateNoteOptionOnAllSearch: true }) return results;
            results.Add(_noteCreatorService.CreateTaggedNoteResult(query.ActionKeyword, query.SecondSearch,
                query.FirstSearch.TrimStart('#')));
        }
        else
        {
            results = _searchService.FindMatchingFiles(search);
            if (_settings is not { AddCreateNoteOptionOnAllSearch: true }) return results;
            results.Add(_noteCreatorService.CreateNewNoteResult(query.ActionKeyword, search));
        }
        return results;
    }

    public async Task ReloadDataAsync()
    {
        if (_vaultManager is null) return;
        await _vaultManager.UpdateVaultListAsync();
    }

    public Control CreateSettingPanel() =>
        _vaultManager is null ? new Control() : new SettingsView(_vaultManager, this);
}
