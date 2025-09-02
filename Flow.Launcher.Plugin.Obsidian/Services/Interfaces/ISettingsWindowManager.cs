using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface ISettingsWindowManager
{
    delegate void ViewClosedEventHandler();

    event ViewClosedEventHandler? ViewClosed;

    void ShowView<TUserControl>(BaseModel viewModel) where TUserControl : UserControl, new();

    void CloseWindow();
}
