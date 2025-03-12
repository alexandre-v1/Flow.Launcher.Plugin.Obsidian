using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class File : Result
{
    public readonly string Name;
    public readonly string FilePath;
    public readonly string Extension;
    public readonly string RelativePath;
    public readonly string VaultId;
    public string[]? Aliases { get; set; }
    public string[]? Tags { get; set; }

    public File(Vault vault, string path)
    {
        Name = Path.GetFileNameWithoutExtension(path);
        FilePath = path;
        Extension = Path.GetExtension(path);
        RelativePath = path.Replace(vault.VaultPath, "").TrimStart('\\');
        SubTitle = Path.Combine(vault.Name, RelativePath);
        CopyText = FilePath;
        Action = _ =>
        {
            if (vault.OpenNoteInNewTabByDefault(vault.VaultSetting))
            {
                OpenNoteInNewTab();
            }
            else
            {
                OpenNote();
            }

            return true;
        };
        VaultId = vault.Id;
        IcoPath = Paths.ObsidianLogo;
    }

    public void OpenNoteInNewTab()
    {
        Vault? vault = VaultManager.GetVaultWithId(VaultId);
        if (vault is null) return;

        string uri = UriService.GetOpenNoteInNewTabUri(vault.Name, RelativePath);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }

    public Vault? GetVault() => VaultManager.GetVaultWithId(VaultId);

    private void OpenNote()
    {
        Vault? vault = VaultManager.GetVaultWithId(VaultId);
        if (vault == null) return;

        string uri = UriService.GetOpenNoteUri(vault.Name, RelativePath);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }
}
