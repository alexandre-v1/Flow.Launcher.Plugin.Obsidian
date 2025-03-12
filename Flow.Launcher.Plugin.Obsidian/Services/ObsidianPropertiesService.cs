using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class ObsidianPropertiesService
{
    private const string YamlFrontMatterDelimiter = "---";
    private const string AliasesKey = "aliases:";
    private const int AliasesKeyLength = 8;
    private const string TagsKey = "tags:";
    private const int TagsKeyLength = 5;


    public static File AddObsidianProperties(this File file, bool useAliases, bool useTags)
    {
        using StreamReader reader = new(file.FilePath);
        if (reader.ReadLine()?.Trim() is not YamlFrontMatterDelimiter) return file;

        List<string>? aliases = useAliases ? new List<string>() : null;
        List<string>? tags = useTags ? new List<string>() : null;

        List<string>? currentList = null;

        while (reader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (line is YamlFrontMatterDelimiter) break;

            if (line.Length is 0) continue;

            if (line[0] is ':' or '#')
            {
                currentList = null;
                continue;
            }

            if (useAliases && line.StartsWith(AliasesKey))
            {
                currentList = aliases;
                ParseLine(line[AliasesKeyLength..].Trim(), aliases!);
                continue;
            }

            if (useTags && line.StartsWith(TagsKey))
            {
                currentList = tags;
                ParseLine(line[TagsKeyLength..].Trim(), tags!);
                continue;
            }

            if (currentList is null) continue;
            if (line[0] is '-')
            {
                currentList.Add(line[1..].Trim().Trim('"'));
            }
            else if (!line.Contains(':'))
            {
                ParseLine(line, currentList);
            }
            else
            {
                currentList = null;
            }
        }

        if (useAliases && aliases?.Count > 0)
        {
            file.Aliases = aliases.ToArray();
        }
        if (useTags && tags?.Count > 0)
        {
            if(tags.Count is 0 || tags.All(string.IsNullOrWhiteSpace)) return file;
            file.Tags = tags.ToArray();
            VaultManager.AddTagsToList(tags);
        }

        return file;
    }

    private static void ParseLine(string value, List<string> entriesList)
    {
        if (!value.StartsWith('['))
        {
            if (!string.IsNullOrWhiteSpace(value))
                entriesList.Add(value.Trim('"'));
            return;
        }

        ReadOnlySpan<char> span = value.AsSpan()
            .TrimStart('[')
            .TrimEnd(']');

        entriesList.AddRange(span.ToString().Split(',')
            .Select(entry => entry.Trim().Trim('"'))
            .Where(trimmed => trimmed.Length > 0));
    }
}
