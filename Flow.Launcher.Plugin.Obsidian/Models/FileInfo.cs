// ReSharper disable CommentTypo

using System;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public record FileInfo(string Path, string Name, string Extension, string FileName)
{
    public static FileInfo Create(string fullPath)
    {
        ReadOnlySpan<char> fullPathSpan = fullPath.AsSpan();
        ReadOnlySpan<char> fileNameSpan = System.IO.Path.GetFileName(fullPathSpan);
        ReadOnlySpan<char> extSpan = System.IO.Path.GetExtension(fileNameSpan);
        ReadOnlySpan<char> nameSpan = fileNameSpan[..^extSpan.Length];

        string fileName = fileNameSpan.ToString();
        string extension = extSpan.ToString();

        // Use extension for file with no name like ".stignore"
        string name = nameSpan.IsEmpty ? extension : nameSpan.ToString();

        return new FileInfo(fullPath, name, extension, fileName);
    }
}
