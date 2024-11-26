using System;
using System.Collections.Generic;
using System.IO;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class AliasesService
{
    public static string[]? GetAliases (string filePath)
    {
        string content = File.ReadAllText(filePath);
        if (!content.StartsWith("---")) return null;
        string[] parts = content.Split(new[] { "---" }, 3, StringSplitOptions.None);
        if (parts.Length < 3) return null;

        List<string> aliasListResult = new();
        string frontMatter = parts[1];
        
        var deserializer = new YamlDotNet.Serialization.Deserializer();
        var frontMatterDict = deserializer.Deserialize<Dictionary<string, object>>(frontMatter);

        if (!frontMatterDict.TryGetValue("aliases", out object? aliases))
            return aliasListResult.Count > 0 ? aliasListResult.ToArray() : null;
        
        switch (aliases)
        {
            case List<object> aliasObjectList:
            {
                foreach (object alias in aliasObjectList)
                {
                    if (alias is string aliasString)
                    {
                        aliasListResult.Add(aliasString);
                    }
                }

                break;
            }
            case string singleAlias:
                aliasListResult.Add(singleAlias);
                break;
        }
        return aliasListResult.Count > 0 ? aliasListResult.ToArray() : null;
    }
}
