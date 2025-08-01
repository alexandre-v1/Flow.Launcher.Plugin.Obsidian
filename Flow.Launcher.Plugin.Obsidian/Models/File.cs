using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class File : Result
{
    public readonly string Extension;
    public readonly string FilePath;
    public readonly string RelativePath;
    public readonly string VaultId;

    public File(Vault vault, string path)
    {
        FilePath = path;
        VaultId = vault.Id;
        Extension = Path.GetExtension(path);
        Name = Path.GetFileNameWithoutExtension(path);
        RelativePath = path.Remove(vault.Path).TrimStart('\\');
        SubTitle = Path.Combine(vault.Name, RelativePath);
        CopyText = FilePath;
        Action = _ =>
        {
            Open(vault.OpenInNewTabByDefault());
            return true;
        };
        Icon = IconCache.GetCachedIconDelegate(Paths.ObsidianLogo);
        Score = 100;
    }

    public string Name
    {
        get => Title;
        set => Title = value;
    }

    public HashSet<string>? Aliases { get; set; }
    public HashSet<string>? Tags { get; set; }

    public File LoadObsidianProperties() =>
        ObsidianProperties.LoadObsidianProperties(this);

    public void Open(bool openInNewTab = false)
    {
        string uri = ObsidianUriGenerator.GenerateOpenFileUri(VaultId, RelativePath, openInNewTab);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }

    public bool HasTags(HashSet<string> tagsToCheck) => Tags is not null && tagsToCheck.IsSubsetOf(Tags);

    public bool CanSearchContent() => Path.GetExtension(FilePath) is ".md" && !HasTag("excalidraw");

    private bool HasTag(string tag) => Tags is not null && Tags.Contains(tag);
}
