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
            var helloWorldResult = new Result
            {
                Title = "Hello World",
                SubTitle = "This is a subtitle",
                IcoPath = "Images\\app.png",
                Action = e =>
                {
                    _context?.API.ShowMsg("Hello World");
                    return true;
                }
            };
            
            List<string> allFiles = new();
            foreach (Vault vault in VaultManager.Vaults)
            {
                if (vault.Files != null) allFiles.AddRange(vault.Files);
            }
            
            var resultsMatches = StringMatcher.FindClosestMatches(allFiles, query.Search, maxDistance: 2);

            string? debug = ""; 
            resultsMatches = resultsMatches.Take(10).ToList();
            foreach ((string? text, int distance) in resultsMatches)
            {
                debug += text + " \n";
            }
            _context?.API.ShowMsg(debug);
            return resultsMatches.Select(variable => new Result { Title = variable.Text, }).ToList();
        }
    }
}
