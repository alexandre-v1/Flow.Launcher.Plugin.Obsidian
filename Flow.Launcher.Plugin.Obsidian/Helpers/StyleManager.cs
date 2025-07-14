using System;
using System.Diagnostics;
using System.Windows;

namespace Flow.Launcher.Plugin.Obsidian.Helpers;

public class StyleManager
{
    public static StyleManager Instance
    {
        get
        {
            if (_instance is not null) return _instance;
            lock (_lock)
            {
                _instance ??= new StyleManager();
            }

            return _instance;
        }
    }

    private static readonly object _lock = new();
    private static StyleManager? _instance;

    public bool FlowLauncherStylesLoaded { get; private set; }

    private ResourceDictionary? _flowLauncherStyles;
    private ResourceDictionary? _pluginStyles;
    private bool _initialized;

    private StyleManager() => Initialize();

    public void ApplyStylesToControl(FrameworkElement control)
    {
        try
        {
            bool pluginStylesAlreadyApplied = IsPluginStylesAlreadyApplied(control);

            if (!pluginStylesAlreadyApplied && _pluginStyles is not null)
            {
                control.Resources.MergedDictionaries.Add(_pluginStyles);
            }

            if (FlowLauncherStylesLoaded && _flowLauncherStyles is not null)
            {
                control.Resources.MergedDictionaries.Add(_flowLauncherStyles);
            }

            Debug.WriteLine($"Styles applied to {control.GetType().Name}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error applying styles to {control.GetType().Name}: {ex.Message}");
        }
    }

    private bool IsPluginStylesAlreadyApplied(FrameworkElement control)
    {
        if (_pluginStyles?.Source is null) return false;

        foreach (ResourceDictionary? dict in control.Resources.MergedDictionaries)
        {
            if (dict.Source is not null && dict.Source.Equals(_pluginStyles.Source))
            {
                return true;
            }
        }

        return false;
    }

    private void Initialize()
    {
        if (_initialized) return;

        try
        {
            _pluginStyles = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Flow.Launcher.Plugin.Obsidian;component/Style.xaml")
            };

            TryLoadFlowLauncherStyles();

            _initialized = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"StyleManager initialization error: {ex.Message}");
            _initialized = true;
        }
    }

    private void TryLoadFlowLauncherStyles()
    {
        try
        {
            Application? app = Application.Current;
            if (app?.Resources is { Count: > 0 })
            {
                _flowLauncherStyles = app.Resources;
                FlowLauncherStylesLoaded = true;
                Debug.WriteLine("Flow Launcher styles loaded successfully");
                return;
            }

            ResourceDictionary flowStyles = new()
            {
                Source = new Uri("pack://application:,,,/Flow.Launcher;component/App.xaml")
            };

            if (flowStyles.Count <= 0) return;
            _flowLauncherStyles = flowStyles;
            FlowLauncherStylesLoaded = true;
            Debug.WriteLine("Flow Launcher styles loaded from direct reference");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Flow Launcher styles not available: {ex.Message}");
            FlowLauncherStylesLoaded = false;
        }
    }
}
