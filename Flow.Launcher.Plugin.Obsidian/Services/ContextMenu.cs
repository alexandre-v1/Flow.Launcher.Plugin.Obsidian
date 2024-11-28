using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class ContextMenu : IContextMenu
{
    private readonly Obsidian _obsidian;
    private readonly Settings _settings;
    
    public ContextMenu(Obsidian obsidian, Settings settings)
    {
        _obsidian = obsidian;
        _settings = settings;
    }
    
    public List<Result> LoadContextMenus(Result selectedResult)
    {
        if (selectedResult is not File file) return new List<Result>();
        string path = file.RelativePath;
        
        var results = new List<Result>();
        if (_settings.AddCheckBoxesToContext)
            results.AddRange(file.GetCheckBoxes());
        if (_settings.AddGlobalFolderExcludeToContext || _settings.AddLocalFolderExcludeToContext)
            results.AddRange(ExcludeResults(path, (string)file.ContextData));
        return results;
    }

    private List<Result> ExcludeResults(string relativePath, string? vaultId)
    {
        var results = new List<Result>();
        string[] parts = relativePath.Split('\\');
        for (int i = 0; i < parts.Length - 1; i++)
        {
            string directory = string.Join("\\", parts.Take(i + 1));
            if (_settings.AddGlobalFolderExcludeToContext)
                results.Add(ExcludeGlobalFolderResult(directory));
            if (_settings.AddLocalFolderExcludeToContext)
                results.Add(ExcludeLocalFolderResult(directory, vaultId));
        }
        return results;
    }

    private Result ExcludeGlobalFolderResult(string folder)
    {
        return new Result
        {
            Title = $"Exclude {folder} folder globally",
            Action = _ => ExcludeGlobalFolder(folder)
        };
    }

    private Result ExcludeLocalFolderResult(string folder, string? vaultId)
    {
        return new Result
        {
            Title = $"Exclude {folder} folder locally",
            Action = _ => TryToExcludeLocalFolder(folder, vaultId)
        };
    }
    
    private bool ExcludeGlobalFolder(string folder)
    {
        _settings.GlobalVaultSetting.ExcludedPaths.Add(folder);
        _obsidian.ReloadData();
        return true;
    }

    private bool TryToExcludeLocalFolder(string folder, string? vaultId)
    {
        if (vaultId is null) return false;
        Vault? vault = VaultManager.GetVault(vaultId);
        if (vault is null) return false;
        vault.VaultSetting.ExcludedPaths.Add(folder);
        _obsidian.ReloadData();
        return true;
    }
}
