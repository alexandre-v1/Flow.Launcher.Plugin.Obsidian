using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class Vault
{
    public delegate void VaultUpdatedEventHandler();

    public readonly string Id;
    public readonly VaultSetting Setting;
    private List<File> _files = [];
    private bool _isDirty;
    private string _path;

    public Vault(string id, string path, VaultSetting setting)
    {
        Id = id;
        Setting = setting;
        Icon = IconCache.GetCachedImage(Paths.ObsidianLogo);
        Path = path;
        UpdateVault();
    }

    public string Name { get; private set; }

    public string Path
    {
        get => _path;
        [MemberNotNull(nameof(_path), nameof(Name))]
        set
        {
            _path = value;
            Name = System.IO.Path.GetFileName(value);
            if (_path != value)
            {
                _isDirty = true;
            }
        }
    }

    public List<File> Files
    {
        get
        {
            if (_isDirty)
            {
                UpdateVault();
            }

            return _files;
        }
        private set => _files = value;
    }

    public HashSet<string> Tags { get; } = [];

    public ImageSource Icon { get; }

    public bool HasAdvancedUri { get; private set; }

    public bool IsActive
    {
        get => Setting.IsActive;
        set => Setting.IsActive = value;
    }

    public int FilesCount => Files.Count;
    public event VaultUpdatedEventHandler? VaultUpdated;

    public void UpdateVault()
    {
        UpdateFiles();
        UpdateObsidianPlugins();
        VaultUpdated?.Invoke();
        _isDirty = false;
    }

    public bool OpenInNewTabByDefault() => HasAdvancedUri && Setting.OpenInNewTabByDefault;

    public bool TagExists(string tag) => Tags.Any(t => t.EqualsIgnoreCase(tag));

    private void UpdateFiles()
    {
        HashSet<string> extensions = Setting.FileExtensions
            .GetActiveExtensionSuffix()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        FileDiscovery fileDiscovery = new(Path, extensions, Setting.RelativeExcludePaths);

        List<File> files = fileDiscovery.GetFiles().Select(CreateFile).ToList();

        foreach (File file in files)
        {
            if (file.Tags is not null)
            {
                Tags.UnionWith(file.Tags);
            }
        }

        Files = files;
        return;

        File CreateFile(FileInfo path) => new(this, path);
    }

    private void UpdateObsidianPlugins()
    {
        HasAdvancedUri = PluginsDetection.IsObsidianAdvancedUriPluginInstalled(Path);
        if (!HasAdvancedUri)
        {
            Setting.OpenInNewTabByDefault = false;
        }
    }
}
