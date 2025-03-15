using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class NoteCreatorService
{
    public const string NoteCreatorKeyword = "create";

    private readonly VaultManager _vaultManager;
    private readonly TagSearchService _tagSearchService;
    private readonly IPublicAPI _publicApi;

    public NoteCreatorService(VaultManager vaultManager, TagSearchService tagSearchService, IPublicAPI publicApi)
    {
        _vaultManager = vaultManager;
        _tagSearchService = tagSearchService;
        _publicApi = publicApi;
    }

    public List<Result> BuildNoteCreationResults(Query query)
    {
        List<Result> results;
        TagQuery tagQuery = new(query, _vaultManager);

        if (!tagQuery.HasValidOrInvalidTags)
        {
            results = _vaultManager.Vaults
                .Select(vault => CreateVaultSpecificNoteResult(vault, query.SecondToEndSearch))
                .ToList();
            return results;
        }

        if (!tagQuery.HasInvalidTags)
        {
            results = _vaultManager.Vaults.Select(vault =>
                CreateVaultSpecificTaggedNoteResult(vault, tagQuery.SecondToEndSearchWithoutTags,
                    tagQuery.Tags)).ToList();
            return results;
        }

        results = _tagSearchService.FindMatchingTags(tagQuery.GetInvalidTag(), query.ActionKeyword);

        string queryString = string.Empty;
        if (!string.IsNullOrEmpty(query.ActionKeyword))
        {
            queryString = $"{query.ActionKeyword} ";
        }

        queryString += $"{tagQuery.SearchWithoutInvalidTags} ";

        foreach (Result result in results)
        {
            result.Action = _ =>
            {
                _publicApi.ChangeQuery(queryString += $"{result.Title} ");
                return false;
            };
        }

        return results;
    }

    public Result CreateNewNoteResult(string actionKeyword, string search)
    {
        if (string.IsNullOrEmpty(search)) search = "Untitled";
        return new Result
        {
            Title = $"Create new note \"{search}\"",
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                if (_vaultManager.HasOnlyOneVault)
                {
                    LaunchNoteCreation(_vaultManager.Vaults[0], search);
                    return true;
                }

                _publicApi.ChangeQuery($"{actionKeyword} {NoteCreatorKeyword} {search}", true);
                return false;
            }
        };
    }

    public Result CreateTaggedNoteResult(string actionKeyword, string noteName, string tag)
    {
        if (string.IsNullOrEmpty(noteName)) noteName = "Untitled";
        return new Result
        {
            Title = $"Create new note \"{noteName}\" with tag #{tag}",
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                if (_vaultManager.HasOnlyOneVault)
                {
                    LaunchTaggedNoteCreation(_vaultManager.Vaults[0], noteName, new HashSet<string> { tag });
                    return true;
                }

                _publicApi.ChangeQuery($"{actionKeyword} {NoteCreatorKeyword} {noteName} #{tag}", true);
                return false;
            }
        };
    }

    private static void LaunchTaggedNoteCreation(Vault vault, string noteName, IReadOnlySet<string> tags)
    {
        string uri = ObsidianUriGenerator.CreateTaggedNoteUri(vault.Name, noteName, tags);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }

    private static Result CreateVaultSpecificNoteResult(Vault vault, string search)
    {
        if (string.IsNullOrEmpty(search)) search = "Untitled";
        return new Result
        {
            Title = $"Create new note \"{search}\" in {vault.Name}",
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                LaunchNoteCreation(vault, search);
                return true;
            }
        };
    }

    private static void LaunchNoteCreation(Vault vault, string noteName)
    {
        string uri = ObsidianUriGenerator.CreateNewNoteUri(vault.Name, noteName);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }

    private static Result CreateVaultSpecificTaggedNoteResult(Vault vault, string noteName, IReadOnlySet<string> tags)
    {
        if (string.IsNullOrEmpty(noteName)) noteName = "Untitled";
        string title = $"Create new note \"{noteName}\" in {vault.Name} ";
        if (tags.Count is 1)
        {
            title += $"with tag #{tags.First()} ";
        }
        else
        {
            title += "with tags ";
            for (int i = 0; i < tags.Count; i++)
            {
                if (i == tags.Count - 1)
                {
                    title += $"and #{tags.ElementAt(i)} ";
                    break;
                }

                title += $"#{tags.ElementAt(i)} ";
            }
        }

        return new Result
        {
            Title = title,
            IcoPath = Paths.ObsidianLogo,
            Action = _ =>
            {
                LaunchTaggedNoteCreation(vault, noteName, tags);
                return true;
            }
        };
    }
}
