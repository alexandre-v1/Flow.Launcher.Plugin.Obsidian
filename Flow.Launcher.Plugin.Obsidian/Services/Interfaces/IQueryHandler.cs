using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface IQueryHandler
{
    Task<List<Result>> HandleQueryAsync(QueryData queryData, CancellationToken cancellationToken);

    List<Result> HandleNoteCreation(QueryData queryData);
}
