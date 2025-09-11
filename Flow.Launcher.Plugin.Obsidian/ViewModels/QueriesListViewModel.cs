using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;
using Flow.Launcher.Plugin.Obsidian.Views;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public partial class QueriesListViewModel : BaseModel
{
    private readonly List<ObsidianQuerySetting> _queriesSettings;
    private readonly IQueryService _queryService;
    private readonly ISettingsWindowManager _windowManager;

    public QueriesListViewModel() : this(
    [
        new FilesQuerySetting { Name = "Default Query", Keyword = "ob" },
        new FilesQuerySetting { Name = "Another Query", Keyword = "another keyword" }
    ], null!, null!) { }

    public QueriesListViewModel(List<ObsidianQuerySetting> queriesSettings,
        ISettingsWindowManager windowManager,
        IQueryService queryService)
    {
        _queriesSettings = queriesSettings;
        _windowManager = windowManager;
        _queryService = queryService;
        Queries = new ObservableCollection<QueryViewModel>(CreateQueryViewModels());
    }

    public ObservableCollection<QueryViewModel> Queries { get; }

    private IEnumerable<QueryViewModel> CreateQueryViewModels() => _queriesSettings.Select(CreateQueryViewModel);

    private QueryViewModel CreateQueryViewModel(ObsidianQuerySetting querySetting)
    {
        QueryViewModel queryViewModel = new(querySetting, _windowManager, _queryService);
        queryViewModel.QueryDeleted += OnQueryDeleted;
        return queryViewModel;
    }

    [RelayCommand]
    private void OpenQueryCreator()
    {
        QueryCreatorDialogViewModel viewModel = new(_queryService);
        QueryCreatorDialog queryCreator = new() { DataContext = viewModel };
        viewModel.RequestClose += queryCreator.Close;
        viewModel.QueryCreated += OnQueryCreated;
        queryCreator.ShowDialog();
    }

    private void OnQueryCreated(ObsidianQuery query)
    {
        QueryViewModel queryViewModel = CreateQueryViewModel(query.Setting);
        Queries.Add(queryViewModel);
        queryViewModel.OpenQuerySettingsCommand.Execute(null);
    }

    private void OnQueryDeleted(QueryViewModel queryViewModel)
    {
        queryViewModel.QueryDeleted -= OnQueryDeleted;
        Queries.Remove(queryViewModel);
    }
}
