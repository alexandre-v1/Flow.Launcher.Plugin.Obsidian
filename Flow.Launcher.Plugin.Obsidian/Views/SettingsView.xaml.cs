using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class SettingsView : INotifyPropertyChanged
{
    private Obsidian Obsidian { get; }
    private Settings Settings { get; }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private int _maxResults;
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
        OldLogo.IsChecked = Settings.OldLogos;
        UseFileExtension.IsChecked = Settings.UseFilesExtension;
        
        OldLogo.Checked += (_, _) => { Settings.OldLogos = true; };
        OldLogo.Unchecked += (_, _) => { Settings.OldLogos = false; };
        
        UseFileExtension.Checked += (_, _) => { Settings.UseFilesExtension = true; };
        UseFileExtension.Unchecked += (_, _) => { Settings.UseFilesExtension = false; };
    }
    
    private void SettingsView_OnUnloaded(object sender, RoutedEventArgs e)
    {
        Obsidian.ReloadData();
    }
    
    private void CreateVaultSettingControls(Settings settings)
    {
        var globalVaultSettingControl = new GlobalVaultSettingView(settings.GlobalVaultSetting);
        GlobalVaultSettingPanel.Children.Add(globalVaultSettingControl);
        var margin = new Thickness(0, 0, 10, 0);
        foreach (VaultSettingView? vaultSettingControl in VaultManager.Vaults.Select(vault => new VaultSettingView(vault) { Margin = margin }))
        {
            VaultsSettingPanel.Children.Add(vaultSettingControl);
        }
    }
    private void OnIncrease(object sender, RoutedEventArgs e)
    {
        MaxResults++;
    }

    private void OnDecrease(object sender, RoutedEventArgs e)
    {
        MaxResults--;
        if (MaxResults < 0) MaxResults = 0;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
