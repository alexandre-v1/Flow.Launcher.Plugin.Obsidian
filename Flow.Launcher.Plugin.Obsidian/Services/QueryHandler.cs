using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class QueryHandler(IPublicAPI publicApi, Settings settings)
{
    private bool UseObsidianProperties => settings.UseTags;
    private readonly SearchService _searchService = new();
    private readonly TagSearchService _tagSearchService = new(publicApi);
    private readonly NoteCreatorService _noteCreatorService = new(publicApi);
    private readonly ContentSearchService _contentSearchService = new();

    public async Task<List<Result>> HandleQuery(QueryData queryData)
    {
        if (queryData.IsEmptyQuery()) return [];

        if (queryData.HasInvalidTags)
        {
            return HandleTagAutoComplete(queryData);
        }

        bool useTags = UseObsidianProperties && queryData.HasValidTags;
        List<File> files = useTags ? queryData.GetAllFilesWithTags() : queryData.GetAllFiles();

        if (!queryData.HasCleanSearchContent()) return files.ToResults();

        string cleanSearch = queryData.GetCleanSearch();
        files = _searchService.SearchAndScoreFilesByName(files, cleanSearch);
        files = await _contentSearchService.SearchAndScoreFilesByContentAsync(files, cleanSearch);

        files = SortAndTruncateFilesResults(files);


        List<Result> results = files.ToResults();

        if (settings.AddCreateNoteOptionOnAllSearch)
        {
            results.Add(_noteCreatorService.BuildSingleVaultNoteCreationResult(queryData));
        }

        return results;
    }

    public List<Result> HandleNoteCreation(QueryData queryData) =>
        _noteCreatorService.BuildMultiVaultNoteCreationResults(queryData);

    private List<Result> HandleTagAutoComplete(QueryData queryData)
    {
        HashSet<string> possibleTags = queryData.GetPossibleTags();
        string tagToAutocomplete = queryData.InvalidTags.First();

        return _tagSearchService.GetMatchingTagResults(possibleTags, tagToAutocomplete, queryData);
    }

    private List<File> SortAndTruncateFilesResults(List<File> files) =>
        settings.MaxResult is 0
            ? files.Where(file => file.Score > 0).ToList()
            : SortFilesResults(files).Where(file => file.Score > 0).Take(settings.MaxResult).ToList();

    private List<File> SortFilesResults(List<File> files) =>
        files.OrderByDescending(result => result.Score).ToList();
}
