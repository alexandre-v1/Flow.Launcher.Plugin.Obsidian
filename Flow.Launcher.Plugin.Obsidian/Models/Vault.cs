using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Vault
{
    public readonly string Id;
    public readonly string Name;
    public readonly string VaultPath;
    
    public readonly VaultSetting VaultSetting;
    public List<File> Files = new();

    public Vault(string id, string vaultPath, VaultSetting vaultSetting)
    {
        Id = id;
        VaultPath = vaultPath;
        VaultSetting = vaultSetting;
        Name = Path.GetFileName(VaultPath);
    }

    public List<File> GetMdFiles()
    {
        var files = new List<File>();
        var directories = new Stack<string>();
        directories.Push(VaultPath);

        while (directories.Count > 0)
        {
            string currentDir = directories.Pop();
            try
            {
                files.AddRange(Directory.GetFiles(currentDir, "*.md")
                    .Select(file => new File(this, file)));
                
                foreach (string dir in Directory.GetDirectories(currentDir))
                {
                    directories.Push(dir);
                }
            }
            catch (UnauthorizedAccessException) 
            {
                // Skip directories we can't access
            }
            catch (PathTooLongException)
            {
                // Skip paths that are too long
            }
        }
        return files;
    }

}
