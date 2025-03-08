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
        string search = query.Search.Trim();
        List<Result> results = new();
        if (string.IsNullOrEmpty(search))
            return results;

        if (NoteCreatorService.IsCreateNewNoteQuery(search))
        {
            results = NoteCreatorService.CreateNewNoteResultsWithVaults(query).ToList();
            return results;
        }

        List<File> files = VaultManager.GetAllFiles();
        results = SearchService.GetSearchResults(files, search, _settings);
        if (_settings.MaxResult > 0)
            results = SearchService.SortAndTruncateResults(results, _settings.MaxResult);

        if (_settings.AddCreateNoteOptionOnAllSearch)
            results.Add(NoteCreatorService.CreateNewNoteResult(query, _publicApi));

        return results;
    }

    public void ReloadData() => VaultManager.UpdateVaultList(_settings);

    public Control CreateSettingPanel() => new SettingsView(_settings, this);
}
