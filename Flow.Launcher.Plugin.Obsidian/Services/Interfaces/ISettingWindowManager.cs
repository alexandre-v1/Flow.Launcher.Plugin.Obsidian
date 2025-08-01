using System.Threading.Tasks;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

public interface ISettingWindowManager
{
    Task ShowViewAsync<TUserControl, TViewModel>(TViewModel viewModel)
        where TUserControl : UserControl
        where TViewModel : BaseModel;
}
