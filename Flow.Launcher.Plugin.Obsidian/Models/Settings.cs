// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// Keep setters to allow JSON deserialization

using System.Collections.Generic;
using System.Windows;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Settings
{
    public QuerySetting DefaultQuery { get; set; } = new();
    public Dictionary<string, VaultSetting> Vaults { get; set; } = new();

    public double SettingWindowWidth { get; set; } = 700;
    public double SettingWindowHeight { get; set; } = 600;
    public double? SettingWindowTop { get; set; }
    public double? SettingWindowLeft { get; set; }
    public WindowState SettingWindowState { get; set; } = WindowState.Normal;

    public VaultSetting LoadVaultOrDefault(string id)
    {
        if (Vaults.TryGetValue(id, out VaultSetting? vaultSetting))
        {
            return vaultSetting;
        }

        VaultSetting newVaultSetting = new();
        Vaults.Add(id, newVaultSetting);
        return newVaultSetting;
    }
}
