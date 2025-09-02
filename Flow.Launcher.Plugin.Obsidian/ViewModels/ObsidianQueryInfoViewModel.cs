using System;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class ObsidianQueryInfoViewModel(ObsidianQueryInfo queryInfo) : BaseModel
{
    public Type QueryType => queryInfo.QueryType;
    public string DisplayName => queryInfo.DisplayName;
    public string Description => queryInfo.Description;
    public GlyphInfo Glyph => queryInfo.Glyph;
}
