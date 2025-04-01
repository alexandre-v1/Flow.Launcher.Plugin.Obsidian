using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using File = System.IO.File;

namespace Flow.Launcher.Plugin.Obsidian.Managers;

public class VaultManager(Settings settings)
{
    public bool HasOnlyOneVault => Vaults.Count is 1;
    public bool OneVaultHasAdvancedUri => Vaults.Any(vault => vault.HasAdvancedUri);
    public bool AllVaultsHaveAdvancedUri => Vaults.All(vault => vault.HasAdvancedUri);
    public readonly Settings Settings = settings;

    public HashSet<Vault> Vaults { get; private set; } = [];

    public async Task UpdateVaultListAsync()
    {
        Vaults = [];
        string jsonString = await File.ReadAllTextAsync(Paths.VaultListJsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);

        JsonElement root = document.RootElement;
        JsonElement vaults = root.GetProperty("vaults");

        List<(Vault Vault, Task LoadingTask)> vaultLoadTasks = [];

        foreach (JsonProperty vault in vaults.EnumerateObject())
        {
            string vaultId = vault.Name;
            string? path = vault.Value.GetProperty("path").GetString();
            if (path is null) continue;

            Settings.VaultsSetting.TryGetValue(vaultId, out VaultSetting? vaultSetting);
            if (vaultSetting is null)
            {
                vaultSetting = new VaultSetting();
                Settings.VaultsSetting.Add(vaultId, vaultSetting);
            }

            Vault newVault = new(vaultId, path, vaultSetting, this);
            Task loadingTask = newVault.LoadFilesAsync();
            vaultLoadTasks.Add((newVault, loadingTask));
        }

        if (!OneVaultHasAdvancedUri)
        {
            Settings.GlobalVaultSetting.OpenInNewTabByDefault = false;
        }

        foreach ((Vault vault, Task loadingTask) in vaultLoadTasks)
        {
            await loadingTask;
            Vaults.Add(vault);
        }
    }

    public Vault? GetVaultWithId(string vaultId) => Vaults.FirstOrDefault(vault => vault.Id == vaultId);
}
