using System.Collections.Generic;
using Flow.Launcher.Plugin.Obsidian.Models;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public static class CheckBoxService
{
    private const string MarkedCheckBox = "- [x] ";
    private const string CheckBox = "- [ ] ";
    private const string FontFamily = "/Resources/#Segoe Fluent Icons";
    private const string MarkedCheckBoxGlyph = "\uE73D";
    private const string CheckBoxGlyph = "\uE739";
    

    public static List<Result> GetCheckBoxes(this File file)
    {
        string path = file.FilePath;
        var checkBoxes = new List<Result>();
        string[] lines = System.IO.File.ReadAllLines(path);

        string prevLine = string.Empty;
        foreach (string line in lines)
        {
            string title;
            bool isChecked;

            if (line.Contains(MarkedCheckBox))
            { 
                isChecked = true;
                title = line.Replace(MarkedCheckBox, "");
            }
            else if (line.Contains(CheckBox))
            {
                title = line.Replace(CheckBox, "");
                isChecked = false;
            }
            else
            {
                if(line.Length > 0)
                    prevLine = line;
                continue;
            }

            string subTitle = string.Empty;
            string trim = prevLine.Trim();
            if (trim.EndsWith(":"))
            {
                subTitle = trim[..^1];
            }

            var item = new Result
            {
                Glyph = new GlyphInfo(FontFamily, isChecked ? MarkedCheckBoxGlyph : CheckBoxGlyph),
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
                lines[i] = lines[i].Replace(MarkedCheckBox, CheckBox);
            else
                lines[i] = lines[i].Replace(CheckBox, MarkedCheckBox);
            break;
        }
        
        System.IO.File.WriteAllLines(filePath, lines);
    }
}