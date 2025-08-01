// Most of the code is taken from https://github.com/Flow-Launcher/Flow.Launcher/blob/dev/Flow.Launcher/MainWindow.xaml.cs
// to replicate Flow Launcher setting window

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.ViewModels;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class SettingsWindow
{
    private readonly SettingsWindowModel _viewModel;

    public SettingsWindow(SettingsWindowModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        // Since WindowStartupLocation is set to Manual, initialize the window position before calling InitializeComponent
        UpdatePositionAndState();
        InitializeComponent();
        StyleManager.Instance.ApplyStylesToControl(this);
    }

    #region Window Events

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RefreshMaximizeRestoreButton();

        // Fix (workaround) for the window freezes after lock screen (Win+L) or sleep
        // https://stackoverflow.com/questions/4951058/software-rendering-mode-wpf
        if (PresentationSource.FromVisual(this) is not HwndSource hwndSource)
        {
            return;
        }

        HwndTarget? hwndTarget = hwndSource.CompositionTarget;
        if (hwndTarget is null)
        {
            return;
        }

        hwndTarget.RenderMode = RenderMode.SoftwareOnly;

        UpdatePositionAndState();
    }

    private void OnClosed(object sender, EventArgs e)
    {
        //Todo: Save settings when window is closed
    }

    private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e) => Close();

    private void WindowStateChanged(object sender, EventArgs e)
    {
        RefreshMaximizeRestoreButton();
        if (IsLoaded)
        {
            _viewModel.WindowState = WindowState;
        }
    }

    private void WindowLocationChanged(object sender, EventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        _viewModel.WindowTop = Top;
        _viewModel.WindowLeft = Left;
    }

    #endregion

    #region Window Custom TitleBar

    private void OnMinimizeButtonClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState switch
        {
            WindowState.Maximized => WindowState.Normal,
            _ => WindowState.Maximized
        };

    private void OnCloseButtonClick(object sender, RoutedEventArgs e) => Close();

    private void RefreshMaximizeRestoreButton()
    {
        if (WindowState is WindowState.Maximized)
        {
            MaximizeButton.Visibility = Visibility.Hidden;
            RestoreButton.Visibility = Visibility.Visible;
        }
        else
        {
            MaximizeButton.Visibility = Visibility.Visible;
            RestoreButton.Visibility = Visibility.Hidden;
        }
    }

    #endregion

    #region Window Position

    private void UpdatePositionAndState()
    {
        double? previousTop = _viewModel.WindowTop;
        double? previousLeft = _viewModel.WindowLeft;

        if (previousTop is not null && previousLeft is not null)
        {
            if (_viewModel.WindowLeft is not null && _viewModel.WindowTop is not null)
            {
                double left = _viewModel.WindowLeft.Value;
                double top = _viewModel.WindowTop.Value;

                AdjustWindowPosition(ref top, ref left);
                SetWindowPosition(top, left);
            }
        }

        WindowState = _viewModel.WindowState;
    }

    private void SetWindowPosition(double top, double left)
    {
        // Ensure window does not exceed screen boundaries
        top = Math.Max(top, SystemParameters.VirtualScreenTop);
        left = Math.Max(left, SystemParameters.VirtualScreenLeft);
        top = Math.Min(top, SystemParameters.VirtualScreenHeight - ActualHeight);
        left = Math.Min(left, SystemParameters.VirtualScreenWidth - ActualWidth);

        Top = top;
        Left = left;
    }

    private void AdjustWindowPosition(ref double top, ref double left)
    {
        // Adjust window position if it exceeds screen boundaries
        top = Math.Max(top, SystemParameters.VirtualScreenTop);
        left = Math.Max(left, SystemParameters.VirtualScreenLeft);
        top = Math.Min(top, SystemParameters.VirtualScreenHeight - ActualHeight);
        left = Math.Min(left, SystemParameters.VirtualScreenWidth - ActualWidth);
    }

    #endregion
}
