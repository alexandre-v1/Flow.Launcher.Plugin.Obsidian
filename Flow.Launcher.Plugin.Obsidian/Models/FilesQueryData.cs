using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FilesQueryData
{
    private readonly FilesQuerySetting _setting;

    private readonly HashSet<Vault> _vaults;
    public readonly string[] CleanSearchTerms;

    public FilesQueryData(Query query, FilesQuerySetting setting, HashSet<Vault> vaults)
    {
        Query = query;
        _setting = setting;
        _vaults = vaults;
        CleanSearchTerms = GetCleanSearchTerms();

        foreach (string searchTerm in query.SearchTerms)
        {
            if (!searchTerm.StartsWith('#'))
            {
                continue;
            }

            string tag = searchTerm.TrimStart('#');

            if (_vaults.Any(vault => vault.TagExists(tag)))
            {
                ValidTags.Add(tag);
            }
            else
            {
                InvalidTags.Add(tag);
            }
        }
    }

    public HashSet<string> ValidTags { get; } = [];
    public HashSet<string> InvalidTags { get; } = [];

    public bool HasInvalidTags => InvalidTags.Count > 0;
    public bool HasValidTags => ValidTags.Count > 0;

    private string[] SearchTerms => Query.SearchTerms;
    private Query Query { get; }

    public string GetRawQueryWithReplaced(string newSearchTerm, int index)
    {
        string[] searchTerms = SearchTerms;
        searchTerms[index] = newSearchTerm;

        return $"{Query.ActionKeyword} {searchTerms.JoinToString()}";
    }

    public int GetFirstInvalidTagIndex()
    {
        for (int i = 0; i < SearchTerms.Length; i++)
        {
            if (SearchTerms[i].StartsWith('#') && !ValidTags.Contains(SearchTerms[i].TrimStart('#')))
            {
                return i;
            }
        }

        return -1;
    }

    public bool IsEmptyQuery() => SearchTerms.Length is 0;

    public bool HasCleanSearchContent() => CleanSearchTerms.Length > 0;

    public bool HasOnlyOneVault() => _vaults.Count is 1;

    public Vault? GetTheOnlyVault() => _vaults.Count is 1 ? _vaults.First() : null;

    public string GetRawQueryWithAPrefix(string prefix) => $"{Query.ActionKeyword} {prefix} {Query.Search}";

    public HashSet<string> GetPossibleTags() =>
        _vaults.SelectMany(vault => vault.Tags)
            .Where(tag => !ValidTags.ContainsIgnoreCase(tag))
            .ToHashSet();

    public IEnumerable<File> GetFiles() => _vaults.Select(AllSearchableFiles).SelectMany(file => file);

    private IEnumerable<File> AllSearchableFiles(Vault vault) => vault.Files.Where(CanBeSearch);

    private bool CanBeSearch(File file) => _setting.FileExtensions.Contains(file.Extension) &&
                                           _setting.RelativeExcludePaths.All(relativeExcludePath =>
                                               !file.RelativePath.StartsWith(relativeExcludePath));

    public IEnumerable<Vault> GetVaults() => _vaults;

    public IEnumerable<File> GetFilesWithTags() => GetFiles().Where(file => file.HasTags(ValidTags));

    public bool IsNoteCreationQuery() => Query.Search.StartsWith(Keyword.NoteCreator);

    // Search terms without tags and vaults
    private string[] GetCleanSearchTerms() =>
        Query.SearchTerms
            .Where(term => !term.StartsWith('#') && !_vaults.Any(vault => vault.Name.EqualsIgnoreCase(term)))
            .ToArray();
}
