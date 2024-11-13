using System;
using System.Collections.Generic;
using System.IO;

namespace Flow.Launcher.Plugin.Obsidian;

public class Vault
{
    public string ID;
    public string? VaultPath;
    public VaultSetting? Setting;
    public string? Name  => Path.GetFileName(VaultPath);
    public List<string>? Files;

    public Vault(string id, string? vaultPath)
    {
        ID = id;
        VaultPath = vaultPath;
    }

    public static List<string> GetMdFiles(string rootPath)
    {
        var files = new List<string>();
        var directories = new Stack<string>();
        directories.Push(rootPath);

        while (directories.Count > 0)
        {
            string currentDir = directories.Pop();
            try
            {
                // Add all .md files in current directory
                files.AddRange(Directory.GetFiles(currentDir, "*.md"));
            
                // Add subdirectories to stack
                foreach (string dir in Directory.GetDirectories(currentDir))
                {
                    directories.Push(dir);
                }
            }
            catch (UnauthorizedAccessException) 
            {
                // Skip directories we can't access
                continue;
            }
            catch (PathTooLongException)
            {
                // Skip paths that are too long
                continue;
            }
        }
        return files;
    }

}
