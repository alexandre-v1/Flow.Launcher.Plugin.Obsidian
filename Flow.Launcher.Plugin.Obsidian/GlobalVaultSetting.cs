using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Flow.Launcher.Plugin.Obsidian;

public class GlobalVaultSetting : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _searchMarkdown = true;
    private bool _searchCanvas = true;
    private bool _searchImages;
    private bool _searchExcalidraw = true;
    private bool _searchOther;
    private bool _searchContent;
    private string[] _excludedPaths = Array.Empty<string>();

    public bool SearchMarkdown
    {
        get => _searchMarkdown;
        set
        {
            if (_searchMarkdown == value) return;
            _searchMarkdown = value;
            OnPropertyChanged();
        }
    }

    public bool SearchCanvas
    {
        get => _searchCanvas;
        set
        {
            if (_searchCanvas == value) return;
            _searchCanvas = value;
            OnPropertyChanged();
        }
    }

    public bool SearchImages
    {
        get => _searchImages;
        set
        {
            if (_searchImages == value) return;
            _searchImages = value;
            OnPropertyChanged();
        }
    }

    public bool SearchExcalidraw
    {
        get => _searchExcalidraw;
        set
        {
            if (_searchExcalidraw == value) return;
            _searchExcalidraw = value;
            OnPropertyChanged();
        }
    }

    public bool SearchOther
    {
        get => _searchOther;
        set
        {
            if (_searchOther == value) return;
            _searchOther = value;
            OnPropertyChanged();
        }
    }

    public bool SearchContent
    {
        get => _searchContent;
        set
        {
            if (_searchContent == value) return;
            _searchContent = value;
            OnPropertyChanged();
        }
    }
    
    public string[] ExcludedPaths
    {
        get => _excludedPaths;
        set
        {
            if (_excludedPaths == value) return;
            _excludedPaths = value;
            OnPropertyChanged();
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
