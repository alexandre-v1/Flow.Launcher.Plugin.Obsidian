using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class QueryService : IQueryHandler
{
    private readonly NoteCreatorService _noteCreatorService;
    private readonly PluginMetadata _pluginMetadata;
    private readonly IPublicAPI _publicApi;
    private readonly List<BaseQuery> _queries = [];
    private readonly Settings _settings;
    private readonly TagSearchService _tagSearchService;
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
        IEnumerable<BaseQuery> queriesToPerform = _queries.Where(query => query.IsSameActionKeyword(flowQuery));

        List<Result>[] queriesResults =
            await Task.WhenAll(queriesToPerform.Select(query => query.QueryAsync(flowQuery, token)));

        IEnumerable<Result> groupedResults = queriesResults.SelectMany(results => results);
        return groupedResults;
    }

    private void RegisterQueries()
    {
        Keywords.Clear();
        foreach (BaseQuerySetting querySetting in _settings.Queries)
        {
            AddQuery(querySetting);
        }
    }

    private bool AddQuery(BaseQuerySetting querySetting)
    {
        bool keywordRegistered = RegisterKeyword(querySetting.Keyword);
        if (keywordRegistered)
        {
            CreateQuery(querySetting);
        }

        return keywordRegistered;
    }

    private void CreateQuery(BaseQuerySetting querySetting)
    {
        switch (querySetting)
        {
            case FilesQuerySetting filesQuery:
                FilesQuery fileQuery = new(filesQuery, _noteCreatorService, _tagSearchService, _vaultManager);
                _queries.Add(fileQuery);
                break;
            default:
                throw new NotImplementedException($"Query type {querySetting.GetType()} is not implemented");
        }
    }

    private bool RegisterKeyword(string keyword)
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
