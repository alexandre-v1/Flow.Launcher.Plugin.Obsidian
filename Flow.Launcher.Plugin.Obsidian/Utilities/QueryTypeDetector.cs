using System.Threading;
using Flow.Launcher.Plugin.Obsidian.Services;

namespace Flow.Launcher.Plugin.Obsidian.Utilities;

public static class QueryTypeDetector
{
    private const string CreateNoteKeyword = NoteCreatorService.NoteCreatorKeyword;
    private const string TagKeyword = "#";

    public static bool ShouldReturnEmptyResults(Query query, CancellationToken token) => string.IsNullOrEmpty(query.Search) || token.IsCancellationRequested;

    public static bool IsNoteCreationSearch(string search) => search.StartsWith(CreateNoteKeyword);

    public static bool IsTagSearch(string search)
    {
        if (search is TagKeyword) return true;
        return search.StartsWith(TagKeyword) && !search.StartsWith(TagKeyword + ' ');
    }
}
