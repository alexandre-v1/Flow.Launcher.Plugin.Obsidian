using System;

namespace Flow.Launcher.Plugin.Obsidian.Extensions;

public static class StringExtensions
{
    public static bool IsSameString(this string a, string b) =>
        string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);

    public static string Remove(this string source, string value) =>
        source.Replace(value, string.Empty);
}
