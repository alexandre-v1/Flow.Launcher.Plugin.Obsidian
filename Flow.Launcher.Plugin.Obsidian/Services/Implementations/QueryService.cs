using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.ViewModels;
using Flow.Launcher.Plugin.Obsidian.Views;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class QueryService : IQueryService
{
    private readonly INoteCreatorService _noteCreatorService;
    private readonly PluginMetadata _pluginMetadata;
    private readonly IPublicAPI _publicApi;
    private readonly List<ObsidianQuery> _queries = [];
    private readonly Settings _settings;
    private readonly ITagSearchService _tagSearchService;
    private readonly IVaultManager _vaultManager;

    public QueryService(PluginInitContext pluginContext, Settings settings, IVaultManager vaultManager)
    {
        _settings = settings;
        _vaultManager = vaultManager;
        _publicApi = pluginContext.API;
        _pluginMetadata = pluginContext.CurrentPluginMetadata;
        _noteCreatorService = new NoteCreatorService(_publicApi);
        _tagSearchService = new TagSearchService(_publicApi);
        RegisterQueries();
    }

    private List<string> Keywords => _pluginMetadata.ActionKeywords;

    public async Task<IEnumerable<Result>> HandleQueriesAsync(Query flowQuery, CancellationToken token)
    {
        IEnumerable<ObsidianQuery> queriesToPerform = _queries.Where(query => query.IsSameActionKeyword(flowQuery));

        List<Result>[] queriesResults =
            await Task.WhenAll(queriesToPerform.Select(query => query.QueryAsync(flowQuery, token)));

        IEnumerable<Result> groupedResults = queriesResults.SelectMany(results => results);
        return groupedResults;
    }

    public ObsidianQuery? GetQuery(string name) => _queries.FirstOrDefault();

    public T? GetQuery<T>(string name) where T : ObsidianQuery => GetQuery(name) as T;

    public T? GetQuery<T>(ObsidianQuerySetting setting) where T : ObsidianQuery => _queries
        .Where(query => query.Setting == setting).Select(query => query as T).FirstOrDefault();

    public void ReloadQuery(ObsidianQuerySetting setting) =>
        _queries.FirstOrDefault(query => query.Setting == setting)?.Reload();

    public bool TryChangeKeyword(ObsidianQuerySetting setting, string newKeyword)
    {
        if (setting.Keyword == newKeyword)
        {
            _publicApi.ShowMsgBox(_publicApi.GetTranslation("newActionKeywordsSameAsOld"));
            return false;
        }

        if (!TryRegisterKeyword(newKeyword))
        {
            // Keyword assigned by another plugin
            _publicApi.ShowMsgBox(_publicApi.GetTranslation("newActionKeywordsHasBeenAssigned"));
            return false;
        }

        setting.Keyword = newKeyword;
        return true;
    }

    public void ShowQuerySettingView(ObsidianQuerySetting querySetting, ISettingWindowManager windowManager)
    {
        switch (querySetting)
        {
            case FilesQuerySetting setting:
                FilesQuerySettingsViewModel viewModel = new(setting, this, _vaultManager, windowManager);
                windowManager.ShowView<FilesQuerySettingsView>(viewModel);
                break;
            default:
                throw new ArgumentException($"{nameof(querySetting)} is not supported");
        }
    }

    public ObsidianQuery CreateQuery(ObsidianQuerySetting obsidianQuerySetting)
    {
        ObsidianQuery obsidianQuery = obsidianQuerySetting switch
        {
            FilesQuerySetting setting => new FilesQuery(setting, _noteCreatorService, _tagSearchService, _vaultManager),
            _ => throw new ArgumentException($"{nameof(obsidianQuerySetting)} is not supported")
        };

        _queries.Add(obsidianQuery);
        return obsidianQuery;
    }

    public ObsidianQuery CreateQuery(Type type, string name)
    {
        if (!type.IsSubclassOf(typeof(ObsidianQuery)))
        {
            throw new Exception($"Query type must be a subclass of {nameof(ObsidianQuery)}");
        }

        if (type == typeof(FilesQuery))
        {
            FilesQuerySetting setting = new() { Name = name };
            return new FilesQuery(setting, _noteCreatorService, _tagSearchService, _vaultManager);
        }

        throw new ArgumentException($"{type.Name} is not supported");
    }

    private void RegisterQueries()
    {
        Keywords.Clear();
        foreach (ObsidianQuerySetting querySetting in _settings.Queries)
        {
            bool keywordRegistered = TryRegisterKeyword(querySetting.Keyword);
            if (!keywordRegistered)
            {
                continue;
            }

            ObsidianQuery newQuery = CreateQuery(querySetting);
            _queries.Add(newQuery);
        }
    }

    public bool TryRegisterKeyword(string keyword)
    {
        if (Keywords.Contains(keyword))
        {
            return true;
        }

        if (!CanRegisterKeyword(keyword))
        {
            return false;
        }

        Keywords.Add(keyword);
        return true;
    }

    private bool CanRegisterKeyword(string keyword)
    {
        if (keyword is Query.GlobalPluginWildcardSign)
        {
            return true;
        }

        return !_publicApi.ActionKeywordAssigned(keyword) && !string.IsNullOrWhiteSpace(keyword);
    }

    private void RemoveKeyword(string keyword) => Keywords.Remove(keyword);
}
