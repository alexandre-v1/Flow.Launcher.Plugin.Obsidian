using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class ContentSearch
{
    private const int TitleBaseScore = 10;
    private const int EndOfLineScore = 3;
    private const int StartOfLineScore = 3;
    private const int SpaceScore = 2;
    private const int WordBreakingCharScore = 1;
    private const int SameCaseScore = 1;

    private const string YamlFrontMatter = "---";
    private const string ObsidianLinkStart = "[[";
    private const string ObsidianLinkEnd = "]]";
    private const string LinkStart = "](";
    private const char LinkEnd = ')';
    private const char LinkPrefix = '[';

    private const int IndexNotFound = -1;

    public static async Task<ContentSearchMatch?> GetBestMatchInFile(File file, string[] searchTerms)
    {
        using StreamReader reader = new(file.FilePath);

        ContentSearchMatch? bestMatch = null;
        bool inFrontMatter = false;

        while (await reader.ReadLineAsync() is { } currentLine)
        {
            if (string.IsNullOrWhiteSpace(currentLine)) continue;
            if (IsYamlFrontMatter(currentLine, reader.BaseStream.Position, ref inFrontMatter)) return bestMatch;

            string? cleanLine = CleanLine(currentLine);
            if (cleanLine is null) continue;

            ContentSearchMatch? bestMatchInLine = BestMatchInLine(searchTerms, cleanLine);
            if (bestMatchInLine is not null && (bestMatch is null || bestMatchInLine.Score < bestMatch.Score))
            {
                bestMatch = bestMatchInLine;
            }
        }

        return bestMatch;
    }

    private static bool IsYamlFrontMatter(string line, long streamPosition, ref bool inFrontMatter)
    {
        if (streamPosition is 0 && line.Trim() is YamlFrontMatter)
        {
            inFrontMatter = true;
            return true;
        }

        if (!inFrontMatter) return false;

        if (line.Trim() is YamlFrontMatter)
        {
            inFrontMatter = false;
        }

        return true;
    }

    private static string? CleanLine(string line)
    {
        line = RemoveObsidianLinks(line);
        line = RemoveMarkdownLinks(line);
        line = line.Trim();

        if (line.Length <= 3) return null;
        return string.IsNullOrWhiteSpace(line) ? null : line;
    }

    private static string RemoveMarkdownLinks(string line)
    {
        StringBuilder result = new(line.Length);
        int currentPosition = 0;

        while (true)
        {
            int prefix = line.IndexOf(LinkPrefix, currentPosition);
            if (prefix is IndexNotFound)
            {
                result.Append(line[currentPosition..]);
                break;
            }

            int linkStart = line.IndexOf(LinkStart, prefix, StringComparison.Ordinal);
            if (linkStart is IndexNotFound)
            {
                result.Append(line[currentPosition..]);
                break;
            }

            int linkEnd = line.IndexOf(LinkEnd, linkStart);
            if (linkEnd is IndexNotFound)
            {
                result.Append(line[currentPosition..]);
                break;
            }

            result.Append(line[currentPosition..prefix]);
            currentPosition = linkEnd + 1;
        }

        return result.ToString();
    }

    private static string RemoveObsidianLinks(string line)
    {
        StringBuilder result = new(line.Length);
        int currentPosition = 0;

        while (true)
        {
            int linkStart = line.IndexOf(ObsidianLinkStart, currentPosition, StringComparison.Ordinal);
            if (linkStart is IndexNotFound)
            {
                result.Append(line[currentPosition..]);
                break;
            }

            int linkEnd = line.IndexOf(ObsidianLinkEnd, linkStart, StringComparison.Ordinal);
            if (linkEnd is IndexNotFound)
            {
                result.Append(line[currentPosition..]);
                break;
            }

            result.Append(line[currentPosition..linkStart]);
            currentPosition = linkEnd + ObsidianLinkEnd.Length;
        }

        return result.ToString();
    }

    private static ContentSearchMatch? BestMatchInLine(string searchTerm, string cleanLine)
    {
        ContentSearchMatch? bestMatch = null;
        int currentPosition = 0;

        while (true)
        {
            int matchIndex = cleanLine.IndexOf(searchTerm, currentPosition, StringComparison.CurrentCultureIgnoreCase);
            if (matchIndex is IndexNotFound) return bestMatch;

            int score = ScoreLineMatch(searchTerm, cleanLine, matchIndex);

            if (bestMatch is null || score > bestMatch.Score)
            {
                bestMatch = new ContentSearchMatch(score, cleanLine, matchIndex, searchTerm);
            }

            currentPosition = matchIndex + searchTerm.Length;
        }
    }

    private static ContentSearchMatch? BestMatchInLine(string[] searchTerms, string cleanLine)
    {
        ContentSearchMatch? bestMatch = null;
        foreach (string searchTerm in searchTerms)
        {
            ContentSearchMatch? searchTermMatch = BestMatchInLine(searchTerm, cleanLine);
            if (searchTermMatch is not null && (bestMatch is null || searchTermMatch.Score > bestMatch.Score))
            {
                bestMatch = searchTermMatch;
            }
        }

        return bestMatch;
    }

    private static int ScoreLineMatch(string searchTerm, string cleanLine, int matchIndex)
    {
        int score = 0;

        // Same case bonus
        if (cleanLine[matchIndex..].StartsWith(searchTerm)) score += SameCaseScore;

        // Before search term scoring
        if (matchIndex is 0)
        {
            score += StartOfLineScore;
        }
        else
        {
            char beforeChar = cleanLine[matchIndex - 1];
            score += ScoreCharacterContext(beforeChar);

            // In title scoring
            string trimmedLine = cleanLine.TrimStart();
            if (trimmedLine.StartsWith('#'))
            {
                int headerLevel = trimmedLine.TakeWhile(c => c is '#').Count();
                score += TitleBaseScore - Math.Min(headerLevel - 1, 5);
            }
        }

        // After search term scoring
        int endMatchIndex = matchIndex + searchTerm.Length;
        if (endMatchIndex >= cleanLine.Length)
        {
            score += EndOfLineScore;
        }
        else
        {
            char afterChar = cleanLine[endMatchIndex];
            score += ScoreCharacterContext(afterChar);
        }

        return score;
    }

    private static int ScoreCharacterContext(char character)
    {
        if (char.IsWhiteSpace(character)) return SpaceScore;
        return SearchUtility.IsWordBreakingChar(character) ? WordBreakingCharScore : 0;
    }
}
