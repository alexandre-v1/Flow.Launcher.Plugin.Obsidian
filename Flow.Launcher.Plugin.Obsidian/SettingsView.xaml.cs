using System;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian;

public partial class SettingsView : UserControl
{
    public SettingsView(IPublicAPI? publicApi)
    {
        InitializeComponent();
        CreateVaultSettingControls();
    }

    private void SettingsView_OnLoaded(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("SettingsView_OnLoaded");
    }
    
    private void CreateVaultSettingControls()
    {
        var globalVaultSettingControl = new GlobalVaultSettingView(VaultManager.GlobalSetting);
        GlobalVaultSettingPanel.Children.Add(globalVaultSettingControl);
        Console.WriteLine($"CreateVaultSettingControls: {VaultManager.Vaults.Count}");
        foreach (Vault vault in VaultManager.Vaults)
        {
            var vaultSettingControl = new VaultSettingView(vault)
            {
                Margin = new Thickness(0, 0, 10, 0)
            };
            VaultsSettingPanel.Children.Add(vaultSettingControl);
            Console.WriteLine($"VaultSettingControl added: {vault.Name}");
        }
    }

    private void OnIncrease(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnDecrease(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
