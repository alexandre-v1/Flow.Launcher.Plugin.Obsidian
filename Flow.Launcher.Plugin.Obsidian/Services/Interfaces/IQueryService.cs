using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface IQueryService
{
    Task<IEnumerable<Result>> HandleQueriesAsync(Query flowQuery, CancellationToken token);
}
