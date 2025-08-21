using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface IQueryService
{
    Task<IEnumerable<Result>> HandleQueriesAsync(Query flowQuery, CancellationToken token);

    ObsidianQuery? GetQuery(string name);

    T? GetQuery<T>(string name) where T : ObsidianQuery;

    T? GetQuery<T>(ObsidianQuerySetting setting) where T : ObsidianQuery;

    void ReloadQuery(ObsidianQuerySetting setting);

    bool TryChangeKeyword(ObsidianQuerySetting setting, string newKeyword);
}
