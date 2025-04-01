using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class SearchUtility
{
    public static List<TResult> SearchAndScore<TSource, TResult>(
        IEnumerable<TSource> items,
        string search,
        Func<TSource, SearchInfo, TResult> scoreFunc)
    {
        SearchInfo searchInfo = new(
            search,
            $@"[_\s\-\.]{Regex.Escape(search)}"
        );

        return items
            .AsParallel()
            .Select(item => scoreFunc(item, searchInfo))
            .ToList();
    }

    public static Result CalculateResultRelevance(Result result, SearchInfo searchInfo)
    {
        int score = CalculateScore(result.Title, searchInfo.SearchTerm, searchInfo.Pattern);
        result.Score = score;
        return result;
    }

    public static File CalculateFileRelevance(File file, SearchInfo searchInfo)
    {
        int maxScore = CalculateScore(file.Name, searchInfo.SearchTerm, searchInfo.Pattern);
        string bestMatchTitle = file.Name;

        if (file.Aliases is not null)
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

    public static bool IsWordBreakingChar(char c) =>
        char.IsWhiteSpace(c)
        || c is '_'
        || c is '-'
        || c is '.'
        || c is ','
        || c is ';'
        || c is ':'
        || c is '?'
        || c is '!';

    private static int CalculateScore(string source, string target, string pattern)
    {
        int score = 0;

        if (source.Equals(target, StringComparison.CurrentCultureIgnoreCase)) return 100;

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

    public record SearchInfo(string SearchTerm, string Pattern);
}
