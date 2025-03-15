using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class SearchService
{
    private readonly VaultManager _vaultManager;
    private readonly Settings _settings;

    public SearchService(VaultManager vaultManager)
    {
        _settings = vaultManager.Settings;
        _vaultManager = vaultManager;
    }

    public static int CalculateScore(string source, string target, string pattern)
    {
        int score = 0;

        if (string.Equals(source, target, StringComparison.CurrentCultureIgnoreCase)) return 100;

        int distance = StringMatcher.CalculateLevenshteinDistance(source, target);

        if (source.StartsWith(target, StringComparison.CurrentCultureIgnoreCase))
        {
            score = 80;
        }
        else if (source.Contains(target, StringComparison.CurrentCultureIgnoreCase))
        {
            score = 50;
            if (Regex.IsMatch(source, pattern, RegexOptions.IgnoreCase))
                score += 20;
        }
        else
        {
            return score;
        }

        score += 2 - distance;
        return score;
    }

    public List<Result> FindMatchingFiles(string search)
    {
        List<Result> results = CreateFileSearchResults(_vaultManager.GetAllFiles(), search);
        if (_settings.MaxResult > 0)
        {
            results = SortAndTruncateResults(results);
        }

        return results;
    }

    public List<Result> CreateFileSearchResults(List<File> files, string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            //Return all file with Name as Title
            return files.Cast<Result>().ToList();
        }

        SearchInfo searchInfo = new(
            search,
            $@"[_\s\-\.]{Regex.Escape(search)}"
        );

        return files
            .AsParallel()
            .Select(file => CalculateFileRelevance(file, searchInfo))
            .Where(result => result.Score > 0)
            .ToList();
    }

    public List<Result> SortAndTruncateResults(List<Result> results) => _settings.MaxResult is 0
        ? results
        : results.OrderByDescending(result => result.Score).Take(_settings.MaxResult).ToList();

    private Result CalculateFileRelevance(File file, SearchInfo searchInfo)
    {
        int maxScore = CalculateScore(file.Name, searchInfo.SearchTerm, searchInfo.Pattern);
        string bestMatchTitle = file.Name;

        if (_settings.UseAliases && file.Aliases is not null)
        {
            foreach (string alias in file.Aliases)
            {
                int score = CalculateScore(alias, searchInfo.SearchTerm, searchInfo.Pattern);

                if (score < maxScore) continue;
                maxScore = score;
                bestMatchTitle = alias;
                if (score is 100) break;
            }
        }

        file.Score = maxScore;
        file.Title = bestMatchTitle;
        return file;
    }

    private record SearchInfo(string SearchTerm, string Pattern);
}
