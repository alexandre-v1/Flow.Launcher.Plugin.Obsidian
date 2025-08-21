using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

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

    private void RegisterQueries()
    {
        Keywords.Clear();
        foreach (ObsidianQuerySetting querySetting in _settings.Queries)
        {
            AddQuery(querySetting);
        }
    }

    private bool AddQuery(ObsidianQuerySetting obsidianQuerySetting)
    {
        bool keywordRegistered = TryRegisterKeyword(obsidianQuerySetting.Keyword);
        if (keywordRegistered)
        {
            CreateQuery(obsidianQuerySetting);
        }

        return keywordRegistered;
    }

    private void CreateQuery(ObsidianQuerySetting obsidianQuerySetting)
    {
        switch (obsidianQuerySetting)
        {
            case FilesQuerySetting filesQuery:
                FilesQuery fileQuery = new(filesQuery, _noteCreatorService, _tagSearchService, _vaultManager);
                _queries.Add(fileQuery);
                break;
            default:
                throw new InvalidCastException($"Query type {obsidianQuerySetting.GetType()} is not implemented");
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
