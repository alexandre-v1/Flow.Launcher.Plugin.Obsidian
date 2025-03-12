using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.Views;
using ContextMenu = Flow.Launcher.Plugin.Obsidian.Services.ContextMenu;

namespace Flow.Launcher.Plugin.Obsidian;

public class Obsidian : IPlugin, ISettingProvider, IReloadable, IContextMenu
{
    private IPublicAPI _publicApi = null!;
    private Settings _settings = null!;
    private IContextMenu _contextMenu = null!;

    public List<Result> LoadContextMenus(Result selectedResult) => _contextMenu.LoadContextMenus(selectedResult);

    public void Init(PluginInitContext context)
    {
        _publicApi = context.API;
        _settings = _publicApi.LoadSettingJsonStorage<Settings>();
        _contextMenu = new ContextMenu(this, _settings);
        ReloadData();
    }

    public List<Result> Query(Query query)
    {
        string search = query.Search;
        List<Result> results = new();
        if (string.IsNullOrEmpty(search)) return results;

        if (QueryTypeDetector.IsCreateNewNoteQuery(search))
        {
            return NoteCreatorService.CreateNewNoteResultsWithVaults(query).ToList();
        }

        if (_settings.UseTags && QueryTypeDetector.IsTagSearchQuery(search))
        {
            if (search is "#")
            {
                return TagSearchService.GetAllSearchTagResults(_publicApi, query.ActionKeyword);
            }

            string tag = query.FirstSearch.TrimStart('#');
            string lowerTag = tag.ToLower();
            if (TagSearchService.IsATag(lowerTag))
            {
                results = TagSearchService.GetSearchResultWithTag(lowerTag, query.SecondToEndSearch, _settings);

                if (_settings.MaxResult > 0)
                    results = SearchService.SortAndTruncateResults(results, _settings.MaxResult);
                if (_settings.AddCreateNoteOptionOnAllSearch && string.IsNullOrEmpty(query.SecondToEndSearch))
                    results.Add(NoteCreatorService.CreateNewNoteResult(query.ActionKeyword, query.SecondToEndSearch,
                        _publicApi));

                return results;
            }

            return TagSearchService.GetTagSearchResults(lowerTag, query.ActionKeyword, _publicApi);
        }

        List<File> files = VaultManager.GetAllFiles();

        results = SearchService.GetSearchResults(files, search, _settings);
        if (_settings.MaxResult > 0)
            results = SearchService.SortAndTruncateResults(results, _settings.MaxResult);

        if (_settings.AddCreateNoteOptionOnAllSearch && string.IsNullOrEmpty(search))
            results.Add(NoteCreatorService.CreateNewNoteResult(query.ActionKeyword, search, _publicApi));

        return results;
    }

    public void ReloadData() => VaultManager.UpdateVaultList(_settings);

    public Control CreateSettingPanel() => new SettingsView(_settings, this);
}
