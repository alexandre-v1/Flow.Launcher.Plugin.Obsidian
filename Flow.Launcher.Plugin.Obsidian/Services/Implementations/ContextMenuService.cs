using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class ContextMenuService(Obsidian obsidian, IVaultManager vaultManager, Settings settings)
    : IContextMenu
{
    public List<Result> LoadContextMenus(Result selectedResult)
    {
        if (selectedResult is not File file)
        {
            return [];
        }

        string path = file.RelativePath;

        List<Result> results = [];
        Vault? vault = vaultManager.GetVaultWithId(file.VaultId);

        if (vault is null)
        {
            return results;
        }

        if (vault.HasAdvancedUri)
        {
            if (!vault.OpenInNewTabByDefault())
            {
                results.Add(OpenInNewTabResult(file));
            }
        }

        if (settings.DefaultQuery.AddCheckBoxesToContext)
        {
            results.AddRange(file.GetCheckBoxes());
        }

        if (
            settings.DefaultQuery.AddGlobalFolderExcludeToContext
            || settings.DefaultQuery.AddLocalFolderExcludeToContext
        )
        {
            results.AddRange(ExcludeResults(path, file.VaultId));
        }

        return results;
    }

    private static Result OpenInNewTabResult(File file) =>
        new()
        {
            Title = "Open in new tab",
            Glyph = new GlyphInfo(Font.Family, Font.OpenInNewTabGlyph),
            Action = _ =>
            {
                file.Open(true);
                return true;
            }
        };

    private List<Result> ExcludeResults(string relativePath, string? vaultId)
    {
        List<Result> results = [];
        string[] parts = relativePath.Split('\\');
        for (int i = 0; i < parts.Length - 1; i++)
        {
            string directory = string.Join("\\", parts.Take(i + 1));
            if (settings.DefaultQuery.AddLocalFolderExcludeToContext)
            {
                results.Add(ExcludeLocalFolderResult(directory, vaultId));
            }
        }

        return results;
    }

    private Result ExcludeLocalFolderResult(string folder, string? vaultId) =>
        new()
        {
            Title = $"Exclude {folder} folder locally",
            Action = _ => TryToExcludeLocalFolder(folder, vaultId),
            Glyph = new GlyphInfo(Font.Family, Font.ExcludeGlyph)
        };

    private bool TryToExcludeLocalFolder(string folder, string? vaultId)
    {
        if (vaultId is null)
        {
            return false;
        }

        Vault? vault = vaultManager.GetVaultWithId(vaultId);
        if (vault is null)
        {
            return false;
        }

        vault.Setting.RelativeExcludePaths.Add(folder);
        _ = obsidian.ReloadDataAsync();
        return true;
    }
}
