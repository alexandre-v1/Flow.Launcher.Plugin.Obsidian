using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian;

public partial class VaultSettingView : UserControl
{
    private Vault Vault { get; set; }
    private VaultSetting Setting { get; set; }
    
    public VaultSettingView(Vault vault)
    {
        Vault = vault;
        Setting = vault.Setting;
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        var globalVaultSettingControl = new GlobalVaultSettingView(Setting);
        GlobalVaultSettingPanel.Children.Add(globalVaultSettingControl);
    }

    private void VaultSettingViewOnLoaded(object sender, RoutedEventArgs e)
    {
        VaultName.Text = Vault.Name;
        
        UseGlobalSetting.IsChecked = Setting.UseGlobalSetting;
        UpdateGlobalVaultSettingVisibility(!Setting.UseGlobalSetting);
        UseGlobalExcludedPaths.IsChecked = Setting.UseGlobalExcludedPaths;
        
        UseGlobalSetting.Checked += (_, _) =>
        {
            Setting.UseGlobalSetting = true;
            UpdateGlobalVaultSettingVisibility(false);
        };
        UseGlobalSetting.Unchecked += (_, _) =>
        {
            Setting.UseGlobalSetting = false;
            UpdateGlobalVaultSettingVisibility(true);
        };
        
        UseGlobalExcludedPaths.Checked += (_, _) => { Setting.UseGlobalExcludedPaths = true; };
        UseGlobalExcludedPaths.Unchecked += (_, _) => { Setting.UseGlobalExcludedPaths = false; };
    }
    

    private void UpdateGlobalVaultSettingVisibility(bool isVisible)
    {
        GlobalVaultSettingPanel.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }
}
