using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface ISettingWindowManager
{
    void ShowView<TUserControl, TViewModel>(TViewModel viewModel)
        where TUserControl : UserControl, new()
        where TViewModel : BaseModel;
}
