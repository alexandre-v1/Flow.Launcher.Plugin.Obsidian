using System.Windows;
using Flow.Launcher.Plugin.Obsidian.Managers;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class VaultSettingView
{
    private VaultManager VaultManager { get; }
    private Vault Vault { get; }
    private VaultSetting VaultSetting { get; }
    private GlobalVaultSettingView? GlobalVaultSettingView { get; set; }

    public VaultSettingView(VaultManager vaultManager, Vault vault)
    {
        VaultManager = vaultManager;
        Vault = vault;
        VaultSetting = vault.VaultSetting;
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        GlobalVaultSettingView = new GlobalVaultSettingView(VaultManager, Vault);
        GlobalVaultSettingPanel.Children.Add(GlobalVaultSettingView);
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
        if (GlobalVaultSettingView != null)
            GlobalVaultSettingView.GlobalSettingVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }
}
