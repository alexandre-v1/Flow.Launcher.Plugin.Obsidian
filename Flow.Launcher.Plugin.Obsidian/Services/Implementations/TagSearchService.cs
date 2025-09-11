using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class TagSearchService(IPublicAPI publicApi) : ITagSearchService
{
    public List<Result> GetMatchingTagResults(IEnumerable<string> tags, string tagToSearch,
        FilesQueryData filesQueryData) =>
        CreateTagsResults(filesQueryData, tags)
            .AsParallel()
            .Select(result => SearchUtility.CalculateResultRelevance(result, tagToSearch))
            .ToList();

    private List<Result> CreateTagsResults(FilesQueryData filesQueryData, IEnumerable<string> tags) =>
        tags.Select(tag => CreateTagResult(filesQueryData, tag)).ToList();

    private Result CreateTagResult(FilesQueryData filesQueryData, string tag) =>
        new()
        {
            Title = $"#{tag}",
            SubTitle = "Tag",
            Icon = IconCache.GetCachedIconDelegate(Paths.ObsidianLogo),
            Action = _ =>
                ChangeQueryToAutoCompleteOne(filesQueryData, tag, filesQueryData.GetFirstInvalidTagIndex())
        };

    private bool ChangeQueryToAutoCompleteOne(FilesQueryData filesQueryData, string newTag, int index)
    {
        string newQuery = filesQueryData.GetRawQueryWithReplaced($"#{newTag}", index);
        publicApi.ChangeQuery($"{newQuery} ");
        return false;
    }
}
