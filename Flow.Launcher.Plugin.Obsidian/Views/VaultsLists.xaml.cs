using System.Diagnostics;
using System.Windows;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class VaultsLists
{
    public VaultsLists() => InitializeComponent();

    private void VaultClick(object sender, RoutedEventArgs e) => Debug.WriteLine("Open Vault in new window");
}
