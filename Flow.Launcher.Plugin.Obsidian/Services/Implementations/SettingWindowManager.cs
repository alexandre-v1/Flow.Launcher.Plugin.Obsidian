using System;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.ViewModels;
using Flow.Launcher.Plugin.Obsidian.Views;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class SettingWindowManager(Settings settings) : ISettingWindowManager
{
    private readonly SettingsWindowModel _settingsWindowModel = new(settings);
    private SettingsWindow? _settingsWindow;

    private UserControl? OpenView
    {
        get => _settingsWindowModel.Content;
        set => _settingsWindowModel.Content = value;
    }

    public void ShowView<TUserControl, TViewModel>(TViewModel viewModel)
        where TUserControl : UserControl, new()
        where TViewModel : BaseModel
    {
        bool isSameView = OpenView?.GetType() == typeof(TUserControl);

        if (!isSameView)
        {
            OpenView = new TUserControl();
        }

        OpenView ??= new TUserControl();
        OpenView.DataContext = viewModel;
        ShowWindow();
    }

    private void SettingsWindowOnClosed(object? sender, EventArgs e)
    {
        _settingsWindowModel.Content = null;
        _settingsWindow = null;
    }

    private void ShowWindow()
    {
        if (_settingsWindow is null)
        {
            _settingsWindow = new SettingsWindow(_settingsWindowModel);
            _settingsWindow.Closed += SettingsWindowOnClosed;
        }

        _settingsWindow.Show();
        _settingsWindow.Activate();
    }
}
