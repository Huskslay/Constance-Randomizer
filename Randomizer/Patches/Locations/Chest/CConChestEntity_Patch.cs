using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;

namespace Randomizer.Patches.Locations.Chest;

[HarmonyPatch(typeof(CConChestEntity))]
public class CConChestEntity_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConChestEntity.SpawnLoot))]
    private static bool CConChestEntity_Prefix(CConChestEntity __instance)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Chests)) return true;

        ALocation location = __instance.GetComponent<LocationComponent>().Location;
        RandomState.TryGetItem(location);

        return false;
    }
}
