using System;
using System.Diagnostics;
using System.IO;

namespace Flow.Launcher.Plugin.Obsidian;

public class File
{
    private static readonly string ObsidianLogoPath = Path.Combine("Icons", "obsidian-logo.png");
    public readonly string Title;
    private readonly string _path;
    private readonly string _relativePath;
    private readonly Vault _vault;
    
    public File(Vault vault, string path)
    {
        _vault = vault;
        _path = path;
        Title = Path.GetFileNameWithoutExtension(path);
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
            ContextData = _vault,
            IcoPath = ObsidianLogoPath
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
