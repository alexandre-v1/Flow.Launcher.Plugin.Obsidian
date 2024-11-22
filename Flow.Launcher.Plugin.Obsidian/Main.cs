using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Services;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using Flow.Launcher.Plugin.Obsidian.Views;

namespace Flow.Launcher.Plugin.Obsidian
{
    public class Obsidian : IPlugin, ISettingProvider, IReloadable
    {
        private IPublicAPI? _publicApi;
        private Settings? _settings;

        public void Init(PluginInitContext context)
        {
            _publicApi = context.API;       
            _settings = _publicApi.LoadSettingJsonStorage<Settings>();
            ReloadData();
        }

        public List<Result> Query(Query query)
        {
            var allFiles = VaultManager.GetAllFiles();
            var resultsMatches = StringMatcher.FindClosestMatches(allFiles, query.Search, maxDistance: 2);

            List<Result> resultList = new();
            foreach ((File file, int distance) in resultsMatches)
            {
                Console.WriteLine($"{file} - {distance}");
                resultList.Add(file.ToResult());
            }
            return resultList;
        }

        public Control CreateSettingPanel()
        {
            if (_settings == null) throw new Exception("Settings not initialized");
            return new SettingsView(_settings, this);
        }

        public void ReloadData()
        {
            if (_settings != null) VaultManager.UpdateVaultList(_settings);
        }
    }
}
