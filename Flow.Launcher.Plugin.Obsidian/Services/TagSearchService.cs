using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class TagSearchService
{
    public static List<Result> GetTagSearchResults(string tagSearch, string actionKeyword, IPublicAPI publicApi)
    {
        string searchLower = tagSearch.ToLower();
        List<Result> results = new();

        foreach (string tag in VaultManager.TagsList)
        {
            string tagLower = tag.ToLower();
            Result result = GetTagResult(tag, actionKeyword, publicApi);
            if (!tagLower.StartsWith(tagSearch)) continue;

            string pattern = $@"[_\s\-\.]{Regex.Escape(searchLower)}";
            int score = SearchService.CalculateScore(tagLower, tagSearch, pattern);
            if (score <= 0) continue;
            result.Score = score;
            results.Add(result);
        }

        return results;
    }

    public static List<Result> GetAllSearchTagResults(IPublicAPI publicApi, string actionKeyword) =>
        VaultManager.TagsList.Select(tag => GetTagResult(tag, actionKeyword, publicApi)).ToList();

    public static List<Result> GetSearchResultWithTag(string lowerTag, string searchWithoutTag, Settings settings)
    {
        Debug.WriteLine($"GetSearchResultWithTag lowertag:'{lowerTag}' searchWithoutTag: '{searchWithoutTag}'");
        return SearchService.GetSearchResults(VaultManager.GetAllFilesWithTag(lowerTag), searchWithoutTag, settings);
    }

    public static bool IsATag(string lowerTagToCheck) =>
        VaultManager.TagsList.Any(tag => tag.ToLower() == lowerTagToCheck);

    public static bool IsSameTag(this string tag, string tagToCheck) =>
        string.Equals(tag, tagToCheck, StringComparison.CurrentCultureIgnoreCase);

    public static bool HasTag(this File file, string tag) =>
        file.Tags?.Any(tagToCheck => tagToCheck.IsSameTag(tag)) ?? false;

    private static Result GetTagResult(string tag, string actionKeyword, IPublicAPI publicApi) =>
        new()
        {
            Title = $"#{tag}",
            SubTitle = "Tag",
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                publicApi.ChangeQuery($"{actionKeyword} #{tag} ");
                return false;
            }
        };
}
