using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;
using CheckBox = System.Windows.Controls.CheckBox;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class SettingsView : INotifyPropertyChanged
{
    public int MaxResults
    {
        get => _maxResults;
        set
        {
            if (_maxResults == value) return;
            _maxResults = value;
            Settings.MaxResult = value;
            OnPropertyChanged();
        }
    }

    private Obsidian Obsidian { get; }
    private Settings Settings { get; }
    private int _maxResults;

    public SettingsView(VaultManager vaultManager, Obsidian obsidian)
    {
        Obsidian = obsidian;
        Settings = vaultManager.Settings;
        InitializeComponent();
        CreateVaultSettingControls(vaultManager);
    }

    private void SettingsView_OnLoaded(object sender, RoutedEventArgs e)
    {
        MaxResults = Settings.MaxResult;
        SetupCheckboxes();
    }

    private void SetupCheckboxes()
    {
        SetupCheckbox(AddCreateNoteOptionOnAllSearch, nameof(Settings.AddCreateNoteOptionOnAllSearch));
        SetupCheckbox(UseFileExtension, nameof(Settings.UseFilesExtension));
        SetupCheckbox(UseAliases, nameof(Settings.UseAliases));
        SetupCheckbox(UseTags, nameof(Settings.UseTags));
        SetupCheckbox(AddGlobalFolderExcludeToContext, nameof(Settings.AddGlobalFolderExcludeToContext));
        SetupCheckbox(AddLocalFolderExcludeToContext, nameof(Settings.AddLocalFolderExcludeToContext));
        SetupCheckbox(AddCheckBoxesToContext, nameof(Settings.AddCheckBoxesToContext));
    }

    private void SetupCheckbox(CheckBox checkBox, string propertyName)
    {
        PropertyInfo? property = Settings.GetType().GetProperty(propertyName);
        if (property is null) return;
        checkBox.IsChecked = property.GetValue(Settings) as bool? ?? false;
        checkBox.Checked += (_, _) => property.SetValue(Settings, true);
        checkBox.Unchecked += (_, _) => property.SetValue(Settings, false);
    }

    private void SettingsView_OnUnloaded(object sender, RoutedEventArgs e) => _ = Obsidian.ReloadDataAsync();

    private void CreateVaultSettingControls(VaultManager vaultManager)
    {
        GlobalVaultSettingView globalVaultSettingControl = new(vaultManager);
        GlobalVaultSettingPanel.Children.Add(globalVaultSettingControl);
        Thickness margin = new(0, 0, 10, 0);
        foreach (VaultSettingView? vaultSettingControl in vaultManager.Vaults.Select(vault =>
                     new VaultSettingView(vaultManager, vault) { Margin = margin }))
            VaultsSettingPanel.Children.Add(vaultSettingControl);
    }

    private void OnIncrease(object sender, RoutedEventArgs e) => MaxResults++;

    private void OnDecrease(object sender, RoutedEventArgs e)
    {
        MaxResults--;
        if (MaxResults < 0) MaxResults = 0;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}
