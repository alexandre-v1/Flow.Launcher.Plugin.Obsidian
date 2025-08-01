using System.Collections.Generic;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface INoteCreatorService
{
    Result BuildSingleVaultNoteCreationResult(QueryData queryData);

    List<Result> BuildMultiVaultNoteCreationResults(QueryData queryData);
}
