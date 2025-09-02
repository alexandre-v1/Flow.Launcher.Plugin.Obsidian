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
    private readonly ISettingWindowManager _settingWindowManager;

    public QueriesListViewModel() : this(
    [
        new FilesQuerySetting { Name = "Default Query", Keyword = "ob" },
        new FilesQuerySetting { Name = "Another Query", Keyword = "another keyword" }
    ], null!, null!) { }

    public QueriesListViewModel(List<ObsidianQuerySetting> queriesSettings,
        ISettingWindowManager settingWindowManager,
        IQueryService queryService)
    {
        _queriesSettings = queriesSettings;
        _settingWindowManager = settingWindowManager;
        _queryService = queryService;
        Queries = new ObservableCollection<QueryViewModel>(CreateQueryViewModels());
    }

    public ObservableCollection<QueryViewModel> Queries { get; set; }

    private IEnumerable<QueryViewModel> CreateQueryViewModels() => _queriesSettings.Select(CreateQueryViewModel);

    private QueryViewModel CreateQueryViewModel(ObsidianQuerySetting querySetting) =>
        new(querySetting, _settingWindowManager, _queryService);

    [RelayCommand]
    private void OpenQueryCreator()
    {
        QueryCreatorDialogViewModel viewModel = new(_settingWindowManager, _queryService);
        QueryCreatorDialog queryCreator = new() { DataContext = viewModel };
        viewModel.OnRequestClose += (_, _) => queryCreator.Close();
        viewModel.OnQueryCreated += OnQueryCreated;
        queryCreator.ShowDialog();
    }

    private void OnQueryCreated(ObsidianQuery query)
    {
        QueryViewModel queryViewModel = CreateQueryViewModel(query.Setting);
        Queries.Add(queryViewModel);
    }
}
