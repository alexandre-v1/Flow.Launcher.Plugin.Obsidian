using System.Collections.Generic;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class QueryHandler(
    SearchService searchService,
    TagSearchService tagSearchService,
    NoteCreatorService noteCreatorService,
    Settings settings)
{

    public bool IsTagSearchEnabled => settings.UseTags;

    public List<Result> HandleTagSearch(Query query)
    {
        var results = tagSearchService.ExecuteTagQuery(query);

        if (!string.IsNullOrEmpty(query.SecondSearch) && settings.AddCreateNoteOptionOnAllSearch)
        {
            results.Add(noteCreatorService.CreateTaggedNoteResult(
                query.ActionKeyword,
                query.SecondSearch,
                query.FirstSearch.TrimStart('#')));
        }

        return results;
    }

    public List<Result> HandleRegularSearch(Query query)
    {
        var results = searchService.FindMatchingFiles(query.Search);

        if (settings.AddCreateNoteOptionOnAllSearch)
        {
            results.Add(noteCreatorService.CreateNewNoteResult(
                query.ActionKeyword,
                query.Search));
        }

        return results;
    }

    public List<Result> HandleNoteCreation(Query query)
    {
        return noteCreatorService.BuildNoteCreationResults(query);
    }
}
