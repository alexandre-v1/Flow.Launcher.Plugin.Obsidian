using System;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.Obsidian;

public class File
{
    public readonly string Title;
    private readonly string _path;
    private readonly string _relativePath;
    private readonly Vault _vault;
    
    public File(Vault vault, string path)
    {
        _vault = vault;
        _path = path;
        Title = System.IO.Path.GetFileNameWithoutExtension(path);
        _relativePath = path.Replace(_vault.Path, "");
    }

    public Result ToResult()
    {
        var result = new Result
        {
            Title = Title,
            SubTitle = _path,
            Action = c =>
            {
                OpenNote();
                return true;
            },
            ContextData = _vault
        };
        return result;
    }

    private void OpenNote()
    {
        string encodedVault = Uri.EscapeDataString(_vault.Name);
        string encodedPath = Uri.EscapeDataString(_relativePath);
        
        string uri = $"obsidian://open?vault={encodedVault}&file={encodedPath}";
        
        Process.Start(new ProcessStartInfo
        {
            FileName = uri,
            UseShellExecute = true
        });
    }
}
