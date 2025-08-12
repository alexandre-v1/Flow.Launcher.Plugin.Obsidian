using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FilesQuerySetting : BaseQuerySetting
{
    [JsonInclude]
    public List<string>? VaultIds { get; set; }

    [JsonInclude]
    public FileExtensionsSetting FileExtensions { get; set; } = new();

    [JsonInclude]
    public int MaxResult { get; set; }

    [JsonInclude]
    public bool UseAliases { get; set; } = true;

    [JsonInclude]
    public bool AddCreateNoteResult { get; set; } = true;
}
