using System;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class ObsidianUriGenerator
{
    public static string GenerateOpenFileUri(string vaultId, string relativePath, bool openInNewTab = false)
    {
        vaultId = Uri.EscapeDataString(vaultId);
        relativePath = Uri.EscapeDataString(relativePath);
        return openInNewTab
            ? $"obsidian://adv-uri?vault={vaultId}&filepath={relativePath}&openmode=true"
            : $"obsidian://open?vault={vaultId}&file={relativePath}";
    }

    public static string GenerateNewFileUri(string vaultId, string fileName, string content, bool openInNewTab = false)
    {
        vaultId = Uri.EscapeDataString(vaultId);
        fileName = Uri.EscapeDataString(fileName);
        content = Uri.EscapeDataString(content);
        return openInNewTab
            ? $"obsidian://adv-uri?vault={vaultId}&filepath={fileName}&content={content}&openmode=true"
            : $"obsidian://new?vault={vaultId}&name={fileName}&content={content}";
    }
}
