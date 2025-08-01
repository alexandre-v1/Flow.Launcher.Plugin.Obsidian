using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class IconCache
{
    private static readonly ConcurrentDictionary<string, ImageSource> _imageCache = new();
    private static readonly ConcurrentDictionary<string, Result.IconDelegate> _delegateCache = new();

    private static ImageSource? _defaultIcon;

    public static Result.IconDelegate GetCachedIconDelegate(string resourcePath) =>
        _delegateCache.GetOrAdd(resourcePath, path =>
            () => GetCachedImage(path));

    public static ImageSource GetCachedImage(string resourcePath) =>
        _imageCache.GetOrAdd(resourcePath, LoadIcon);


    private static ImageSource LoadIcon(string resourcePath)
    {
        if (!TryLoadFromContentFile(resourcePath, out ImageSource? contentIcon))
        {
            return LoadEmbeddedIcon(resourcePath);
        }

        return contentIcon ?? LoadEmbeddedIcon(resourcePath);
    }

    private static bool TryLoadFromContentFile(string resourcePath, out ImageSource? icon)
    {
        icon = null;
        try
        {
            string filePath = GetContentFilePath(resourcePath);

            if (File.Exists(filePath))
            {
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                icon = bitmap;
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load Content icon {resourcePath}: {ex.Message}");
        }

        return false;
    }

    private static string GetContentFilePath(string resourcePath)
    {
        if (Path.IsPathRooted(resourcePath))
        {
            return resourcePath;
        }

        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation) ?? string.Empty;

        return Path.Combine(assemblyDirectory, resourcePath);
    }

    private static ImageSource LoadEmbeddedIcon(string resourcePath)
    {
        try
        {
            Uri uri = new($"pack://application:,,,/Flow.Launcher.Plugin.Obsidian;component/{resourcePath}");
            BitmapImage bitmap = new(uri);
            bitmap.Freeze();
            return bitmap;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load icon {resourcePath}: {ex.Message}");
            return CreateDefaultIcon();
        }
    }

    private static ImageSource CreateDefaultIcon()
    {
        if (_defaultIcon is not null)
        {
            return _defaultIcon;
        }

        const int size = 256;

        DrawingVisual drawingVisual = new();
        using (DrawingContext context = drawingVisual.RenderOpen())
        {
            context.DrawRectangle(Brushes.LightGray, new Pen(Brushes.Gray, 1),
                new Rect(0, 0, size, size));
        }

        RenderTargetBitmap bitmap = new(size, size, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(drawingVisual);
        bitmap.Freeze();

        _defaultIcon = bitmap;
        return bitmap;
    }
}
