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
                new FileExtension("AVIF", ".avif"),
                new FileExtension("Windows bitmap", ".bmp"),
                new FileExtension("GIF", ".gif"),
                new FileExtension("JPEG", ".jpeg"),
                new FileExtension("JPEG", ".jpg"),
                new FileExtension("PNG", ".png"),
                new FileExtension("Scalable Vector Graphics", ".svg"),
                new FileExtension("WebP", ".webp")
            ]
        ),
        new(
            "Audio",
            [
                new FileExtension("FLAC", ".flac"),
                new FileExtension("MPEG-4 Audio", ".m4a"),
                new FileExtension("MP3", ".mp3"),
                new FileExtension("Ogg", ".ogg"),
                new FileExtension("Waveform", ".wav"),
                new FileExtension("3GP", ".3gp")
            ],
            false
        ),

        new("Video",
            [
                new FileExtension("Matroska", ".mkv"),
                new FileExtension("QuickTime Movie", ".mov"),
                new FileExtension("MP4", ".mp4"),
                new FileExtension("Theora", ".ogv"),
                new FileExtension("WebM", ".webm")
            ],
            false
        )
    ];

    // Extensions who are not in a group
    private static HashSet<FileExtension> DefaultExtensions { get; } =
    [
        new("Markdown", ".md"),
        new("Bases", ".base"),
        new("Canvas", ".canvas"),
        new("Excalidraw", ".excalidraw"),
        new("PDF", ".pdf")
    ];

    public IEnumerable<FileExtension> GetActiveExtensions() =>
        Extensions.Where(extension => extension.IsActive)
            .Concat(ExtensionGroups.Where(group => group.IsActive)
                .SelectMany(group => group.Extensions).Where(extension => extension.IsActive));

    public IEnumerable<string> GetActiveExtensionSuffix() =>
        GetActiveExtensions().Select(extension => extension.Suffix);

    public bool Contains(string extensionSuffix) => GetActiveExtensionSuffix().Any(suffix => suffix == extensionSuffix);
}
