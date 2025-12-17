using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;

namespace Randomizer.Patches.Locations.Inspiration;

[HarmonyPatch(typeof(CConInspirationTriggerBehaviour))]
public class CConInspirationTriggerBehaviour_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConInspirationTriggerBehaviour.EnterSeq))]
    private static void EnterSeq_Prefix(CConInspirationTriggerBehaviour __instance)
    {
        if (!RandomState.Randomized) return;
        if (!RandomState.IsRandomized(RandomizableItems.Inspirations)) return;

        LocationComponent comp = __instance.GetComponent<LocationComponent>();
        if (comp == null)
        {
            Plugin.Logger.LogWarning($"Inspiration '{__instance.name}' is not randomized");
            return;
        }
        ALocation location = comp.Location;
        RandomState.TryGetItem(location);

        __instance.FinishCollection();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConInspirationTriggerBehaviour.Start))]
    private static bool Start_Prefix(CConInspirationTriggerBehaviour __instance)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Inspirations)) return true;

        LocationComponent comp = __instance.GetComponent<LocationComponent>();
        return comp == null;
    }
}
