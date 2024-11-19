using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian;

public partial class GlobalVaultSettingView
{
    private GlobalVaultSetting GlobalVaultSetting { get; set; }
    
    public GlobalVaultSettingView(GlobalVaultSetting globalVaultSetting)
    {
        GlobalVaultSetting = globalVaultSetting;
        InitializeComponent();
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
}
