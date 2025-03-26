using System;
using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.Obsidian.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string a, string b) =>
        string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);

    public static string Remove(this string source, string value) =>
        source.Replace(value, string.Empty);

    public static IEnumerable<string> Without(this IEnumerable<string> source, string value) =>
        source.Where(s => !s.EqualsIgnoreCase(value));

    public static string JoinToString(this IEnumerable<string> source, char separator = ' ') =>
        string.Join(separator, source);

    public static string JoinToString(this IEnumerable<string> source, string separator) =>
        string.Join(separator, source);

    public static bool ContainsIgnoreCase(this IEnumerable<string> source, string value) =>
        source.Any(s => s.EqualsIgnoreCase(value));
}
