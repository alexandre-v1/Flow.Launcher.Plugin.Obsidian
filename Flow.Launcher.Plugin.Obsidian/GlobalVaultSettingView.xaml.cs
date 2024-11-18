using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian;

public partial class GlobalVaultSettingView : UserControl
{
    public GlobalVaultSetting Setting { get; set; }
    
    public GlobalVaultSettingView(GlobalVaultSetting setting)
    {
        Setting = setting;
        InitializeComponent();
    }
    
    private void GlobalVaultSettingViewOnLoaded(object sender, RoutedEventArgs e)
    {
        SearchMarkdown.IsChecked = Setting.SearchMarkdown;
        SearchCanvas.IsChecked = Setting.SearchCanvas;
        SearchImages.IsChecked = Setting.SearchImages;
        SearchExcalidraw.IsChecked = Setting.SearchExcalidraw;
        SearchOther.IsChecked = Setting.SearchOther;
        SearchContent.IsChecked = Setting.SearchContent;
        
        SearchMarkdown.Checked += (_, _) => { Setting.SearchMarkdown = true; };
        SearchMarkdown.Unchecked += (_, _) => { Setting.SearchMarkdown = false; }; 
        
        SearchCanvas.Checked += (_, _) => { Setting.SearchCanvas = true; }; 
        SearchCanvas.Unchecked += (_, _) => { Setting.SearchCanvas = false; }; 
        
        SearchImages.Checked += (_, _) => { Setting.SearchImages = true; }; 
        SearchImages.Unchecked += (_, _) => { Setting.SearchImages = false; }; 
        
        SearchExcalidraw.Checked += (_, _) => { Setting.SearchExcalidraw = true; }; 
        SearchExcalidraw.Unchecked += (_, _) => { Setting.SearchExcalidraw = false; }; 
        
        SearchOther.Checked += (_, _) => { Setting.SearchOther = true; }; 
        SearchOther.Unchecked += (_, _) => { Setting.SearchOther = false; }; 
        
        SearchContent.Checked += (_, _) => { Setting.SearchContent = true; }; 
        SearchContent.Unchecked += (_, _) => { Setting.SearchContent = false; }; 
    }
}
