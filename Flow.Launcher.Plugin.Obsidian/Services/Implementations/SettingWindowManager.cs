using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

    public Task ShowViewAsync<TUserControl, TViewModel>(TViewModel viewModel)
        where TUserControl : UserControl
        where TViewModel : BaseModel
    {
        UserControl? openView = _settingsWindowModel.Content;
        Type viewType = typeof(TUserControl);
        bool isSameView = openView?.GetType() == typeof(TUserControl);

        if (openView is not null && isSameView)
        {
            openView.DataContext = viewModel;
            ShowWindow();
            return Task.CompletedTask;
        }

        _settingsWindowModel.Content = CreateView(viewType, viewModel);
        ShowWindow();
        return Task.CompletedTask;
    }

    private void SettingsWindowOnClosed(object? sender, EventArgs e)
    {
        _settingsWindowModel.Content = null;
        _settingsWindow = null;
    }

    public bool IsWindowOpen() => _settingsWindow?.IsActive is true;

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

    private static UserControl? CreateView<TViewModel>(Type viewType, TViewModel viewModel)
        where TViewModel : BaseModel
    {
        UserControl? newView = null;
        switch (viewType.Name)
        {
            case nameof(VaultSettingsView):
                if (viewModel is VaultSettingsViewModel vaultSettingsViewModel)
                {
                    newView = new VaultSettingsView(vaultSettingsViewModel);
                }

                break;
            default:
                Debug.WriteLine($"View type {viewType.Name} not supported");
                break;
        }

        if (newView is null)
        {
            return null;
        }

        newView.DataContext = viewModel;
        return newView;
    }
}
