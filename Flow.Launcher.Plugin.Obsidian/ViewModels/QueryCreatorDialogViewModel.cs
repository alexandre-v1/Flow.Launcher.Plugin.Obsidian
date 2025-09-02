using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class QueryCreatorDialogViewModel : BaseModel
{
    public delegate void QueryCreatedEventHandler(ObsidianQuery query);

    private readonly IQueryService _queryService;
    private readonly ISettingWindowManager _settingWindowManager;

    [UsedImplicitly] // For design-time data
    public QueryCreatorDialogViewModel() : this(null!, null!) { }

    public QueryCreatorDialogViewModel(ISettingWindowManager settingWindowManager, IQueryService queryService)
    {
        QueryTypes.Add(new ObsidianQueryInfoViewModel(FilesQuery.QueryInfo));
        _settingWindowManager = settingWindowManager;
        _queryService = queryService;
    }

    public string QueryNameInput { get; set; } = string.Empty;
    public ObsidianQueryInfoViewModel? SelectedQueryType { get; set; }

    public List<ObsidianQueryInfoViewModel> QueryTypes { get; } = [];

    public event EventHandler? OnRequestClose;
    public event QueryCreatedEventHandler? OnQueryCreated;

    [RelayCommand]
    private void CreateQuery(object? queryInfoObject)
    {
        if (queryInfoObject is null || string.IsNullOrWhiteSpace(QueryNameInput))
        {
            return;
        }

        ObsidianQueryInfoViewModel queryInfo = (ObsidianQueryInfoViewModel)queryInfoObject;
        ObsidianQuery query = _queryService.CreateQuery(queryInfo.QueryType, QueryNameInput);
        _queryService.ShowQuerySettingView(query.Setting, _settingWindowManager);

        OnQueryCreated?.Invoke(query);
        OnRequestClose?.Invoke(this, EventArgs.Empty);
    }
}
