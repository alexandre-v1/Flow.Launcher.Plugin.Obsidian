using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class GlobalVaultSetting
{
    public bool SearchMarkdown { get; set; } = true;
    public bool SearchCanvas { get; set; } = true;
    public bool SearchImages { get; set; }
    public bool SearchExcalidraw { get; set; } = true;
    public bool SearchOther { get; set; }
    public bool SearchContent { get; set; }
    public List<string> ExcludedPaths { get; set; } = new();
}
