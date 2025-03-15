using System;
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class ObsidianUriGenerator
{
    public static string CreateOpenFileUri(string vaultId, string relativePath)
    {
        vaultId = Uri.EscapeDataString(vaultId);
        relativePath = Uri.EscapeDataString(relativePath);
        return $"obsidian://open?vault={vaultId}&file={relativePath}";
    }

    public static string CreateNewNoteUri(string vaultId, string fileName)
    {
        vaultId = Uri.EscapeDataString(vaultId);
        fileName = Uri.EscapeDataString(fileName);
        return $"obsidian://new?vault={vaultId}&name={fileName}";
    }

    public static string CreateOpenInNewTabUri(string vaultId, string relativePath)
    {
        vaultId = Uri.EscapeDataString(vaultId);
        relativePath = Uri.EscapeDataString(relativePath);
        return $"obsidian://adv-uri?vault={vaultId}&filepath={relativePath}&openmode=true";
    }

    public static string CreateTaggedNoteUri(string vaultId, string fileName, IReadOnlySet<string> tags)
    {
        vaultId = Uri.EscapeDataString(vaultId);
        fileName = Uri.EscapeDataString(fileName);
        string yamlContent = Uri.EscapeDataString(BuildYamlFrontMatter(tags));
        return $"obsidian://new?vault={vaultId}&name={fileName}&content={yamlContent}";
    }

    private static string BuildYamlFrontMatter(IReadOnlySet<string> tags)
    {
        const string yamlDelimiter = "---";
        string tagsList = string.Join("\n  - ", tags);

        return $"{yamlDelimiter}\n" +
               $"tags:\n" +
               $"  - {tagsList}\n" +
               $"{yamlDelimiter}\n";
    }
}
