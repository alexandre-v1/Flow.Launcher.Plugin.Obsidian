using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

namespace Flow.Launcher.Plugin.Obsidian.ViewModels;

public class QueriesListViewModel : BaseModel
{
    private readonly IQueryService _queryService;
    private readonly ISettingWindowManager _settingWindowManager;
    private readonly IVaultManager _vaultManager;

    public QueriesListViewModel(List<ObsidianQuerySetting> queriesSettings, ISettingWindowManager settingWindowManager,
        IVaultManager vaultManager, IQueryService queryService)
    {
        _settingWindowManager = settingWindowManager;
        _vaultManager = vaultManager;
        _queryService = queryService;
        Queries = CreateQueriesViewModelList(queriesSettings);
    }


    public QueriesListViewModel()
    {
        _settingWindowManager = null!;
        _vaultManager = null!;
        _queryService = null!;

        List<ObsidianQuerySetting> queriesSettings =
        [
            new FilesQuerySetting { Name = "Default Query", Keyword = "ob" },
            new FilesQuerySetting { Name = "Another Query", Keyword = "another keyword" }
        ];
        Queries = CreateQueriesViewModelList(queriesSettings);
    }

    public List<QueryViewModel> Queries { get; }

    private List<QueryViewModel> CreateQueriesViewModelList(List<ObsidianQuerySetting> queriesSettings) =>
        queriesSettings
            .Select(querySetting =>
                new QueryViewModel(querySetting, _settingWindowManager, _vaultManager, _queryService))
            .ToList();
}
