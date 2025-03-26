using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class NoteCreatorService(IPublicAPI publicApi)
{
    public Result BuildSingleVaultNoteCreationResult(QueryData queryData)
    {
        string noteName = GetNoteName(queryData);
        string title = $"Create new note \"{noteName}\"";

        if (queryData.HasOnlyOneVault())
        {
            title += $" in {queryData.GetTheOnlyVault()?.Name}";
        }

        if (queryData.HasValidTags)
        {
            title += $" {FormatTagsForDisplay(queryData.ValidTags)}";
        }

        return BuildNoteCreationResult(title, queryData, noteName);
    }

    public List<Result> BuildMultiVaultNoteCreationResults(QueryData queryData)
    {
        string noteName = GetNoteName(queryData);
        List<Result> results = [];

        foreach (Vault vault in queryData.Vaults)
        {
            string title = $"Create new note \"{noteName}\" in {vault.Name} ";

            if (queryData.HasValidTags)
            {
                title += FormatTagsForDisplay(queryData.ValidTags);
            }

            results.Add(BuildNoteCreationResult(title, queryData, noteName, vault));
        }

        return results;
    }

    private Result BuildNoteCreationResult(string title, QueryData queryData, string noteName, Vault? vault = null) =>
        new()
        {
            Title = title,
            IcoPath = Paths.ObsidianLogo,
            Action = _ => CreateNoteActionHandler(queryData, noteName, vault)
        };

    private bool CreateNoteActionHandler(QueryData queryData, string noteName, Vault? vault = null)
    {
        vault ??= queryData.GetTheOnlyVault();
        if (vault is null)
        {
            string newQuery = queryData.GetRawQueryWithAPrefix(Keyword.NoteCreator);
            publicApi.ChangeQuery(newQuery, true);
            return false;
        }

        string content = string.Empty;
        if (queryData.HasValidTags)
            content = ObsidianPropertiesHelper.BuildYamlFrontMatterWithTags(queryData.ValidTags);

        string uri =
            ObsidianUriGenerator.GenerateNewFileUri(vault.Id, noteName, content, vault.OpenInNewTabByDefault());
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
        return true;
    }

    private string GetNoteName(QueryData queryData)
    {
        string noteName = queryData.GetCleanSearchTerms().Without(Keyword.NoteCreator).JoinToString();
        return string.IsNullOrEmpty(noteName) ? "Untitled" : noteName;
    }

    private string FormatTagsForDisplay(ICollection<string> tags)
    {
        switch (tags.Count)
        {
            case 0:
                return string.Empty;
            case 1:
                return $"with tag #{tags.First()}";
        }

        string result = "with tags ";

        for (int i = 0; i < tags.Count; i++)
        {
            if (i == tags.Count - 1)
            {
                result += $"and #{tags.ElementAt(i)} ";
                break;
            }

            result += $"#{tags.ElementAt(i)} ";
        }

        return result;
    }
}
