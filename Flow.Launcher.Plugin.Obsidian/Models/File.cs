using System;
using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class File : Result
{
    private static readonly string ObsidianLogoPath = Path.Combine("Icons", "obsidian-logo.png");
    private readonly string _relativePath;
    public string Name { get; }
    public string Extension { get; }
    public string[]? Aliases { get; private set; }
    
    public File(Vault vault, string path, bool useExtension , string[]? alias)
    {
        Name = Path.GetFileNameWithoutExtension(path);
        Extension = Path.GetExtension(path);
        Title = useExtension ? Name + Extension : Name;
        _relativePath = path.Replace(vault.VaultPath, "").TrimStart('\\');
        SubTitle = Path.Combine(vault.Name, _relativePath);
        CopyText = Path.Combine(vault.VaultPath, _relativePath);
        Action = _ =>
        {
            OpenNote();
            return true;
        };
        ContextData = vault.Id;
        IcoPath = ObsidianLogoPath;
        Aliases = alias;
    }

    private void OpenNote()
    {
        string vaultId = (string)ContextData;
        Vault? vault = VaultManager.GetVault(vaultId);
        if (vault == null) return;
        
        string encodedVault = Uri.EscapeDataString(vault.Name);
        string encodedPath = Uri.EscapeDataString(_relativePath);
        
        string uri = $"obsidian://open?vault={encodedVault}&file={encodedPath}";
        
        Process.Start(new ProcessStartInfo
        {
            FileName = uri,
            UseShellExecute = true
        });
    }
}
