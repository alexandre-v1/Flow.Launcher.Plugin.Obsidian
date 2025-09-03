using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FilesQuery : ObsidianQuery
{
    public static readonly ObsidianQueryInfo QueryInfo = new(typeof(FilesQuery), "Files",
        "Search files, open and create them", new FluentGlyphInfo(Glyph.File));

    private readonly INoteCreatorService _noteCreatorService;
    private readonly ITagSearchService _tagSearchService;
    private readonly IVaultManager _vaultManager;

    private readonly HashSet<Vault> _vaults = [];
    private HashSet<string> _vaultIds;

    public FilesQuery(FilesQuerySetting setting, INoteCreatorService noteCreatorService,
        ITagSearchService tagSearchService, IVaultManager vaultManager) : base(setting)
    {
        Setting = setting;
        _noteCreatorService = noteCreatorService;
        _tagSearchService = tagSearchService;
        _vaultManager = vaultManager;
        RefreshVaults();
    }

    public override FilesQuerySetting Setting { get; }

    public override async Task<List<Result>> QueryAsync(Query query, CancellationToken cancellationToken)
    {
        RefreshVaults();
        FilesQueryData filesQueryData = new(query, Setting, _vaults);
        if (filesQueryData.IsEmptyQuery())
        {
            return [];
        }

        if (filesQueryData.IsNoteCreationQuery())
        {
            return HandleNoteCreation(filesQueryData);
        }

        if (filesQueryData.HasInvalidTags)
        {
            return HandleTagAutoComplete(filesQueryData);
        }

        List<File> files = filesQueryData.HasValidTags
            ? filesQueryData.GetFilesWithTags().ToList()
            : filesQueryData.GetFiles().ToList();

        if (!filesQueryData.HasCleanSearchContent())
        {
            return files.ToResults();
        }

        const int minCharForSearchContent = 3;
        bool searchContent = filesQueryData.CleanSearchTerms.Length > 1 ||
                             filesQueryData.CleanSearchTerms[0].Length >= minCharForSearchContent;

        files = await SearchUtility.SearchAndScoreFiles(files, filesQueryData, searchContent, cancellationToken);

        files = SortAndTruncateFilesResults(files);

        List<Result> results = files.ToResults();

        if (Setting.AddCreateNoteResult)
        {
            results.Add(_noteCreatorService.BuildSingleVaultNoteCreationResult(filesQueryData));
        }

        return results;
    }

    public override void Reload()
    {
        RefreshVaults();
        foreach (Vault vault in _vaults)
        {
            vault.UpdateVault();
        }
    }

    [MemberNotNull(nameof(_vaultIds))]
    private void RefreshVaults()
    {
        Setting.VaultIds ??= _vaultManager.GetActiveVaults().Select(x => x.Id).ToHashSet();

        HashSet<string> vaultIdsToRemove;
        HashSet<string> vaultIdsToAdd;
        if (_vaultIds is not null)
        {
            vaultIdsToRemove = _vaultIds.Except(Setting.VaultIds).ToHashSet();
            vaultIdsToAdd = Setting.VaultIds.Except(_vaultIds).ToHashSet();

            if (vaultIdsToRemove.Count is 0 && vaultIdsToAdd.Count is 0)
            {
                return;
            }
        }
        else
        {
            vaultIdsToRemove = [];
            vaultIdsToAdd = Setting.VaultIds;
        }

        IEnumerable<Vault> vaultsToRemove = _vaultManager.GetVaultsWithIds(vaultIdsToRemove);
        foreach (Vault vault in vaultsToRemove)
        {
            _vaults.Remove(vault);
        }

        IEnumerable<Vault> vaultsToAdd = _vaultManager.GetVaultsWithIds(vaultIdsToAdd);
        foreach (Vault vault in vaultsToAdd)
        {
            _vaults.Add(vault);
        }

        _vaultIds = Setting.VaultIds;
    }

    private List<Result> HandleNoteCreation(FilesQueryData filesQueryData) =>
        _noteCreatorService.BuildMultiVaultNoteCreationResults(filesQueryData);

    private List<Result> HandleTagAutoComplete(FilesQueryData filesQueryData)
    {
        HashSet<string> possibleTags = filesQueryData.GetPossibleTags();
        string tagToAutocomplete = filesQueryData.InvalidTags.First();

        return _tagSearchService.GetMatchingTagResults(possibleTags, tagToAutocomplete, filesQueryData);
    }

    private List<File> SortAndTruncateFilesResults(List<File> files) =>
        Setting.MaxResult is 0
            ? files.Where(file => file.Score > 0).ToList()
            : SortFilesResults(files)
                .Where(file => file.Score > 0)
                .Take(Setting.MaxResult)
                .ToList();

    private static List<File> SortFilesResults(List<File> files) =>
        files.OrderByDescending(result => result.Score).ToList();
}
