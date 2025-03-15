using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class TagSearchService
{
    private readonly VaultManager _vaultManager;
    private readonly SearchService _searchService;
    private readonly IPublicAPI _publicApi;

    public TagSearchService(VaultManager vaultManager, SearchService searchService, IPublicAPI publicApi)
    {
        _vaultManager = vaultManager;
        _searchService = searchService;
        _publicApi = publicApi;
    }

    public List<Result> FindMatchingTags(string search, string actionKeyword)
    {
        TagSearchInfo searchInfo = new(
            search,
            $@"[_\s\-\.]{Regex.Escape(search)}"
        );

        return _vaultManager.TagsList
            .AsParallel()
            .Select(tag => CreateScoredTagResult(tag, searchInfo, actionKeyword))
            .Where(result => result.Score > 0)
            .ToList();
    }

    public List<Result> ExecuteTagQuery(Query query)
    {
        string tag = query.FirstSearch.TrimStart('#');

        List<Result> results = _vaultManager.IsAnExistingTag(tag)
            ? FindFilesWithTag(tag, query)
            : FindMatchingTags(tag, query.ActionKeyword);

        return results;
    }

    private Result CreateScoredTagResult(string tag, TagSearchInfo searchInfo, string actionKeyword)
    {
        Result result = CreateTagResult(tag, actionKeyword);

        int score = SearchService.CalculateScore(tag, searchInfo.SearchTerm, searchInfo.Pattern);
        result.Score = score;
        return result;
    }

    private List<Result> FindFilesWithTag(string tag, Query query)
    {
        List<Result> results = FilterFilesWithTagByQuery(tag, query.SecondToEndSearch);

        if (_vaultManager.Settings.MaxResult > 0)
            results = _searchService.SortAndTruncateResults(results);

        return results;
    }

    private List<Result> FilterFilesWithTagByQuery(string tag, string search)
    {
        List<File> filesWithTag = _vaultManager.GetAllFilesWithTag(tag);
        return string.IsNullOrEmpty(search)
            ? filesWithTag.Cast<Result>().ToList()
            : _searchService.CreateFileSearchResults(filesWithTag, search);
    }

    private Result CreateTagResult(string tag, string actionKeyword) =>
        new()
        {
            Title = $"#{tag}",
            SubTitle = "Tag",
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                _publicApi.ChangeQuery($"{actionKeyword} #{tag} ");
                return false;
            }
        };

    private record TagSearchInfo(string SearchTerm, string Pattern);
}
