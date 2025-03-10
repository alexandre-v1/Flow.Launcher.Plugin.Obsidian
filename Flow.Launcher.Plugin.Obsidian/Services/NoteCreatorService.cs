using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class NoteCreatorService
{
    private static string NoteCreatorKeyword { get; set; } = "create";

    public static Result CreateNewNoteResult(Query query, IPublicAPI publicApi)
    {
        string search = query.Search;
        return new Result
        {
            Title = $"Create new note \"{search}\"",
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                if (VaultManager.HasOnlyOneVault)
                {
                    CreateNewNote(VaultManager.Vaults[0], search);
                    return true;
                }

                publicApi.ChangeQuery($"{query.ActionKeyword} {NoteCreatorKeyword} {search}", true);
                return false;
            }
        };
    }

    public static IEnumerable<Result> CreateNewNoteResultsWithVaults(Query query)
    {
        string search = RemoveNoteCreatorKeyword(query.Search);
        var select = VaultManager.Vaults.Select(
            vault => CreateNewNoteResultWithVault(search, vault));
        return select;
    }

    private static string RemoveNoteCreatorKeyword(string search)
    {
        return search.Replace(NoteCreatorKeyword, "").Trim();
    }

    private static Result CreateNewNoteResultWithVault(string search, Vault vault)
    {
        return new Result
        {
            Title = $"Create new note \"{search}\" in {vault.Name}",
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                CreateNewNote(vault, search);
                return true;
            }
        };
    }

    private static void CreateNewNote(Vault vault, string noteName)
    {
        string uri = UriService.GetCreateNoteUri(vault.Name, noteName);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }

    public static bool IsCreateNewNoteQuery(string search)
    {
        return search.StartsWith(NoteCreatorKeyword);
    }
}
