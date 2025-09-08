using System.Windows;
using System.Windows.Media;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class VaultsList
{
    public static readonly DependencyProperty BorderBackgroundProperty =
        SharedProperties.BorderBackgroundProperty.AddOwner(
            typeof(VaultsList),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


    public VaultsList()
    {
        InitializeComponent();
        SetResourceReference(BorderBackgroundProperty, "Color00B");
    }

    public Brush BorderBackground
    {
        get => (Brush)GetValue(BorderBackgroundProperty);
        set => SetValue(BorderBackgroundProperty, value);
    }
}
