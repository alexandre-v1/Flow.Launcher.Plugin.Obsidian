using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class QueryData
{
    public HashSet<string> ValidTags { get; } = [];
    public HashSet<string> InvalidTags { get; } = [];

    public bool HasInvalidTags => InvalidTags.Count > 0;
    public bool HasValidTags => ValidTags.Count > 0;
    public readonly string[] CleanSearchTerms;

    public HashSet<Vault> Vaults { get; private set; } = [];

    private string[] SearchTerms => Query.SearchTerms;
    private Query Query { get; }

    private QueryData(Query query)
    {
        Query = query;
        CleanSearchTerms = GetCleanSearchTerms();
    }

    public static QueryData Parse(Query query, HashSet<Vault> vaults)
    {
        QueryData queryData = new(query);
        HashSet<Vault> newVaults = [];

        foreach (string searchTerm in query.SearchTerms)
        {
            if (searchTerm.StartsWith('#')) continue;

            string term = searchTerm;
            foreach (Vault vault in vaults.Where(vault => vault.IsVaultName(term)))
            {
                newVaults.Add(vault);
            }
        }

        queryData.Vaults = newVaults.Count is 0 ? vaults : newVaults;

        foreach (string searchTerm in query.SearchTerms)
        {
            if (!searchTerm.StartsWith('#'))
                continue;

            string tag = searchTerm.TrimStart('#');

            if (queryData.Vaults.Any(vault => vault.TagExists(tag)))
            {
                queryData.ValidTags.Add(tag);
            }
            else
            {
                queryData.InvalidTags.Add(tag);
            }
        }

        return queryData;
    }

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

    public bool HasOnlyOneVault() => Vaults.Count is 1;

    public Vault? GetTheOnlyVault() => Vaults.Count is 1 ? Vaults.First() : null;

    public string GetRawQueryWithAPrefix(string prefix) => $"{Query.ActionKeyword} {prefix} {Query.Search}";

    public HashSet<string> GetPossibleTags() =>
        Vaults.SelectMany(vault => vault.Tags)
            .Where(tag => !ValidTags.ContainsIgnoreCase(tag))
            .ToHashSet();

    public List<File> GetAllFiles() => Vaults.SelectMany(vault => vault.Files).ToList();

    public List<File> GetAllFilesWithTags() =>
        Vaults
            .SelectMany(vault => vault.Files)
            .Where(file => file.HasTags(ValidTags))
            .ToList();

    public bool IsNoteCreationSearch() => Query.Search.StartsWith(Keyword.NoteCreator);

    // Search terms without tags and vaults
    private string[] GetCleanSearchTerms() =>
        Query.SearchTerms
            .Where(term => !term.StartsWith('#') && !Vaults.Any(vault => vault.Name.EqualsIgnoreCase(term)))
            .ToArray();
}
