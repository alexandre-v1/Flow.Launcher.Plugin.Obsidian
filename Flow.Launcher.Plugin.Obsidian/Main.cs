using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Obsidian
{
    public class Obsidian : IPlugin, ISettingProvider
    {
        private IPublicAPI? _publicApi;
        private Settings? _settings;

        public void Init(PluginInitContext context)
        {
            _publicApi = context.API;       
            VaultManager.UpdateVaultList();
            _settings = _publicApi.LoadSettingJsonStorage<Settings>();
            VaultManager.UpdateVaultList(_settings);
        }

        public List<Result> Query(Query query)
        {
            Console.WriteLine($"Query received: {query.Search}");
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
            return new SettingsView(_publicApi);
        }
    }
}
