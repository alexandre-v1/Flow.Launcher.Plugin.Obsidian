using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class VaultSettingsView : INotifyPropertyChanged
{
    public VaultSettingsView()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private bool UiIsSmall => ActualWidth < 520;

    // Horizontal Orientation who change to Vertical when ui is small
    public Orientation AutoHorizontalOrientation =>
        UiIsSmall ? Orientation.Vertical : Orientation.Horizontal;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) =>
        OnPropertyChanged(nameof(AutoHorizontalOrientation));

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
