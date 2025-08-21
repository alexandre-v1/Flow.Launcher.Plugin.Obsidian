using System.Windows;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services.Interfaces;

namespace Flow.Launcher.Plugin.Obsidian.Views;

public partial class ActionKeywordDialog
{
    private readonly ObsidianQuerySetting _obsidianQuerySetting;
    private readonly IQueryService _queryService;

    public ActionKeywordDialog(ObsidianQuerySetting obsidianQuerySetting, IQueryService queryService)
    {
        _obsidianQuerySetting = obsidianQuerySetting;
        _queryService = queryService;
        InitializeComponent();
    }

    private void ActionKeyword_OnLoaded(object sender, RoutedEventArgs e)
    {
        TbOldActionKeyword.Text = _obsidianQuerySetting.Keyword;
        tbAction.Text = TbOldActionKeyword.Text;
        tbAction.SelectAll();
        tbAction.Focus();
    }

    private void BtnCancel_OnClick(object sender, RoutedEventArgs e) => Close();

    private void btnDone_OnClick(object sender, RoutedEventArgs _)
    {
        string newActionKeyword = tbAction.Text.Trim();

        if (string.IsNullOrEmpty(newActionKeyword))
        {
            newActionKeyword = Query.GlobalPluginWildcardSign;
        }

        ChangeActionKeyword(newActionKeyword);
    }

    private void ChangeActionKeyword(string newKeyword)
    {
        bool result = _queryService.TryChangeKeyword(_obsidianQuerySetting, newKeyword);
        if (result)
        {
            Close();
        }
    }
}
