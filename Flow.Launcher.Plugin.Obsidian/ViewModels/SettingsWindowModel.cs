using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class SettingsWindowModel(Settings settings) : BaseModel
{
    private UserControl? _content;

    public UserControl? Content
    {
        get => _content;
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    public static ImageSource FlowLauncherIcon => IconCache.GetCachedImage(Paths.ObsidianLogo);

    public WindowState WindowState
    {
        get => settings.SettingWindowState;
        set => settings.SettingWindowState = value;
    }

    public double WindowWidth
    {
        get => settings.SettingWindowWidth;
        set => settings.SettingWindowWidth = value;
    }

    public double WindowHeight
    {
        get => settings.SettingWindowHeight;
        set => settings.SettingWindowHeight = value;
    }

    public double? WindowTop
    {
        get => settings.SettingWindowTop;
        set => settings.SettingWindowTop = value;
    }

    public double? WindowLeft
    {
        get => settings.SettingWindowLeft;
        set => settings.SettingWindowLeft = value;
    }
}
