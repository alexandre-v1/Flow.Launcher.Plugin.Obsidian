using System.Windows;

namespace Flow.Launcher.Plugin.Obsidian;

public partial class VaultSettingView
{
    private Vault Vault { get; }
    private VaultSetting VaultSetting { get; set; }
    
    public VaultSettingView(Vault vault)
    {
        Vault = vault;
        VaultSetting = vault.VaultSetting;
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        var globalVaultSettingControl = new GlobalVaultSettingView(VaultSetting);
        GlobalVaultSettingPanel.Children.Add(globalVaultSettingControl);
    }

    private void VaultSettingViewOnLoaded(object sender, RoutedEventArgs e)
    {
        VaultName.Text = Vault.Name;
        
        UseGlobalSetting.IsChecked = VaultSetting.UseGlobalSetting;
        UpdateGlobalVaultSettingVisibility(!VaultSetting.UseGlobalSetting);
        UseGlobalExcludedPaths.IsChecked = VaultSetting.UseGlobalExcludedPaths;
        
        UseGlobalSetting.Checked += (_, _) =>
        {
            VaultSetting.UseGlobalSetting = true;
            UpdateGlobalVaultSettingVisibility(false);
        };
        UseGlobalSetting.Unchecked += (_, _) =>
        {
            VaultSetting.UseGlobalSetting = false;
            UpdateGlobalVaultSettingVisibility(true);
        };
        
        UseGlobalExcludedPaths.Checked += (_, _) => { VaultSetting.UseGlobalExcludedPaths = true; };
        UseGlobalExcludedPaths.Unchecked += (_, _) => { VaultSetting.UseGlobalExcludedPaths = false; };
    }
    

    private void UpdateGlobalVaultSettingVisibility(bool isVisible)
    {
        GlobalVaultSettingPanel.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }
}
