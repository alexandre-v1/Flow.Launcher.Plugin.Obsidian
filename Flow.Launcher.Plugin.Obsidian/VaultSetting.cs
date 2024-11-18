namespace Flow.Launcher.Plugin.Obsidian;

public class VaultSetting : GlobalVaultSetting
{
    private bool _useGlobalSetting = true;
    private bool _useGlobalExcludedPaths = true;
    
    
    public bool UseGlobalSetting
    {
        get => _useGlobalSetting;
        set
        {
            if (_useGlobalSetting == value) return;
            _useGlobalSetting = value;
            OnPropertyChanged();
        }
    }

    public bool UseGlobalExcludedPaths
    {
        get => _useGlobalExcludedPaths;
        set
        {
            if (_useGlobalExcludedPaths == value) return;
            _useGlobalExcludedPaths = value;
            OnPropertyChanged();
        }
    }
}
