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
    public readonly string[]? Aliases;
    public readonly string VaultId;

    public File(Vault vault, string path, string[]? alias)
    {
        Name = Path.GetFileNameWithoutExtension(path);
        FilePath = path;
        Extension = Path.GetExtension(path);
        RelativePath = path.Replace(vault.VaultPath, "").TrimStart('\\');
        SubTitle = Path.Combine(vault.Name, RelativePath);
        CopyText = FilePath;
        Action = _ =>
        {
            OpenNote();
            return true;
        };
        VaultId = vault.Id;
        IcoPath = Paths.ObsidianLogo;
        Aliases = alias;
    }

    private void OpenNote()
    {
        Vault? vault = VaultManager.GetVaultWithId(VaultId);
        if (vault == null) return;

        string uri = UriService.GetOpenNoteUri(vault.Name, RelativePath);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }
}
