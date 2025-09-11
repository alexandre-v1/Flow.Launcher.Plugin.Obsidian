using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileInfo = Flow.Launcher.Plugin.Obsidian.Models.FileInfo;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public class FileDiscovery(string root, ISet<string> extensions, IList<string> relativeExcludedPaths)
{
    public IEnumerable<FileInfo> GetFiles()
    {
        EnumerationOptions options = new()
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            AttributesToSkip = 0,
            MatchType = MatchType.Win32
        };

        IEnumerable<FileInfo> query = Directory
            .EnumerateFiles(root, "*", options)
            .Select(FileInfo.Create)
            .Where(IsSearchable);


        return query;
    }

    private bool IsSearchable(FileInfo fileInfo)
    {
        if (!extensions.Contains(fileInfo.Extension))
        {
            return false;
        }

        return !IsExcluded(fileInfo.Path);
    }

    private bool IsExcluded(string path)
    {
        ReadOnlySpan<char> relativePath = path.AsSpan()[(root.Length + 1)..];

        for (int i = 0; i < relativeExcludedPaths.Count; i++)
        {
            if (relativePath.StartsWith(relativeExcludedPaths[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
