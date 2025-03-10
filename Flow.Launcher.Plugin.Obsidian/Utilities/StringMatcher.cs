using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class StringMatcher
{
    public static List<(File File, int Distance)> FindClosestMatches(List<File> sourceList, string searchTerm,
        int maxDistance = 3) =>
        sourceList
            .Select(file => (
                Text: file,
                Distance: CalculateLevenshteinDistance(file.Title.ToLower(), searchTerm.ToLower())
            ))
            .Where(result => result.Distance <= maxDistance)
            .OrderBy(result => result.Distance)
            .ToList();

    public static int CalculateLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
            return string.IsNullOrEmpty(target) ? 0 : target.Length;

        if (string.IsNullOrEmpty(target))
            return source.Length;

        int[,] matrix = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= target.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        for (int j = 1; j <= target.Length; j++)
        {
            int cost = source[i - 1] == target[j - 1] ? 0 : 1;
            matrix[i, j] = Math.Min(
                Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                matrix[i - 1, j - 1] + cost
            );
        }

        return matrix[source.Length, target.Length];
    }
}
