using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.Obsidian.Models;

public class FilesQuerySetting : ObsidianQuerySetting
{
    [JsonInclude]
    public HashSet<string>? VaultIds { get; set; }

    [JsonInclude]
    public FileExtensionsSetting FileExtensions { get; set; } = new();

    [JsonInclude]
    public IList<string> RelativeExcludePaths { get; set; } = [];

    [JsonInclude]
    public int MaxResult { get; set; }

    [JsonInclude]
    public bool UseAliases { get; set; } = true;

    [JsonInclude]
    public bool AddCreateNoteResult { get; set; } = true;

    public bool VaultIsActive(string vaultId) => VaultIds is not null && VaultIds.Contains(vaultId);

    public void SetVaultActiveState(string vaultId, bool isActive)
    {
        if (VaultIds is null)
        {
            return;
        }

        HashSet<string> vaultIds = new(VaultIds);
        if (isActive)
        {
            vaultIds.Add(vaultId);
        }
        else
        {
            vaultIds.Remove(vaultId);
        }

        VaultIds = vaultIds;
    }
}
