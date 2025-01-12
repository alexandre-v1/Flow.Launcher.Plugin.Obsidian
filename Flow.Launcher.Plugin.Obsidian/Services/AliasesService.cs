using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class AliasesService
{
    public static string[]? GetAliases(string filePath, YamlDotNet.Serialization.Deserializer deserializer)
    {
        using var reader = new StreamReader(filePath);

        if (reader.ReadLine() != "---")
            return null;

        var yamlContentBuilder = new StringBuilder();
        
        while (reader.ReadLine() is { } line)
        {
            if (line == "---")
                break;
            yamlContentBuilder.AppendLine(line);
        }
        
        if (yamlContentBuilder.Length == 0)
            return null;
        
        var frontMatterDict = deserializer.Deserialize<Dictionary<string, object>>(yamlContentBuilder.ToString());
        if (!frontMatterDict.TryGetValue("aliases", out object? aliases))
            return null;

        var aliasListResult = new List<string>();
        switch (aliases)
        {
            case IEnumerable<object> aliasList:
                foreach (object alias in aliasList)
                {
                    if (alias is string aliasString)
                        aliasListResult.Add(aliasString);
                }
                break;
            case string singleAlias:
                aliasListResult.Add(singleAlias);
                break;
        }

        return aliasListResult.Count > 0 ? aliasListResult.ToArray() : null;
    }
}
