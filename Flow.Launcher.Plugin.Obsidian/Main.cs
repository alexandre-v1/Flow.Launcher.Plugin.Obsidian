using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.Views;
using ContextMenu = Flow.Launcher.Plugin.Obsidian.Services.ContextMenu;

namespace Flow.Launcher.Plugin.Obsidian;

public class Obsidian : IPlugin, ISettingProvider, IReloadable, IContextMenu
{
    private VaultManager? _vaultManager;
    private SearchService? _searchService;
    private NoteCreatorService? _noteCreatorService;
    private TagSearchService? _tagSearchService;

    private IPublicAPI? _publicApi;
    private Settings? _settings;
    private IContextMenu? _contextMenu;

    public List<Result> LoadContextMenus(Result selectedResult) =>
        _contextMenu is null ? new List<Result>() : _contextMenu.LoadContextMenus(selectedResult);

    public void Init(PluginInitContext context)
    {
        _publicApi = context.API;
        _settings = _publicApi.LoadSettingJsonStorage<Settings>();
        _vaultManager = new VaultManager(_settings);

        _searchService = new SearchService(_vaultManager);
        _tagSearchService = new TagSearchService(_vaultManager, _searchService, _publicApi);
        _noteCreatorService = new NoteCreatorService(_vaultManager, _tagSearchService, _publicApi);
        _contextMenu = new ContextMenu(this, _vaultManager);
        ReloadData();
    }

    public List<Result> Query(Query query)
    {
        List<Result> results = new();
        if (_searchService is null || _tagSearchService is null || _noteCreatorService is null) return results;

        string search = query.Search;
        if (string.IsNullOrEmpty(search)) return results;

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

    public void ReloadData() => _vaultManager?.UpdateVaultList(_settings);

    public Control CreateSettingPanel() =>
        _vaultManager is null ? new Control() : new SettingsView(_vaultManager, this);
}
