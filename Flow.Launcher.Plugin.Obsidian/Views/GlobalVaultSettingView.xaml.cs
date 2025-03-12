using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class GlobalVaultSettingView : INotifyPropertyChanged
{
    public Vault? Vault { get; }
    public GlobalVaultSetting GlobalVaultSetting { get; set; }

    public Visibility GlobalSettingVisibility
    {
        get => _globalSettingVisibility;
        set
        {
            if (_globalSettingVisibility == value) return;
            _globalSettingVisibility = value;
            OnPropertyChanged();
        }
    }

    private Visibility _globalSettingVisibility = Visibility.Visible;

    public GlobalVaultSettingView(Settings settings)
    {
        GlobalVaultSetting = settings.GlobalVaultSetting;
        InitializeComponent();
        DataContext = this;
    }

    public GlobalVaultSettingView(Vault vault)
    {
        Vault = vault;
        GlobalVaultSetting = vault.VaultSetting;
        InitializeComponent();
        DataContext = this;
    }

    private void GlobalVaultSettingViewOnLoaded(object sender, RoutedEventArgs e)
    {
        SetupOpenInNewTabDefault();
        SetupCheckboxes();
    }

    private void SetupOpenInNewTabDefault()
    {
        OpenInNewTabByDefault.IsChecked = GlobalVaultSetting.OpenInNewTabByDefault;
        OpenInNewTabByDefault.Checked += (_, _) => GlobalVaultSetting.OpenInNewTabByDefault = true;
        OpenInNewTabByDefault.Unchecked += (_, _) => GlobalVaultSetting.OpenInNewTabByDefault = false;
        BindingOperations.SetBinding(OpenInNewTabByDefault, VisibilityProperty, new Binding("GlobalSettingVisibility"));

        bool hasAdvancedUri = Vault?.HasAdvancedUri ?? VaultManager.OneVaultHasAdvancedUri;
        if (!hasAdvancedUri)
        {
            OpenInNewTabByDefault.IsEnabled = false;
            OpenInNewTabByDefault.ToolTip = "This option requires the Obsidian Advanced URI plugin in your vault";
            ToolTipService.SetShowOnDisabled(OpenInNewTabByDefault, true);
        }
        else if (Vault is null && !VaultManager.AllVaultsHaveAdvancedUri)
        {
            OpenInNewTabByDefault.ToolTip =
                "This option will be active only in vault with the Obsidian Advanced URI plugin";
        }
    }

    private void SetupCheckboxes()
    {
        SetupCheckbox(SearchMarkdown, nameof(GlobalVaultSetting.SearchMarkdown));
        SetupCheckbox(SearchCanvas, nameof(GlobalVaultSetting.SearchCanvas));
        SetupCheckbox(SearchImages, nameof(GlobalVaultSetting.SearchImages));
        SetupCheckbox(SearchExcalidraw, nameof(GlobalVaultSetting.SearchExcalidraw));
        SetupCheckbox(SearchOther, nameof(GlobalVaultSetting.SearchOther));
    }

    private void SetupCheckbox(CheckBox checkBox, string propertyName)
    {
        PropertyInfo? property = GlobalVaultSetting.GetType().GetProperty(propertyName);
        if (property is null) return;
        checkBox.IsChecked = property.GetValue(GlobalVaultSetting) as bool? ?? false;
        checkBox.Checked += (_, _) => property.SetValue(GlobalVaultSetting, true);
        checkBox.Unchecked += (_, _) => property.SetValue(GlobalVaultSetting, false);
        BindingOperations.SetBinding(checkBox, VisibilityProperty, new Binding("GlobalSettingVisibility"));
    }

    private void AddExcludePath_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewExcludePathText.Text)) return;
        GlobalVaultSetting.ExcludedPaths.Add(NewExcludePathText.Text);
        NewExcludePathText.Clear();

        ScrollViewer? scrollViewer = ScrollViewerHelper.FindScrollViewer(ExcludedPathsListBox);
        scrollViewer?.ScrollToBottom();
    }

    private void RemoveExcludePath_Click(object sender, RoutedEventArgs e)
    {
        Button? button = sender as Button;
        if (button?.DataContext is not string path) return;
        GlobalVaultSetting.ExcludedPaths.Remove(path);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e) =>
        ScrollViewerHelper.HandlePreviewMouseWheel(sender, e, ExcludedPathsListBox);

    public event PropertyChangedEventHandler? PropertyChanged;
}
