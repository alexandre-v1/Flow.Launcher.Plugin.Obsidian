using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class TagSearchService(IPublicAPI publicApi) : ITagSearchService
{
    public List<Result> GetMatchingTagResults(IEnumerable<string> tags, string tagToSearch, QueryData queryData) =>
        CreateTagsResults(queryData, tags)
            .AsParallel()
            .Select(result => SearchUtility.CalculateResultRelevance(result, tagToSearch))
            .ToList();

    private List<Result> CreateTagsResults(QueryData queryData, IEnumerable<string> tags) =>
        tags.Select(tag => CreateTagResult(queryData, tag)).ToList();

    private Result CreateTagResult(QueryData queryData, string tag) =>
        new()
        {
            Title = $"#{tag}",
            SubTitle = "Tag",
            IcoPath = Paths.ObsidianLogo,
            Action = _ => ChangeQueryToAutoCompleteOne(queryData, tag, queryData.GetFirstInvalidTagIndex())
        };

    private bool ChangeQueryToAutoCompleteOne(QueryData queryData, string newTag, int index)
    {
        string newQuery = queryData.GetRawQueryWithReplaced($"#{newTag}", index);
        publicApi.ChangeQuery($"{newQuery} ");
        return false;
    }
}
