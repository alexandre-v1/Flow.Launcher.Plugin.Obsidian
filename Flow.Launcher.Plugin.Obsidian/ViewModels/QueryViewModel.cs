using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.Views;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class QueryViewModel(
    ObsidianQuerySetting obsidianQuerySetting,
    ISettingsWindowManager windowManager,
    IQueryService queryService) : BaseModel
{
    public delegate void QueryDeletedEventHandler(QueryViewModel queryViewModel);

    [UsedImplicitly] // For design-time data
    public QueryViewModel() : this(new ObsidianQuerySetting(), null!, null!) { }

    public string Name => obsidianQuerySetting.Name;

    public ImageSource Icon => IconCache.GetCachedImage(Paths.ObsidianLogo);

    public bool IsActive
    {
        get => obsidianQuerySetting.IsActive;
        set
        {
            obsidianQuerySetting.IsActive = value;
            OnPropertyChanged();
        }
    }

    public string Keyword
    {
        get => obsidianQuerySetting.Keyword;
        set
        {
            obsidianQuerySetting.Keyword = value;
            OnPropertyChanged();
        }
    }

    public event QueryDeletedEventHandler? QueryDeleted;

    [RelayCommand]
    private void OpenQuerySettings()
    {
        queryService.ShowQuerySettingView(obsidianQuerySetting, windowManager);
        windowManager.ViewClosed += OnQuerySettingViewClosed;
    }

    private void OnQuerySettingViewClosed()
    {
        ObsidianQuery? query = queryService.GetQuery(obsidianQuerySetting);
        if (query is null)
        {
            windowManager.ViewClosed -= Update;
            QueryDeleted?.Invoke(this);
        }

        Update();
    }

    private void Update()
    {
        windowManager.ViewClosed -= Update;
        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(Keyword));
    }

    [RelayCommand]
    private void SetActionKeyword()
    {
        ActionKeywordDialog dialog = new(obsidianQuerySetting, queryService);
        dialog.ShowDialog();
        OnPropertyChanged(nameof(Keyword));
    }
}
