using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class SearchUtility
{
    private static readonly char[] _wordBreakingChars = ['_', '-', '.', ',', ';', ':', '|', '?', '!', ' '];

    public static async Task<List<File>> SearchAndScoreFiles(List<File> files, QueryData queryData, bool searchContent,
        CancellationToken cancellationToken)
    {
        const int batchSize = 100;
        List<File> results = [];

        foreach (File[] batch in files.Chunk(batchSize))
        {
            ParallelQuery<Task<File>> tasks = batch.AsParallel()
                .WithCancellation(cancellationToken)
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Select(file => CalculateFileScore(file, queryData.CleanSearchTerms, searchContent));

            results.AddRange(await Task.WhenAll(tasks));
        }

        return results;
    }

    public static Result CalculateResultRelevance(Result result, string search)
    {
        int score = StringMatcher.CalculateWordScore(result.Title, search);
        result.Score = score;
        return result;
    }

    public static bool IsWordBreakingChar(char c) => _wordBreakingChars.Contains(c);

    private static async Task<File> CalculateFileScore(File file, string[] searchTerms, bool searchContent)
    {
        List<string> titleToSearch = [file.Name];
        if (file.Aliases is not null)
        {
            titleToSearch.AddRange(file.Aliases);
        }

        string bestMatchTitle = file.Name;
        int bestScore = 0;

        foreach (string title in titleToSearch)
        {
            string[] tileTerms = SplitByWordBreakingChar(title);
            int score = CalculateScore(tileTerms, searchTerms);

            if (score < bestScore)
            {
                continue;
            }

            bestScore = score;
            bestMatchTitle = title;
            if (score is 100)
            {
                break;
            }
        }

        file.Score = bestScore;
        file.Title = bestMatchTitle;

        if (bestScore is 100)
        {
            return file;
        }

        if (!searchContent || !file.CanSearchContent())
        {
            return file;
        }

        ContentSearchMatch? bestMatch = await ContentSearch.GetBestMatchInFile(file, searchTerms);

        if (bestMatch is null)
        {
            return file;
        }

        if (bestMatch.Score < file.Score)
        {
            return file;
        }

        string matchedWord = bestMatch.ExtractMatchedWord();
        file.Title = $"{file.Name} - {matchedWord}";
        file.Score = bestMatch.Score;
        return file;
    }

    private static string[] SplitByWordBreakingChar(string title) => title.Split(_wordBreakingChars,
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static int CalculateScore(string[] sourceTerms, string[] searchTerms)
    {
        double totalScore = 0;
        int matchCount = 0;

        foreach (string searchWord in searchTerms)
        {
            double bestWordScore = 0;

            for (int index = 0; index < sourceTerms.Length; index++)
            {
                string sourceWord = sourceTerms[index];
                double currentScore = StringMatcher.CalculateWordScore(sourceWord, searchWord);
                if (index is 0 && currentScore > 0)
                {
                    currentScore += 15;
                }

                bestWordScore = Math.Max(bestWordScore, currentScore);
            }

            if (!(bestWordScore > 0))
            {
                continue;
            }

            totalScore += bestWordScore;
            matchCount++;
        }

        double averageScore = matchCount > 0 ? totalScore / matchCount : 0;
        double coverage = (double)matchCount / searchTerms.Length;

        return (int)Math.Round(averageScore * coverage);
    }
}
