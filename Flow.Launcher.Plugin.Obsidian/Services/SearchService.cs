using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class SearchService
{
    public static List<Result> GetSearchResults(List<File> files, string search, Settings settings)
    {
        ConcurrentBag<Result> results = new();
        string searchLower = search.ToLower();
        string pattern = $@"[_\s\-\.]{Regex.Escape(searchLower)}";

        Parallel.ForEach(files, file =>
        {
            int maxScore = CalculateScore(file.Name, searchLower, pattern);
            string bestMatchTitle = file.Name;

            if (settings.UseAliases && file.Aliases != null)
                foreach (string alias in file.Aliases)
                {
                    int score = CalculateScore(alias, searchLower, pattern);

                    if (score <= maxScore) continue;
                    maxScore = score;
                    bestMatchTitle = alias;
                    if (score == 100) break;
                }

            if (maxScore <= 0) return;
            file.Score = maxScore;
            file.Title = bestMatchTitle;
            if (settings.UseFilesExtension)
                file.Title += file.Extension;
            results.Add(file);
        });

        return results.ToList();
    }

    public static List<Result> SortAndTruncateResults(List<Result> results, int maxResult) =>
        results.OrderByDescending(result => result.Score).Take(maxResult).ToList();

    private static int CalculateScore(string name, string searchLower, string pattern)
    {
        int score = 0;
        string fileTitleLower = name.ToLower();

        if (fileTitleLower == searchLower) return 100;

        int distance = StringMatcher.CalculateLevenshteinDistance(fileTitleLower, searchLower);

        if (fileTitleLower.StartsWith(searchLower))
        {
            score = 80;
        }
        else if (fileTitleLower.Contains(searchLower))
        {
            score = 50;
            if (Regex.IsMatch(fileTitleLower, pattern)) // Preceded by special char
                score += 20;
        }
        else
        {
            return score;
        }

        score += 2 - distance;
        return score;
    }
}
