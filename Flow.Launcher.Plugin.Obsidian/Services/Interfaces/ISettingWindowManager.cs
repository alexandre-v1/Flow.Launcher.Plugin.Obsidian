using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface ISettingWindowManager
{
    void ShowView<TUserControl>(BaseModel viewModel)
        where TUserControl : UserControl, new();
}
