using System.Windows;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.ViewModels;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class SettingsView
{
    public SettingsView(SettingsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e) =>
        StyleManager.Instance.ApplyStylesToControl(this);

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        SettingsViewModel? settingsViewModel = DataContext as SettingsViewModel;
        settingsViewModel?.OnUnloaded();
    }
}
