using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class GlobalVaultSetting
{
    private static readonly List<string> _defaultsExcludedPaths = new() { ".trash", ".obsidian" };

    public ObservableCollection<string> ExcludedPaths { get; } = new(_defaultsExcludedPaths);

    public bool OpenInNewTabByDefault { get; set; } = true;
    public bool SearchMarkdown { get; set; } = true;
    public bool SearchCanvas { get; set; } = true;
    public bool SearchImages { get; set; }
    public bool SearchExcalidraw { get; set; } = true;
    public bool SearchOther { get; set; }
    public bool SearchContent { get; set; }


    public virtual HashSet<string> GetSearchableExtensions(Settings settings)
    {
        HashSet<string> searchPattern = [];
        if (SearchMarkdown)
        {
            searchPattern.Add(".md");
        }

        if (SearchCanvas)
        {
            searchPattern.Add(".canvas");
        }

        if (SearchImages)
        {
            searchPattern.Add(".png");
            searchPattern.Add(".jpg");
            searchPattern.Add(".jpeg");
            searchPattern.Add(".gif");
            searchPattern.Add(".bmp");
        }

        if (SearchExcalidraw)
        {
            searchPattern.Add(".excalidraw");
        }

        if (SearchOther)
        {
            searchPattern.Add(".json");
            searchPattern.Add(".csv");
        }

        return searchPattern;
    }

    public virtual List<string> GetExcludedPaths(Settings settings) => new(ExcludedPaths);
}
