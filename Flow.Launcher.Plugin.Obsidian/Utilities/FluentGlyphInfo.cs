namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public record FluentGlyphInfo(string Glyph) : GlyphInfo(Utilities.Glyph.Family, Glyph);
