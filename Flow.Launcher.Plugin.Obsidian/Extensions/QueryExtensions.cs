using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Extensions;

public static class QueryExtensions
{
    public static List<Result> ToResults(this IEnumerable<File> source) => source.Cast<Result>().ToList();
}
