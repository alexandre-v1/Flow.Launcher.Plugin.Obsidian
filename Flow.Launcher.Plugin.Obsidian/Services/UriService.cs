using System;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class UriService
{
    public static string GetOpenNoteUri(string vaultName, string relativePath)
    {
        string eVaultName = Uri.EscapeDataString(vaultName);
        string eRelativePath = Uri.EscapeDataString(relativePath);
        return $"obsidian://open?vault={eVaultName}&file={eRelativePath}";
    }

    public static string GetCreateNoteUri(string vaultName, string fileName)
    {
        string eVaultName = Uri.EscapeDataString(vaultName);
        string eFileName = Uri.EscapeDataString(fileName);
        return $"obsidian://new?vault={eVaultName}&name={eFileName}";
    }
}
