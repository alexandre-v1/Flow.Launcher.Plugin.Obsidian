using System.Text;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public record ContentSearchMatch(int Score, string Line, int MatchIndex, string SearchTerm)
{
    public readonly int Score = Score;

    public string ExtractMatchedWord()
    {
        string matchedWord = Line.Substring(MatchIndex, SearchTerm.Length);
        StringBuilder stringBuilder = new(matchedWord);

        int endMatchIndex = MatchIndex + SearchTerm.Length;
        for (int i = endMatchIndex; i < Line.Length; i++)
        {
            if (char.IsLetterOrDigit(Line[i]))
            {
                stringBuilder.Append(Line[i]);
            }
            else
            {
                break;
            }
        }

        return stringBuilder.ToString();
    }
}
