using Constance;
using FileHandler.Classes;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Items.Types.Progressive;

[Serializable]
public class ProgressiveItemInstance(string name, List<string> collectables)
{
    public readonly string name = name;

    private readonly List<string> locations = [];
    private readonly List<string> collectables = collectables;


    public ProgressiveItem AddItem(ALocation location)
    {
        if (AllLoaded(out _))
        {
            Plugin.Logger.LogError($"Tried to add to progressive item instance '{name}' but max locations already added");
            return null;
        }
        locations.Add(location.GetFullName());
        FileSaveLoader.TrySaveClassToFile(this, ProgressiveItemHandler.folderName, name);
        return new ProgressiveItem(location, name);
    }

    public bool AllLoaded(out Tuple<int, int> counts)
    {
        counts = new(locations.Count, collectables.Count);
        return locations.Count == collectables.Count;
    }

    private int GetIndex()
    {
        int collected = 0;
        Plugin.Logger.LogError(locations.Count);
        foreach (string location in locations)
        {
            if (!RandomState.TryGetElementFromDestName(location, out RandomStateElement element))
            {
                Plugin.Logger.LogError($"Could not find element for location: {location}");
                continue;
            }
            Plugin.Logger.LogWarning($"ADWD {element.source.GetFullName()}");
            if (element.hasObtainedSource) collected++;
        }
        Plugin.Logger.LogError("A");
        return collected;
    }

    public void Claim(IConPlayerEntity player, IConPlayerInventory inventoryManager)
    {
        if (!AllLoaded(out _))
        {
            Plugin.Logger.LogError($"Collectable instance does not have all expected registered");
            return;
        }

        int collected = GetIndex();

        Plugin.Logger.LogMessage($"Giving 1 {collectables[collected]}");
        inventoryManager.Collect(player, CollectableHandler.dict[collectables[collected]], 1);
    }
    
    public string GetItemName()
    {
        int collected = GetIndex();
        return $"{name} - {collected}/{collectables.Count}";
    }
}
