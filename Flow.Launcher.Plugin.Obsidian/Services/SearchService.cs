using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class SearchService
{
    public static List<Result> GetSearchResults(List<File> files, string search)
    {
        var results = new List<Result>();
        string searchLower = search.ToLower();
        string pattern = $@"[_\s\-\.]{Regex.Escape(searchLower)}";
        
        
        foreach (File file in files)
        {
            string fileTitleLower = file.Title.ToLower();
            
            if (fileTitleLower == searchLower)
            {
                file.Score = 100;
                results.Add(file);
                continue;
            }
            
            int distance = StringMatcher.CalculateLevenshteinDistance(fileTitleLower, searchLower);
    
            if (fileTitleLower.StartsWith(searchLower))
                file.Score = 80;
            else if (fileTitleLower.Contains(searchLower))
            {
                file.Score = 50;
                if (Regex.IsMatch(fileTitleLower, pattern)) // Preceded by special char
                    file.Score += 20;
            }
            else
            {
                continue;
            }

            file.Score += 2 - distance;
            if (file.Score >= 0)
                results.Add(file);
        }

        return results;
    }

    public static List<Result> SortAndTruncateResults(List<Result> results, int maxResult)
    {
        return results.OrderByDescending(result => result.Score).Take(maxResult).ToList();
    }
}
