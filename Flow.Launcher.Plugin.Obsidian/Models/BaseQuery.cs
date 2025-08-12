using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public abstract class BaseQuery(BaseQuerySetting querySetting)
{
    protected virtual BaseQuerySetting QuerySetting { get; } = querySetting;

    public bool IsSameActionKeyword(Query query) => QuerySetting.Keyword == query.ActionKeyword;

    public abstract Task<List<Result>> QueryAsync(Query query, CancellationToken cancellationToken);
}
