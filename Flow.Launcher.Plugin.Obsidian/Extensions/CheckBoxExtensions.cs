using System.Collections.Generic;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;

namespace Flow.Launcher.Plugin.Obsidian.Extensions;

public static class CheckBoxExtensions
{
    private const string MarkedCheckBoxString = "- [x] ";
    private const string CheckBoxString = "- [ ] ";

    public static List<Result> GetCheckBoxes(this File file)
    {
        string path = file.FilePath;
        List<Result> checkBoxes = [];
        string[] lines = System.IO.File.ReadAllLines(path);

        string prevLine = string.Empty;
        foreach (string line in lines)
        {
            string title;
            bool isChecked;

            if (line.Contains(MarkedCheckBoxString))
            {
                isChecked = true;
                title = line.Remove(MarkedCheckBoxString);
            }
            else if (line.Contains(CheckBoxString))
            {
                title = line.Remove(CheckBoxString);
                isChecked = false;
            }
            else
            {
                if (line.Length > 0)
                    prevLine = line;
                continue;
            }

            string subTitle = string.Empty;
            string trim = prevLine.Trim();
            if (trim.EndsWith(':')) subTitle = trim[..^1];

            Result item = new()
            {
                Glyph = new GlyphInfo(Font.Family, isChecked ? Font.MarkedCheckBoxGlyph : Font.CheckBoxGlyph),
                Title = title.Trim(),
                SubTitle = subTitle,
                Action = _ =>
                {
                    file.TryToggleCheckBox(line, isChecked);
                    return false;
                }
            };
            checkBoxes.Add(item);
        }

        return checkBoxes;
    }

    private static void TryToggleCheckBox(this File file, string checkBoxLine, bool isChecked)
    {
        string filePath = file.FilePath;
        string[] lines = System.IO.File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            if (!lines[i].Contains(checkBoxLine)) continue;

            if (isChecked)
                lines[i] = lines[i].Replace(MarkedCheckBoxString, CheckBoxString);
            else
                lines[i] = lines[i].Replace(CheckBoxString, MarkedCheckBoxString);
            break;
        }

        System.IO.File.WriteAllLines(filePath, lines);
    }
}
