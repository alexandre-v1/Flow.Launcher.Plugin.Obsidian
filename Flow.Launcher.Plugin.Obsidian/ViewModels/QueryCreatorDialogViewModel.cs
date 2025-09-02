using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class QueryCreatorDialogViewModel : BaseModel
{
    public delegate void QueryCreatedEventHandler(ObsidianQuery query);

    public delegate void RequestCloseEventHandler();

    private readonly IQueryService _queryService;

    [UsedImplicitly] // For design-time data
    public QueryCreatorDialogViewModel() : this(null!) { }

    public QueryCreatorDialogViewModel(IQueryService queryService)
    {
        QueryTypes.Add(new ObsidianQueryInfoViewModel(FilesQuery.QueryInfo));
        _queryService = queryService;
    }

    public string QueryNameInput { get; set; } = string.Empty;
    public ObsidianQueryInfoViewModel? SelectedQueryType { get; set; }

    public List<ObsidianQueryInfoViewModel> QueryTypes { get; } = [];

    public event RequestCloseEventHandler? RequestClose;
    public event QueryCreatedEventHandler? QueryCreated;

    [RelayCommand]
    private void CreateQuery(object? queryInfoObject)
    {
        if (queryInfoObject is null || string.IsNullOrWhiteSpace(QueryNameInput))
        {
            return;
        }

        ObsidianQueryInfoViewModel queryInfo = (ObsidianQueryInfoViewModel)queryInfoObject;
        ObsidianQuery query = _queryService.CreateQuery(queryInfo.QueryType, QueryNameInput);

        QueryCreated?.Invoke(query);
        RequestClose?.Invoke();
    }
}
