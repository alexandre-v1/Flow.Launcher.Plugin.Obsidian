namespace Flow.Launcher.Plugin.Obsidian.Models;

public class ObsidianPlugin(string rawName)
{
    public string? Name { get; set; }
    public string RawName { get; set; } = rawName;
}
