using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FileExtensionsSetting
{
    // Extensions who are not in a group
    [JsonInclude]
    public HashSet<FileExtension> Extensions { get; set; } = DefaultExtensions;

    [JsonInclude]
    public HashSet<FileExtensionGroup> ExtensionGroups { get; set; } = DefaultExtensionGroups;

    private static HashSet<FileExtensionGroup> DefaultExtensionGroups { get; } =
    [
        new(
            "Image",
            [
                new FileExtension("PNG", ".png"),
                new FileExtension("JPEG", ".jpeg"),
                new FileExtension("JPEG", ".jpg"),
                new FileExtension("GIF", ".gif"),
                new FileExtension("Windows bitmap", ".bmp")
            ]
        ),
        new("Video", [new FileExtension("MP4", ".mp4")])
    ];

    // Extensions who are not in a group
    private static HashSet<FileExtension> DefaultExtensions { get; } =
        [new("Markdown", ".md"), new("Excalidraw", ".excalidraw"), new("Canvas", ".canvas")];

    public IEnumerable<FileExtension> GetActiveExtensions() =>
        Extensions.Where(extension => extension.IsActive)
            .Concat(ExtensionGroups.Where(group => group.IsActive)
                .SelectMany(group => group.Extensions).Where(extension => extension.IsActive));

    public IEnumerable<string> GetActiveExtensionSuffix() =>
        GetActiveExtensions().Select(extension => extension.Suffix);

    public bool Contains(string extensionSuffix)
    {
        return GetActiveExtensionSuffix().Any(suffix => suffix == extensionSuffix);
    }
}
