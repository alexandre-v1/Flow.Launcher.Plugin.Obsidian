using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class TagQuery
{
    private readonly HashSet<string> _tags;

    public Query Query { get; }
    public bool HasInvalidTags { get; private set; }
    public int TagStartIndex { get; private set; }
    public IReadOnlySet<string> Tags => _tags;

    public TagQuery(Query query, VaultManager vaultManager)
    {
        Query = query;
        _tags = new HashSet<string>();
        ParseTags(vaultManager);
    }

    public int ValidTagsCount => Tags.Count;
    public bool HasValidTags => Tags.Count > 0;
    public bool HasValidOrInvalidTags => HasValidTags || HasInvalidTags;
    public int InvalidTagIndex => TagStartIndex + ValidTagsCount;

    public string[] SearchTermsWithoutTags => Query.SearchTerms[..TagStartIndex].ToArray();
    public string SecondToEndSearchWithoutTags => string.Join(' ', SearchTermsWithoutTags[1..]);
    public string SearchWithoutTags => string.Join(' ', SearchTermsWithoutTags);
    public string SearchWithoutInvalidTags => string.Join(' ', Query.SearchTerms[..InvalidTagIndex]);
    public string RawQueryWithoutInvalidTags => string.Join(' ', GetRawQueryTerms()[..InvalidTagIndex]);

    public string GetInvalidTag() => !HasInvalidTags ? string.Empty : Query.SearchTerms[InvalidTagIndex].TrimStart('#');

    public string GetSearchAtPos(int startIndex, int endIndex) =>
        string.Join(' ', Query.SearchTerms[startIndex..endIndex]);

    public string[] GetRawQueryTerms()
    {
        if (string.IsNullOrEmpty(Query.ActionKeyword)) return Query.SearchTerms;

        string[] result = new string[Query.SearchTerms.Length + 1];
        result[0] = Query.ActionKeyword;
        Query.SearchTerms.CopyTo(result, 1);
        return result;
    }

    private void ParseTags(VaultManager vaultManager)
    {
        bool wasAValidTag = false;
        HasInvalidTags = false;

        for (int index = 0; index < Query.SearchTerms.Length; index++)
        {
            string? searchTerm = Query.SearchTerms[index];
            if (string.IsNullOrEmpty(searchTerm)) continue;

            if (searchTerm.StartsWith('#'))
            {
                if (!wasAValidTag) TagStartIndex = index;

                string tag = searchTerm.TrimStart('#');
                if (!vaultManager.IsAnExistingTag(tag))
                {
                    HasInvalidTags = true;
                    break;
                }

                _tags.Add(tag);
                wasAValidTag = true;
            }
            else if (wasAValidTag)
            {
                break;
            }
        }
    }
}
