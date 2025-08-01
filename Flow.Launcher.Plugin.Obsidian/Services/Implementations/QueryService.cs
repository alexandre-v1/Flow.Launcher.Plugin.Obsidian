using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class QueryService(IPublicAPI publicApi, Settings settings) : IQueryHandler
{
    private readonly NoteCreatorService _noteCreatorService = new(publicApi);
    private readonly TagSearchService _tagSearchService = new(publicApi);
    private bool UseObsidianProperties => settings.DefaultQuery.UseTags;

    public async Task<List<Result>> HandleQueryAsync(
        QueryData queryData,
        CancellationToken cancellationToken
    )
    {
        if (queryData.IsEmptyQuery())
        {
            return [];
        }

        if (queryData.HasInvalidTags)
        {
            return HandleTagAutoComplete(queryData);
        }

        bool useTags = UseObsidianProperties && queryData.HasValidTags;
        List<File> files = useTags ? queryData.GetAllFilesWithTags() : queryData.GetAllFiles();

        if (!queryData.HasCleanSearchContent())
        {
            return files.ToResults();
        }

        const int minCharForSearchContent = 3;
        bool searchContent =
            queryData.CleanSearchTerms.Length > 1
            || queryData.CleanSearchTerms[0].Length >= minCharForSearchContent;

        files = await SearchUtility.SearchAndScoreFiles(
            files,
            queryData,
            searchContent,
            cancellationToken
        );

        files = SortAndTruncateFilesResults(files);

        List<Result> results = files.ToResults();

        if (settings.DefaultQuery.AddCreateNoteOptionOnAllSearch)
        {
            results.Add(_noteCreatorService.BuildSingleVaultNoteCreationResult(queryData));
        }

        return results;
    }

    public List<Result> HandleNoteCreation(QueryData queryData) =>
        _noteCreatorService.BuildMultiVaultNoteCreationResults(queryData);

    private static List<File> SortFilesResults(List<File> files) =>
        files.OrderByDescending(result => result.Score).ToList();

    private List<Result> HandleTagAutoComplete(QueryData queryData)
    {
        HashSet<string> possibleTags = queryData.GetPossibleTags();
        string tagToAutocomplete = queryData.InvalidTags.First();

        return _tagSearchService.GetMatchingTagResults(possibleTags, tagToAutocomplete, queryData);
    }

    private List<File> SortAndTruncateFilesResults(List<File> files) =>
        settings.DefaultQuery.MaxResult is 0
            ? files.Where(file => file.Score > 0).ToList()
            : SortFilesResults(files)
                .Where(file => file.Score > 0)
                .Take(settings.DefaultQuery.MaxResult)
                .ToList();
}
