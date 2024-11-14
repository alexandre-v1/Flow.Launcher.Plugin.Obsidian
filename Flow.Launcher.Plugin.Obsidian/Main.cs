using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.Obsidian
{
    public class Obsidian : IPlugin
    {
        private PluginInitContext? _context;
        private IPublicAPI? _publicApi;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _publicApi = context.API;
            VaultManager.UpdateVaultList(_publicApi);
            
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
    }
}
