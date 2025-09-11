using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public abstract class ObsidianQuery(ObsidianQuerySetting setting)
{
    public string Name => Setting.Name;

    public virtual ObsidianQuerySetting Setting { get; } = setting;

    public bool IsSameActionKeyword(Query query) => Setting.Keyword == query.ActionKeyword;

    public abstract Task<List<Result>> QueryAsync(Query query, CancellationToken cancellationToken);

    public abstract void Reload();
}
