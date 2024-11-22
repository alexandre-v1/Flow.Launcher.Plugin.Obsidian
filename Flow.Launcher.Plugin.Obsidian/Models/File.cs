using System;
using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class File : Result
{
    private static readonly string ObsidianLogoPath = Path.Combine("Icons", "obsidian-logo.png");
    private readonly string _relativePath;
    
    public File(Vault vault, string path)
    {
        Title = Path.GetFileNameWithoutExtension(path);
        _relativePath = path.Replace(vault.VaultPath, "").TrimStart('\\');
        SubTitle = Path.Combine(vault.Name, _relativePath);
        Action = c =>
        {
            OpenNote();
            return true;
        };
        ContextData = vault.Id;
        IcoPath = ObsidianLogoPath;
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
