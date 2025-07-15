using System.Windows.Media;
using Flow.Launcher.Plugin.Obsidian.Helpers;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class VaultViewModel : BaseModel
{
    public string Name => _vault?.Name ?? DesignName;
    public string VaultPath => _vault?.VaultPath ?? DesignPath;
    public int FilesCount => _vault?.Files.Count ?? 0;
    public static ImageSource Icon => IconCache.GetCachedImage(Paths.ObsidianLogo);

    public bool IsActive
    {
        get => _vault?.IsActive ?? _isActive;
        set
        {
            if (_vault is not null)
                _vault.IsActive = value;
            else
                _isActive = value;
            OnPropertyChanged();
        }
    }

    public string DesignName = "Vault Name";
    public string DesignPath = "Vault Path";
    private readonly Vault? _vault;
    private bool _isActive;

    public VaultViewModel(Vault vault)
    {
        _vault = vault;
        _isActive = vault.IsActive;
    }

    // Parameterless constructor for design-time data
    public VaultViewModel() { }
}
