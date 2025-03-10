using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class AliasesService
{
    private const string YamlFrontMatterDelimiter = "---";
    private const string AliasesKey = "aliases:";
    private const int AliasesKeyLength = 8;

    public static string[]? GetAliases(string filePath)
    {
        using StreamReader reader = new(filePath);
        if (reader.ReadLine()?.Trim() is not YamlFrontMatterDelimiter) return null;

        bool inAliases = false;
        var aliases = new List<string>();

        while (reader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (line is YamlFrontMatterDelimiter) break;

            if (line.StartsWith(AliasesKey))
            {
                inAliases = true;
                ParseAliasLine(line[AliasesKeyLength..].Trim(), aliases);
                continue;
            }

            if (!inAliases) continue;
            if (line.StartsWith('-'))
            {
                aliases.Add(line[1..].Trim().Trim('"'));
            }
            else if (line.Contains(':'))
            {
                break;
            }
            else
            {
                ParseAliasLine(line.Trim(), aliases);
            }
        }

        return aliases.Count > 0 ? aliases.ToArray() : null;
    }


    private static void ParseAliasLine(string value, List<string> aliases)
    {
        if (value.StartsWith('['))
        {
            string[] entries = value.TrimStart('[').TrimEnd(']').Split(',');
            aliases.AddRange(entries.Select(entry => entry.Trim().Trim('"')));
        }
        else if (!string.IsNullOrWhiteSpace(value))
        {
            aliases.Add(value.Trim('"'));
        }
    }
}
