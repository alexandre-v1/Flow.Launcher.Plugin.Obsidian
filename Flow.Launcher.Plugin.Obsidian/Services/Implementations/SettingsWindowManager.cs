using System;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.ViewModels;
using Flow.Launcher.Plugin.Obsidian.Views;

namespace Flow.Launcher.Plugin.Obsidian.Services.Implementations;

public class SettingsWindowManager : ISettingsWindowManager
{
    private readonly SettingsWindowModel _settingsWindowModel;
    private SettingsWindow? _settingsWindow;
    private bool _viewIsOpen;

    public SettingsWindowManager(Settings settings) => _settingsWindowModel = CreateSettingsWindowModel(settings);

    private UserControl? CurrentView
    {
        get => _settingsWindowModel.Content;
        set => _settingsWindowModel.Content = value;
    }

    public event ISettingsWindowManager.ViewClosedEventHandler? ViewClosed;

    public void ShowView<TUserControl>(BaseModel viewModel) where TUserControl : UserControl, new()
    {
        CurrentView = OpenView<TUserControl>();
        CurrentView.DataContext = viewModel;
        ShowWindow();
        _viewIsOpen = true;
    }

    public void CloseWindow() => _settingsWindow?.Close();


    private TUserControl OpenView<TUserControl>() where TUserControl : UserControl, new()
    {
        bool isSameView = CurrentViewIsSameType(typeof(TUserControl));

        if (!isSameView)
        {
            CloseCurrentView();
        }

        CurrentView ??= CreateView<TUserControl>();
        return (TUserControl)CurrentView;
    }

    private bool CurrentViewIsSameType(Type type) => CurrentView?.GetType() == type;

    private void CloseCurrentView()
    {
        if (CurrentView is not null)
        {
            CurrentView.DataContextChanged -= OnViewClosed;
        }

        CurrentView = null;
    }

    private TUserControl CreateView<TUserControl>() where TUserControl : UserControl, new()
    {
        TUserControl view = new();
        view.DataContextChanged += OnViewClosed;
        return view;
    }

    private void ShowWindow()
    {
        _settingsWindow ??= CreateWindow();
        _settingsWindow.Show();
        _settingsWindow.Activate();
    }

    private SettingsWindow CreateWindow()
    {
        SettingsWindow settingsWindow = new(_settingsWindowModel);
        settingsWindow.Closed += OnSettingsWindowClosed;
        return settingsWindow;
    }

    private SettingsWindowModel CreateSettingsWindowModel(Settings settings)
    {
        SettingsWindowModel settingsWindowModel = new(settings);
        settingsWindowModel.ContentChanged += OnSettingWindowModelContentChanged;
        return settingsWindowModel;
    }

    private void OnViewClosed(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (!_viewIsOpen || ViewClosed is null)
        {
            return;
        }

        ViewClosed();
        _viewIsOpen = false;
    }

    private void OnSettingWindowModelContentChanged() => OnViewClosed(this, new DependencyPropertyChangedEventArgs());

    private void OnSettingsWindowClosed(object? sender, EventArgs e)
    {
        _settingsWindowModel.Content = null;
        _settingsWindow = null;
    }
}
