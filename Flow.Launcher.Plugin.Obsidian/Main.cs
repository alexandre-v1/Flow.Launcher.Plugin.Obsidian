using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Implementations;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.ViewModels;
using Flow.Launcher.Plugin.Obsidian.Views;
using JetBrains.Annotations;
using ContextMenuService = Flow.Launcher.Plugin.Obsidian.Services.Implementations.ContextMenuService;

namespace Flow.Launcher.Plugin.Obsidian;

[UsedImplicitly]
public class Obsidian : IAsyncPlugin, ISettingProvider, IAsyncReloadable, IContextMenu
{
    private IContextMenu? _contextMenu;

    private IPublicAPI? _publicApi;
    private IQueryService? _queryService;
    private Settings? _settings;
    private SettingsViewModel? _settingsViewModel;

    private IVaultManager? _vaultManager;
    private ISettingsWindowManager? _windowManager;

    public async Task InitAsync(PluginInitContext context)
    {
        _publicApi = context.API;
        _settings = _publicApi.LoadSettingJsonStorage<Settings>();
        _vaultManager = new VaultManager(_settings);

        await _vaultManager.ReloadVaultsAsync();

        _queryService = new QueryService(context, _settings, _vaultManager);
        _contextMenu = new ContextMenuService(this, _vaultManager, _settings);

        _windowManager = new SettingsWindowManager(_settings);
        _settingsViewModel = new SettingsViewModel(_settings, this, _vaultManager, _windowManager, _queryService);
    }

    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        Task<IEnumerable<Result>>? queriesAsync = _queryService?.HandleQueriesAsync(query, token);
        if (queriesAsync is null)
        {
            return [];
        }

        IEnumerable<Result> results = await queriesAsync;
        return results.ToList();
    }

    public async Task ReloadDataAsync()
    {
        if (_vaultManager is null)
        {
            return;
        }

        await _vaultManager.ReloadVaultsAsync();
    }

    public List<Result> LoadContextMenus(Result selectedResult) =>
        _contextMenu is not null ? _contextMenu.LoadContextMenus(selectedResult) : [];

    public Control CreateSettingPanel() =>
        _settingsViewModel is null ? new Control() : new SettingsView(_settingsViewModel);
}
