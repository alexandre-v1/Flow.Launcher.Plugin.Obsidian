using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;

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

    public SettingsView(Settings settings, Obsidian obsidian)
    {
        Obsidian = obsidian;
        Settings = settings;
        InitializeComponent();
        CreateVaultSettingControls(settings);
    }

    private void SettingsView_OnLoaded(object sender, RoutedEventArgs e)
    {
        MaxResults = Settings.MaxResult;
        UseAliases.IsChecked = Settings.UseAliases;
        UseFileExtension.IsChecked = Settings.UseFilesExtension;
        AddGlobalFolderExcludeToContext.IsChecked = Settings.AddGlobalFolderExcludeToContext;
        AddLocalFolderExcludeToContext.IsChecked = Settings.AddLocalFolderExcludeToContext;
        AddCheckBoxesToContext.IsChecked = Settings.AddCheckBoxesToContext;

        UseAliases.Checked += (_, _) => { Settings.UseAliases = true; };
        UseAliases.Unchecked += (_, _) => { Settings.UseAliases = false; };

        UseFileExtension.Checked += (_, _) => { Settings.UseFilesExtension = true; };
        UseFileExtension.Unchecked += (_, _) => { Settings.UseFilesExtension = false; };

        AddGlobalFolderExcludeToContext.Checked += (_, _) => { Settings.AddGlobalFolderExcludeToContext = true; };
        AddGlobalFolderExcludeToContext.Unchecked += (_, _) => { Settings.AddGlobalFolderExcludeToContext = false; };

        AddLocalFolderExcludeToContext.Checked += (_, _) => { Settings.AddLocalFolderExcludeToContext = true; };
        AddLocalFolderExcludeToContext.Unchecked += (_, _) => { Settings.AddLocalFolderExcludeToContext = false; };

        AddCheckBoxesToContext.Checked += (_, _) => { Settings.AddCheckBoxesToContext = true; };
        AddCheckBoxesToContext.Unchecked += (_, _) => { Settings.AddCheckBoxesToContext = false; };
    }

    private void SettingsView_OnUnloaded(object sender, RoutedEventArgs e) => Obsidian.ReloadData();

    private void CreateVaultSettingControls(Settings settings)
    {
        GlobalVaultSettingView globalVaultSettingControl = new(settings.GlobalVaultSetting);
        GlobalVaultSettingPanel.Children.Add(globalVaultSettingControl);
        Thickness margin = new(0, 0, 10, 0);
        foreach (VaultSettingView? vaultSettingControl in VaultManager.Vaults.Select(vault =>
                     new VaultSettingView(vault) { Margin = margin }))
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
