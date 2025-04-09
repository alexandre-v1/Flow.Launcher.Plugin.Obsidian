using System;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class NormalizationUtility
{
    public static int NormalizeToInt(int value, int max, int weight) =>
        (int)Math.Round(Normalize(value, max) * weight);

    private static double Normalize(int value, int max)
    {
        if (max <= 0) return 0;
        return (double)value / max;
    }
}
