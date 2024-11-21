using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class GlobalVaultSettingView : INotifyPropertyChanged
{
    private GlobalVaultSetting GlobalVaultSetting { get; set; }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private Visibility _globalSettingVisibility = Visibility.Visible;
    public Visibility GlobalSettingVisibility
    {
        get => _globalSettingVisibility;
        set
        {
            if (_globalSettingVisibility == value) return;
            _globalSettingVisibility = value;
            OnPropertyChanged();
        }
    }
    
    public GlobalVaultSettingView(GlobalVaultSetting globalVaultSetting)
    {
        GlobalVaultSetting = globalVaultSetting;
        InitializeComponent();
        DataContext = this;
        RefreshExcludedPaths();
    }
    
    private void GlobalVaultSettingViewOnLoaded(object sender, RoutedEventArgs e)
    {
        SearchMarkdown.IsChecked = GlobalVaultSetting.SearchMarkdown;
        SearchCanvas.IsChecked = GlobalVaultSetting.SearchCanvas;
        SearchImages.IsChecked = GlobalVaultSetting.SearchImages;
        SearchExcalidraw.IsChecked = GlobalVaultSetting.SearchExcalidraw;
        SearchOther.IsChecked = GlobalVaultSetting.SearchOther;
        SearchContent.IsChecked = GlobalVaultSetting.SearchContent;
        
        SearchMarkdown.Checked += (_, _) => { GlobalVaultSetting.SearchMarkdown = true; };
        SearchMarkdown.Unchecked += (_, _) => { GlobalVaultSetting.SearchMarkdown = false; }; 
        
        SearchCanvas.Checked += (_, _) => { GlobalVaultSetting.SearchCanvas = true; }; 
        SearchCanvas.Unchecked += (_, _) => { GlobalVaultSetting.SearchCanvas = false; }; 
        
        SearchImages.Checked += (_, _) => { GlobalVaultSetting.SearchImages = true; }; 
        SearchImages.Unchecked += (_, _) => { GlobalVaultSetting.SearchImages = false; }; 
        
        SearchExcalidraw.Checked += (_, _) => { GlobalVaultSetting.SearchExcalidraw = true; }; 
        SearchExcalidraw.Unchecked += (_, _) => { GlobalVaultSetting.SearchExcalidraw = false; }; 
        
        SearchOther.Checked += (_, _) => { GlobalVaultSetting.SearchOther = true; }; 
        SearchOther.Unchecked += (_, _) => { GlobalVaultSetting.SearchOther = false; }; 
        
        SearchContent.Checked += (_, _) => { GlobalVaultSetting.SearchContent = true; }; 
        SearchContent.Unchecked += (_, _) => { GlobalVaultSetting.SearchContent = false; }; 
    }

    private void AddExcludePath_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewExcludePathText.Text)) return;
        GlobalVaultSetting.ExcludedPaths.Add(NewExcludePathText.Text);
        RefreshExcludedPaths();
        NewExcludePathText.Clear();
        
        ScrollViewer? scrollViewer = ScrollViewerHelper.FindScrollViewer(ExcludedPathsListBox);
        scrollViewer?.ScrollToBottom();
    }

    private void RemoveExcludePath_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.DataContext is not string path) return;
        GlobalVaultSetting.ExcludedPaths.Remove(path);
        RefreshExcludedPaths();
    }

    private void RefreshExcludedPaths()
    {
        // Force the ListBox to refresh
        ExcludedPathsListBox.ItemsSource = null;
        ExcludedPathsListBox.ItemsSource = GlobalVaultSetting.ExcludedPaths;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScrollViewerHelper.HandlePreviewMouseWheel(sender, e, ExcludedPathsListBox);
    }
}
