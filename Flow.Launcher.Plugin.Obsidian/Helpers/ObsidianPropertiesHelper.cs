using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian.Helpers;

public static class ObsidianPropertiesHelper
{
    private const string YamlFrontMatterDelimiter = "---";
    private const string AliasesKey = "aliases:";
    private const int AliasesKeyLength = 8;
    private const string TagsKey = "tags:";
    private const int TagsKeyLength = 5;


    public static File AddObsidianProperties(File file, bool useAliases, bool useTags)
    {
        using StreamReader reader = new(file.FilePath);
        if (reader.ReadLine()?.Trim() is not YamlFrontMatterDelimiter) return file;

        List<string>? aliases = useAliases ? [] : null;
        List<string>? tags = useTags ? [] : null;

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

            if (aliases is not null && line.StartsWith(AliasesKey))
            {
                currentList = aliases;
                ParseLine(line[AliasesKeyLength..].Trim(), aliases);
                continue;
            }

            if (tags is not null && line.StartsWith(TagsKey))
            {
                currentList = tags;
                ParseLine(line[TagsKeyLength..].Trim(), tags);
                continue;
            }

            if (currentList is null) continue;
            if (line[0] is '-')
            {
                currentList.Add(CleanValue(line[1..]));
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
            file.Aliases = aliases.ToHashSet(StringComparer.CurrentCultureIgnoreCase);
        }

        if (!useTags || !(tags?.Count > 0)) return file;

        if (tags.Count is 0 || tags.All(string.IsNullOrWhiteSpace)) return file;
        file.Tags = tags.ToHashSet(StringComparer.CurrentCultureIgnoreCase);

        return file;
    }

    public static string BuildYamlFrontMatterWithTags(IReadOnlySet<string> tags)
    {
        const string yamlDelimiter = "---";
        string tagsList = tags.JoinToString("\n  - ");

        return $"{yamlDelimiter}\n" +
               $"tags:\n" +
               $"  - {tagsList}\n" +
               $"{yamlDelimiter}\n";
    }

    private static void ParseLine(string value, List<string> entriesList)
    {
        if (!value.StartsWith('['))
        {
            string cleanValue = CleanValue(value);
            if (!string.IsNullOrWhiteSpace(cleanValue))
                entriesList.Add(cleanValue);
            return;
        }

        ReadOnlySpan<char> span = value.AsSpan()
            .TrimStart('[')
            .TrimEnd(']');

        entriesList.AddRange(span.ToString().Split(',')
            .Select(CleanValue)
            .Where(trimmed => trimmed.Length > 0));
    }

    private static string CleanValue(string value) => value.Trim().Trim('"').TrimStart('#');
}
