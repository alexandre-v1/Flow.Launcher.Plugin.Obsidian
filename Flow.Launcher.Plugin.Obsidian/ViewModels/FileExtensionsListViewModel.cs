using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class FileExtensionsListViewModel : BaseModel
{
    private readonly FileExtensionsSetting _extensionsSetting;
    private readonly ObservableCollection<FileExtension> _extensions;
    private readonly ObservableCollection<FileExtensionGroup> _extensionGroups;

    public FileExtensionsListViewModel()
    {
        _extensionsSetting = new FileExtensionsSetting();
        _extensions = new ObservableCollection<FileExtension>(_extensionsSetting.Extensions);
        _extensionGroups = new ObservableCollection<FileExtensionGroup>(_extensionsSetting.ExtensionGroups);
    }

    public FileExtensionsListViewModel(FileExtensionsSetting extensionsSetting)
    {
        _extensionsSetting = extensionsSetting;
        _extensions = new ObservableCollection<FileExtension>(_extensionsSetting.Extensions);
        _extensionGroups = new ObservableCollection<FileExtensionGroup>(_extensionsSetting.ExtensionGroups);
    }

    public IEnumerable<FileExtension> Extensions => _extensions;
    public IEnumerable<FileExtensionGroup> ExtensionGroups => _extensionGroups;

}
