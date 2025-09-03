using System.Collections.Generic;
using System.Collections.ObjectModel;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class FileExtensionsListViewModel : BaseModel
{
    private readonly ObservableCollection<FileExtensionGroup> _extensionGroups;
    private readonly ObservableCollection<FileExtension> _extensions;

    public FileExtensionsListViewModel()
    {
        FileExtensionsSetting extensionsSetting = new();
        _extensions = new ObservableCollection<FileExtension>(extensionsSetting.Extensions);
        _extensionGroups = new ObservableCollection<FileExtensionGroup>(extensionsSetting.ExtensionGroups);
    }

    public FileExtensionsListViewModel(FileExtensionsSetting extensionsSetting)
    {
        _extensions = new ObservableCollection<FileExtension>(extensionsSetting.Extensions);
        _extensionGroups = new ObservableCollection<FileExtensionGroup>(extensionsSetting.ExtensionGroups);
    }

    public IEnumerable<FileExtension> Extensions => _extensions;
    public IEnumerable<FileExtensionGroup> ExtensionGroups => _extensionGroups;
}
