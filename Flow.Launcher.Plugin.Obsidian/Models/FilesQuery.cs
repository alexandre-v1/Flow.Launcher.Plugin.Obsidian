using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Services.Implementations;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FilesQuery : BaseQuery
{
    private readonly NoteCreatorService _noteCreatorService;
    private readonly TagSearchService _tagSearchService;
    private readonly IVaultManager _vaultManager;

    public FilesQuery(FilesQuerySetting querySetting, NoteCreatorService noteCreatorService,
        TagSearchService tagSearchService, IVaultManager vaultManager) : base(querySetting)
    {
        QuerySetting = querySetting;
        _noteCreatorService = noteCreatorService;
        _tagSearchService = tagSearchService;
        _vaultManager = vaultManager;
        LoadVaults();
    }

    private HashSet<Vault> Vaults { get; } = [];
    protected override FilesQuerySetting QuerySetting { get; }

    public override async Task<List<Result>> QueryAsync(Query query, CancellationToken cancellationToken)
    {
        QueryData queryData = QueryData.Parse(query, QuerySetting.FileExtensions, Vaults);
        if (queryData.IsEmptyQuery())
        {
            return [];
        }

        if (queryData.IsNoteCreationQuery())
        {
            return HandleNoteCreation(queryData);
        }

        if (queryData.HasInvalidTags)
        {
            return HandleTagAutoComplete(queryData);
        }

        List<File> files = queryData.HasValidTags
            ? queryData.GetFilesWithTags().ToList()
            : queryData.GetFiles().ToList();

        if (!queryData.HasCleanSearchContent())
        {
            return files.ToResults();
        }

        const int minCharForSearchContent = 3;
        bool searchContent = queryData.CleanSearchTerms.Length > 1 ||
                             queryData.CleanSearchTerms[0].Length >= minCharForSearchContent;

        files = await SearchUtility.SearchAndScoreFiles(files, queryData, searchContent, cancellationToken);

        files = SortAndTruncateFilesResults(files);

        List<Result> results = files.ToResults();

        if (QuerySetting.AddCreateNoteResult)
        {
            results.Add(_noteCreatorService.BuildSingleVaultNoteCreationResult(queryData));
        }

        return results;
    }

    private void LoadVaults()
    {
        if (QuerySetting.VaultIds is null)
        {
            QuerySetting.VaultIds = [];
            foreach (Vault vault in _vaultManager.Vaults)
            {
                Vaults.Add(vault);
                QuerySetting.VaultIds.Add(vault.Id);
            }

            return;
        }

        IEnumerable<Vault> vaults = QuerySetting.VaultIds.Select(_vaultManager.GetVaultWithId).OfType<Vault>();
        foreach (Vault vault in vaults)
        {
            Vaults.Add(vault);
        }
    }

    private List<Result> HandleNoteCreation(QueryData queryData) =>
        _noteCreatorService.BuildMultiVaultNoteCreationResults(queryData);

    private List<Result> HandleTagAutoComplete(QueryData queryData)
    {
        HashSet<string> possibleTags = queryData.GetPossibleTags();
        string tagToAutocomplete = queryData.InvalidTags.First();

        return _tagSearchService.GetMatchingTagResults(possibleTags, tagToAutocomplete, queryData);
    }

    private List<File> SortAndTruncateFilesResults(List<File> files) =>
        QuerySetting.MaxResult is 0
            ? files.Where(file => file.Score > 0).ToList()
            : SortFilesResults(files)
                .Where(file => file.Score > 0)
                .Take(QuerySetting.MaxResult)
                .ToList();

    private static List<File> SortFilesResults(List<File> files) =>
        files.OrderByDescending(result => result.Score).ToList();
}
