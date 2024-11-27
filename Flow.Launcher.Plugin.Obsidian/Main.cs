using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.Views;

namespace Flow.Launcher.Plugin.Obsidian
{
    public class Obsidian : IPlugin, ISettingProvider, IReloadable
    {
        private IPublicAPI _publicApi = null!;
        private Settings _settings = null!;

        public void Init(PluginInitContext context)
        {
            _publicApi = context.API;       
            _settings = _publicApi.LoadSettingJsonStorage<Settings>();
            ReloadData();
        }

        public List<Result> Query(Query query)
        {
            string search = query.Search.Trim();
            if (string.IsNullOrEmpty(search))
                return new List<Result>();

            var files = VaultManager.GetAllFiles();
            var results = SearchService.GetSearchResults(files, search, _settings.UseAliases);
            if (_settings.MaxResult > 0)
                results = SearchService.SortAndTruncateResults(results, _settings.MaxResult);
            
            return results;
        }

        public Control CreateSettingPanel()
        {
            return new SettingsView(_settings, this);
        }

        public void ReloadData()
        {
            VaultManager.UpdateVaultList(_settings);
        }
    }
}
