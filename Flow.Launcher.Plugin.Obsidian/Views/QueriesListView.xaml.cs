using System.Windows;
using System.Windows.Media;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class QueriesListView
{
    public static readonly DependencyProperty BorderBackgroundProperty =
        SharedProperties.BorderBackgroundProperty.AddOwner(
            typeof(QueriesListView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


    public QueriesListView()
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
