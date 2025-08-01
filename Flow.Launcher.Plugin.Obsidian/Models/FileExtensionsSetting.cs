// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// Keep setters to allow JSON deserialization

using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FileExtensionsSetting
{
    public HashSet<FileExtensionGroup> ExtensionGroups { get; set; } = DefaultExtensionGroups;

    // Extensions who are not in a group
    public HashSet<FileExtension> Extensions { get; set; } = DefaultExtensions;

    public static HashSet<FileExtensionGroup> DefaultExtensionGroups { get; } =
        [
            new(
                "Image",
                [
                    new FileExtension("PNG", ".png"),
                    new FileExtension("JPEG", ".jpeg"),
                    new FileExtension("JPEG", ".jpg"),
                    new FileExtension("GIF", ".gif"),
                    new FileExtension("Windows bitmap", ".bmp"),
                ]
            ),
            new("Video", [new FileExtension("MP4", ".mp4")]),
        ];

    // Extensions who are not in a group
    public static HashSet<FileExtension> DefaultExtensions { get; } =
        [new("Markdown", ".md"), new("Excalidraw", ".excalidraw"), new("Canvas", ".canvas")];

    public ISet<string> GetAllSuffix()
    {
        HashSet<string> result = [];

        foreach (FileExtension fileExtension in Extensions)
        {
            result.Add(fileExtension.Suffix);
        }

        foreach (
            FileExtension fileExtension in ExtensionGroups.SelectMany(group => group.FileExtensions)
        )
        {
            result.Add(fileExtension.Suffix);
        }

        return result;
    }
}
