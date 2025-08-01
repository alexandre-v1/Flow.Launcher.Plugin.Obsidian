using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class ExcludePathsViewModel : BaseModel
{
    private readonly List<string> _designExcludePaths = ["Path 1", "Path 2"];
    private string _excludePathInput = string.Empty;

    public ExcludePathsViewModel() =>
        ExcludePaths = new ObservableCollection<string>(_designExcludePaths);

    public ExcludePathsViewModel(IList<string> excludePaths)
    {
        ExcludePaths = new ObservableCollection<string>(excludePaths);
        ExcludePaths.CollectionChanged += OnCollectionChanged;
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

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        OnPropertyChanged(nameof(ExcludePaths));

    public List<string> GetCurrentPaths() => ExcludePaths.ToList();

    [RelayCommand]
    private void RemoveExcludePath(string path) => ExcludePaths.Remove(path);

    [RelayCommand]
    private void AddExcludePath()
    {
        if (string.IsNullOrWhiteSpace(ExcludePathInput) || ExcludePaths.Contains(ExcludePathInput))
        {
            return;
        }

        ExcludePaths.Add(ExcludePathInput);
        ExcludePathInput = string.Empty;
    }
}
