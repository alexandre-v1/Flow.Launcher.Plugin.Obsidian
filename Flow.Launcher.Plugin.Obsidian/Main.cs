using System.Collections.Generic;
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
        if (string.IsNullOrEmpty(search))
            return new List<Result>();

        List<File> files = VaultManager.GetAllFiles();
        List<Result> results = SearchService.GetSearchResults(files, search, _settings);
        if (_settings.MaxResult > 0)
            results = SearchService.SortAndTruncateResults(results, _settings.MaxResult);

        return results;
    }

    public void ReloadData() => VaultManager.UpdateVaultList(_settings);

    public Control CreateSettingPanel() => new SettingsView(_settings, this);
}
