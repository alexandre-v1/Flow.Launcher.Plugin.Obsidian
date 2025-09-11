using System.Windows;
using System.Windows.Media;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class SharedProperties
{
    public static readonly DependencyProperty BorderBackgroundProperty =
        DependencyProperty.RegisterAttached(
            "BorderBackground",
            typeof(Brush),
            typeof(SharedProperties),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static void SetBorderBackground(DependencyObject obj, Brush value) =>
        obj.SetValue(BorderBackgroundProperty, value);

    public static Brush GetBorderBackground(DependencyObject obj) =>
        (Brush)obj.GetValue(BorderBackgroundProperty);
}
