using System;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class ObsidianQueryInfo(Type type, string displayName, string description, GlyphInfo glyph)
{
    public Type QueryType => type;
    public string DisplayName => displayName;
    public string Description => description;
    public GlyphInfo Glyph => glyph;
}
