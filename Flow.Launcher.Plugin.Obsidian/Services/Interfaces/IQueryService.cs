using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface IQueryService
{
    Task<IEnumerable<Result>> HandleQueriesAsync(Query flowQuery, CancellationToken token);

    ObsidianQuery? GetQuery(ObsidianQuerySetting setting);

    void ReloadQuery(ObsidianQuerySetting setting);

    bool TryChangeKeyword(ObsidianQuerySetting setting, string newKeyword);

    void ShowQuerySettingView(ObsidianQuerySetting querySetting, ISettingsWindowManager windowManager);

    ObsidianQuery CreateQuery(Type type, string name);

    void DeleteQuery(ObsidianQuerySetting setting);
}
