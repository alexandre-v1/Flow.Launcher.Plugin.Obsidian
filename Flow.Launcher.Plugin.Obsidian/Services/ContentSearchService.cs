using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class ContentSearchService
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
    private const int IndexNotFound = -1;

    public async Task<List<File>> SearchAndScoreFilesByContentAsync(List<File> files, string search)
    {
        File[] task = await Task.WhenAll(files.Select(async file => await ProcessFileAsync(file, search)));
        return task.ToList();
    }

    private static bool SkipYamlFrontMatter(string line, long streamPosition, ref bool inFrontMatter)
    {
        if (streamPosition is 0 && line.Trim() is YamlFrontMatter)
        {
            inFrontMatter = true;
            return true;
        }

        if (!inFrontMatter) return false;

        if (line.Trim() is YamlFrontMatter)
            inFrontMatter = false;
        return true;
    }

    private static string RemoveObsidianLinks(string line)
    {
        while (line.Contains(ObsidianLinkStart, StringComparison.Ordinal) &&
               line.Contains(ObsidianLinkEnd, StringComparison.Ordinal))
        {
            int linkStart = line.IndexOf(ObsidianLinkStart, StringComparison.Ordinal);
            if (linkStart is IndexNotFound) break;

            int linkEnd = line.IndexOf(ObsidianLinkEnd, linkStart, StringComparison.Ordinal);
            if (linkEnd is IndexNotFound) break;

            string beforeLink = line[..linkStart];
            string afterLink = line[(linkEnd + 2)..];
            line = beforeLink + afterLink;
        }

        return line;
    }

    private async Task<File> ProcessFileAsync(File file, string search)
    {
        if (Path.GetExtension(file.FilePath) is not ".md") return file;
        if (file.HasTag("excalidraw")) return file;

        using StreamReader reader = new(file.FilePath);

        (int score, string line, int matchIndex) bestMatch = (0, string.Empty, IndexNotFound);
        bool inFrontMatter = false;

        while (await reader.ReadLineAsync() is { } currentLine)
        {
            if (SkipYamlFrontMatter(currentLine, reader.BaseStream.Position, ref inFrontMatter)) continue;

            string cleanLine = RemoveObsidianLinks(currentLine);
            string remainingLine = cleanLine;
            int currentPosition = 0;

            while (true)
            {
                int index = remainingLine.IndexOf(search, StringComparison.CurrentCultureIgnoreCase);
                if (index is IndexNotFound) break;

                int absoluteIndex = currentPosition + index;
                int score = 0;

                // Same case bonus
                if (remainingLine[index..].StartsWith(search)) score += SameCaseScore;

                // Before search term scoring
                if (absoluteIndex is 0)
                {
                    score += StartOfLineScore;
                }
                else
                {
                    char beforeChar = cleanLine[absoluteIndex - 1];
                    if (char.IsWhiteSpace(beforeChar)) score += SpaceScore;
                    else if (SearchUtility.IsWordBreakingChar(beforeChar)) score += WordBreakingCharScore;

                    // Title scoring
                    string trimmedLine = cleanLine.TrimStart();
                    if (trimmedLine.StartsWith("#"))
                    {
                        int headerLevel = trimmedLine.TakeWhile(c => c is '#').Count();
                        score += TitleBaseScore - Math.Min(headerLevel - 1, 5);
                    }
                }

                // After search term scoring
                int endIndex = absoluteIndex + search.Length;
                if (endIndex >= cleanLine.Length)
                {
                    score += EndOfLineScore;
                }
                else
                {
                    char afterChar = cleanLine[endIndex];
                    if (char.IsWhiteSpace(afterChar)) score += SpaceScore;
                    else if (SearchUtility.IsWordBreakingChar(afterChar)) score += WordBreakingCharScore;
                }

                if (score > bestMatch.score)
                {
                    bestMatch = (score, cleanLine, absoluteIndex);
                }

                // Move to next potential match
                currentPosition += index + search.Length;
                remainingLine = cleanLine[currentPosition..];
            }
        }

        if (bestMatch.score <= 0 || bestMatch.score <= file.Score || bestMatch.matchIndex is IndexNotFound) return file;

        string matchedWord = bestMatch.line.Substring(bestMatch.matchIndex, search.Length);
        for (int i = bestMatch.matchIndex + search.Length; i < bestMatch.line.Length; i++)
        {
            if (char.IsLetterOrDigit(bestMatch.line[i]))
            {
                matchedWord += bestMatch.line[i];
            }
            else
            {
                break;
            }
        }

        file.Title = file.Name + $" - {matchedWord}";
        file.Score = bestMatch.score;

        return file;
    }
}
