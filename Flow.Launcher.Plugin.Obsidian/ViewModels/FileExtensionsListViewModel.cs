using System.Collections.Generic;
using System.Collections.ObjectModel;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class FileExtensionsListViewModel : BaseModel
{
    public FileExtensionsListViewModel()
    {
        FileExtensions = new ObservableCollection<FileExtension>(
            FileExtensionsSetting.DefaultExtensions
        );
        FileExtensionGroups = new ObservableCollection<FileExtensionGroup>(
            FileExtensionsSetting.DefaultExtensionGroups
        );
    }

    public FileExtensionsListViewModel(FileExtensionsSetting fileExtensionsSetting)
    {
        FileExtensions = new ObservableCollection<FileExtension>(fileExtensionsSetting.Extensions);
        FileExtensionGroups = new ObservableCollection<FileExtensionGroup>(
            fileExtensionsSetting.ExtensionGroups
        );
    }

    public ICollection<FileExtension> FileExtensions { get; set; }

    public ICollection<FileExtensionGroup> FileExtensionGroups { get; set; }
}
