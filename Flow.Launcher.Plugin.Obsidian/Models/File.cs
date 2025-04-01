using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class File : Result
{
    public readonly string Name;
    public readonly string FilePath;
    public readonly string RelativePath;
    public readonly string VaultId;
    public HashSet<string>? Aliases { get; set; }
    public HashSet<string>? Tags { get; set; }

    public File(Vault vault, string path)
    {
        Name = Path.GetFileNameWithoutExtension(path);
        Title = Name;
        FilePath = path;
        RelativePath = path.Remove(vault.VaultPath).TrimStart('\\');
        SubTitle = Path.Combine(vault.Name, RelativePath);
        CopyText = FilePath;
        Action = _ =>
        {
            Open(vault.OpenInNewTabByDefault());
            return true;
        };
        VaultId = vault.Id;
        IcoPath = Paths.ObsidianLogo;
        Score = 100;
    }

    public File AddObsidianProperties(bool useAliases, bool useTags) =>
        ObsidianPropertiesHelper.AddObsidianProperties(this, useAliases, useTags);

    public void Open(bool openInNewTab = false)
    {
        string uri = ObsidianUriGenerator.GenerateOpenFileUri(VaultId, RelativePath, openInNewTab);
        Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
    }

    public bool HasTags(HashSet<string> tagsToCheck) => Tags is not null && tagsToCheck.IsSubsetOf(Tags);

    public bool HasTag(string tag) => Tags is not null && Tags.Contains(tag);
}
