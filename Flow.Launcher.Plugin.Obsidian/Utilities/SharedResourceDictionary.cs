using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "WPFTutorial.Utils")]

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

/// <summary>
///     The shared resource dictionary is a specialized resource dictionary
///     that loads it content only once. If a second instance with the same source
///     is created, it only merges the resources from the cache.
/// </summary>
public class SharedResourceDictionary : ResourceDictionary
{
    private static readonly Dictionary<Uri, ResourceDictionary> _sharedDictionaries = new();

    private Uri _sourceUri = null!;

    /// <summary>
    ///     Gets or sets the uniform resource identifier (URI) to load resources from.
    /// </summary>
    public new Uri Source
    {
        get => _sourceUri;
        set
        {
            _sourceUri = new Uri(value.OriginalString, UriKind.RelativeOrAbsolute);

            if (!_sharedDictionaries.TryGetValue(value, out ResourceDictionary? dictionary))
            {
                base.Source = value;
                _sharedDictionaries.Add(value, this);
            }
            else
            {
                MergedDictionaries.Add(dictionary);
            }
        }
    }
}
