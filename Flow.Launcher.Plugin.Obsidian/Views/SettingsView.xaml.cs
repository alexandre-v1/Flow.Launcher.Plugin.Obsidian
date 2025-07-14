using System.Windows;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.ViewModels;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class SettingsView
{
    private readonly SettingsViewModel _viewModel;

    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => StyleManager.Instance.ApplyStylesToControl(this);

    private void OnUnloaded(object sender, RoutedEventArgs e) => _ = _viewModel.ReloadPluginDataAsync();
}
