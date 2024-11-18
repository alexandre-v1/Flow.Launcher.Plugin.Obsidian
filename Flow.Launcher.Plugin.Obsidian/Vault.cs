using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flow.Launcher.Plugin.Obsidian;

public class Vault
{
    public string Id;
    public readonly string VaultPath;
    public string Name;
    
    public readonly VaultSetting Setting;
    public List<File> Files = new();

    public Vault(string id, string vaultPath, VaultSetting? setting = null)
    {
        Id = id;
        VaultPath = vaultPath;
        Setting = setting ?? new VaultSetting();
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
