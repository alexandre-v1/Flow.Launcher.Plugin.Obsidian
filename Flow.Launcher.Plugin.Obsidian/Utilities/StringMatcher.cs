using System;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class StringMatcher
{
    public static int CalculateWordScore(string source, string search)
    {
        StringMatchType stringMatchType = GetMatchType(source, search);

        switch (stringMatchType)
        {
            case StringMatchType.Exact:
                return 100;
            case StringMatchType.ExactCaseInsensitive:
                return 90 + CalculateClosenessBonus(source, search);
            case StringMatchType.Prefix:
                return 80 + CalculateClosenessBonus(source, search);
            case StringMatchType.PrefixCaseInsensitive:
                return 60 + CalculateClosenessBonus(source, search);
            case StringMatchType.PrefixMinusOne:
                return 40 + CalculateClosenessBonus(source, search);
            case StringMatchType.PrefixMinusOneCaseInsensitive:
                return 20 + CalculateClosenessBonus(source, search);
            case StringMatchType.None:
            default:
                return 0;
        }
    }

    private static int CalculateClosenessBonus(string source, string target)
    {
        int levenshteinDistance = CalculateLevenshteinDistance(source, target);

        int invertedDistance = source.Length - levenshteinDistance;
        return NormalizationUtility.NormalizeToInt(invertedDistance, source.Length, 10);
    }

    private static StringMatchType GetMatchType(string source, string target)
    {
        if (source == target) return StringMatchType.Exact;

        if (source.StartsWith(target, StringComparison.CurrentCultureIgnoreCase))
        {
            if (source.Equals(target, StringComparison.CurrentCultureIgnoreCase))
            {
                return StringMatchType.ExactCaseInsensitive;
            }

            return source.StartsWith(target)
                ? StringMatchType.Prefix
                : StringMatchType.PrefixCaseInsensitive;
        }

        const int minCharForPrefixMinusOne = 2;
        if (target.Length > minCharForPrefixMinusOne + 1 &&
            source.StartsWith(target[..^1], StringComparison.CurrentCultureIgnoreCase))
        {
            return source.StartsWith(target[..^1])
                ? StringMatchType.PrefixMinusOne
                : StringMatchType.PrefixMinusOneCaseInsensitive;
        }

        return StringMatchType.None;
    }

    private static int CalculateLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.IsNullOrEmpty(target) ? 0 : target.Length;
        }

        if (string.IsNullOrEmpty(target)) return source.Length;

        if (source.Length > target.Length)
        {
            (source, target) = (target, source);
        }

        int[] previousRow = new int[source.Length + 1];
        int[] currentRow = new int[source.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            previousRow[i] = i;

        for (int j = 1; j <= target.Length; j++)
        {
            currentRow[0] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                int cost = source[i - 1] == target[j - 1] ? 0 : 1;
                currentRow[i] = Math.Min(
                    Math.Min(currentRow[i - 1] + 1, previousRow[i] + 1),
                    previousRow[i - 1] + cost
                );
            }

            (previousRow, currentRow) = (currentRow, previousRow);
        }

        return previousRow[source.Length];
    }

    private enum StringMatchType
    {
        Exact,
        ExactCaseInsensitive,
        Prefix,
        PrefixCaseInsensitive,
        PrefixMinusOne,
        PrefixMinusOneCaseInsensitive,
        None
    }
}
