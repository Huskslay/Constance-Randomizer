using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;

namespace Randomizer.Patches.Locations.DropBehaviour;

[HarmonyPatch(typeof(AConEntityDropBehaviour))]
public class AConEntityDropBehaviour_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(AConEntityDropBehaviour.Collect))]
    private static void Collect_Prefix(AConEntityDropBehaviour __instance)
    {
        if (!RandomState.Randomized) return;
        if (!RandomState.IsRandomized(RandomizableItems.DropBehaviours)) return;

        if (__instance is CConEntityDropBehaviour_TouchToCollect)
        {
            ALocation location = __instance.GetComponent<LocationComponent>().Location;
            RandomState.TryGetItem(location);
        }
    }
}
