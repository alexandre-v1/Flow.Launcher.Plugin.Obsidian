using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class ExcludePathsViewModel : BaseModel
{
    private readonly List<string> _designExcludePaths = ["Path 1", "Path 2"];
    private readonly IList<string>? _excludePaths;
    private string _excludePathInput = string.Empty;

    public ExcludePathsViewModel() => ExcludePaths = new ObservableCollection<string>(_designExcludePaths);

    public ExcludePathsViewModel(IList<string> excludePaths)
    {
        ExcludePaths = new ObservableCollection<string>(excludePaths);
        _excludePaths = excludePaths;
    }

    public ObservableCollection<string> ExcludePaths { get; }

    public string ExcludePathInput
    {
        get => _excludePathInput;
        set
        {
            if (value == _excludePathInput)
            {
                return;
            }

            _excludePathInput = value;
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private void RemoveExcludePath(string path)
    {
        ExcludePaths.Remove(path);
        _excludePaths?.Remove(path);
    }

    [RelayCommand]
    private void AddExcludePath()
    {
        if (string.IsNullOrWhiteSpace(ExcludePathInput) || ExcludePaths.Contains(ExcludePathInput))
        {
            return;
        }

        ExcludePaths.Add(ExcludePathInput);
        _excludePaths?.Add(ExcludePathInput);
        ExcludePathInput = string.Empty;
    }
}
