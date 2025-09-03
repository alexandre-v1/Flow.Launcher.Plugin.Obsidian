using System.Windows;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class QueryCreatorDialog
{
    public QueryCreatorDialog() => InitializeComponent();

    private void BtnCancel_OnClick(object sender, RoutedEventArgs e) => Close();
}
