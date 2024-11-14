using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flow.Launcher.Plugin.Obsidian;

public class Vault
{
    public string Id;
    public string Name  => System.IO.Path.GetFileName(Path);
    public readonly string Path;
    
    public VaultSetting? Setting;
    public List<File> Files = new();

    public Vault(string id, string path)
    {
        Id = id;
        Path = path;
    }

    public List<File> GetMdFiles()
    {
        var files = new List<File>();
        var directories = new Stack<string>();
        directories.Push(Path);

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
