using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Flow.Launcher.Plugin.Obsidian.Helpers;

public static class ScrollViewerHelper
{
    public static ScrollViewer? FindScrollViewer(DependencyObject element)
    {
        if (element is ScrollViewer scrollViewer) return scrollViewer;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            ScrollViewer? result = FindScrollViewer(VisualTreeHelper.GetChild(element, i));
            if (result is not null) return result;
        }

        return null;
    }

    public static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e, UIElement element)
    {
        ScrollViewer? scrollViewer = FindScrollViewer(element);
        if (scrollViewer is null) return;

        bool isScrollingDown = e.Delta < 0;
        bool isScrollingUp = e.Delta > 0;

        if (!IsScrolledToBoundary(scrollViewer, isScrollingDown, isScrollingUp)) return;

        e.Handled = true;
        RaiseMouseWheelEventOnParent(e, element);
    }

    private static ScrollViewer? FindParentScrollViewer(DependencyObject element)
    {
        DependencyObject? parent = VisualTreeHelper.GetParent(element);
        while (parent is not null)
        {
            if (parent is ScrollViewer scrollViewer) return scrollViewer;
            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }

    private static bool IsScrolledToBoundary(ScrollViewer scrollViewer, bool scrollingDown, bool scrollingUp)
    {
        bool atBottomBoundary = scrollingDown && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight;
        bool atTopBoundary = scrollingUp && scrollViewer.VerticalOffset is 0;

        return atBottomBoundary || atTopBoundary;
    }

    private static void RaiseMouseWheelEventOnParent(MouseWheelEventArgs e, UIElement element)
    {
        MouseWheelEventArgs eventArg = new(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent
        };
        ScrollViewer? parentScrollViewer = FindParentScrollViewer(element);
        parentScrollViewer?.RaiseEvent(eventArg);
    }
}
