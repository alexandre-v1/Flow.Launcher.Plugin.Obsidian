using System.Collections.Generic;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface ITagSearchService
{
    List<Result> GetMatchingTagResults(IEnumerable<string> tags, string tagToSearch, QueryData queryData);
}
