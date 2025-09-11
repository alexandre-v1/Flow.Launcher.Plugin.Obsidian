using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class File : Result
{
    private readonly FileInfo _info;

    public readonly string RelativePath;
    public readonly string VaultId;

    public File(Vault vault, FileInfo fileInfo)
    {
        VaultId = vault.Id;
        _info = fileInfo;
        RelativePath = Path.GetRelativePath(vault.Path, fileInfo.Path);
        SubTitle = Path.Combine(vault.Name, RelativePath);
        CopyText = FilePath;
        Action = _ =>
        {
            Open(vault.OpenInNewTabByDefault());
            return true;
        };
        Icon = IconCache.GetCachedIconDelegate(Paths.ObsidianLogo);
        Score = 100;
        if (Extension is ".md")
        {
            LoadObsidianProperties();
        }
    }

    public string Extension => _info.Extension;
    public string FilePath => _info.Path;
    public string Name => _info.Name;
    public string FileName => _info.FileName;

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
