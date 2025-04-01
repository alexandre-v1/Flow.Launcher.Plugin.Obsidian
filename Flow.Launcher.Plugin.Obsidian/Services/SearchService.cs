using System.Collections.Generic;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class SearchService
{
    public List<File> SearchAndScoreFilesByName(List<File> files, string search) =>
        SearchUtility.SearchAndScore(
            files,
            search,
            SearchUtility.CalculateFileRelevance
        );
}
