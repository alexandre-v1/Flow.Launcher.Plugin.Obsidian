using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Obsidian.Extensions;
using Flow.Launcher.Plugin.Obsidian.Models;
using Flow.Launcher.Plugin.Obsidian.Utilities;
using File = Flow.Launcher.Plugin.Obsidian.Models.File;

namespace Flow.Launcher.Plugin.Obsidian.Services;

public class VaultManager(Settings settings)
{
    public bool HasOnlyOneVault => Vaults.Count is 1;
    public bool OneVaultHasAdvancedUri => Vaults.Any(vault => vault.HasAdvancedUri);
    public bool AllVaultsHaveAdvancedUri => Vaults.All(vault => vault.HasAdvancedUri);
    public HashSet<string> TagsList => [.._tagsList.Keys];
    private List<File>? _allFiles;

    public List<Vault> Vaults { get; private set; } = [];
    private readonly ConcurrentDictionary<string, byte> _tagsList = new();
    public readonly Settings Settings = settings;

    public async Task UpdateVaultListAsync()
    {
        Vaults = [];
        string jsonString = await System.IO.File.ReadAllTextAsync(Paths.VaultListJsonPath);
        using JsonDocument document = JsonDocument.Parse(jsonString);

        JsonElement root = document.RootElement;
        JsonElement vaults = root.GetProperty("vaults");

        var vaultLoadTasks = new List<(Vault Vault, Task LoadingTask)>();

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

            var newVault = new Vault(vaultId, path, vaultSetting, this);
            var loadingTask = newVault.LoadFilesAsync();
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

        _allFiles = Vaults.SelectMany(vault => vault.Files).ToList();
    }

    public List<File> GetAllFiles()
    {
        return _allFiles ??= Vaults.SelectMany(vault => vault.Files).ToList();
    }

    public List<File> GetAllFilesWithTag(string tag)
    {
        return GetAllFiles().Where(file => file.HasTag(tag)).ToList();
    }

    public Vault? GetVaultWithId(string vaultId) => Vaults.Find(vault => vault.Id == vaultId);

    public Vault? GetVaultWithName(string name)
    {
        Vault? vault = Vaults.Find(vault => vault.Name == name);
        return vault;
    }

    public void AddTagsToList(string[] tags)
    {
        foreach (string tag in tags)
        {
            _tagsList.TryAdd(tag, 0);
        }
    }

    public bool IsAnExistingTag(string tagToCheck) => TagsList.Any(tag => tag.IsSameString(tagToCheck));

    public string? GetExistingTag(string tagToGet)
    {
        return TagsList.FirstOrDefault(tag => tag.IsSameString(tagToGet));
    }
}
