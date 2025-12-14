using System;
using System.Collections.Generic;
using System.Text;
using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Locations;

namespace Randomizer.Patches.Locations.FoundryPipe;

[HarmonyPatch(typeof(AConFoundryPaintPipe))]
public class AConFoundryPaintPipe_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(AConFoundryPaintPipe.UpdatePropertyBlock))]
    private static void UpdatePropertyBlock_Prefix(AConFoundryPaintPipe __instance, ref float fill)
    {
        if (!RandomState.Randomized) return;
        if (!RandomState.IsRandomized(RandomizableItems.FoundryPipe)) return;

        if (__instance is not ConFoundryPaintPipe_Valve) return;

        LocationComponent locationComponent = __instance.GetComponent<LocationComponent>();
        if (locationComponent == null) return;

        ALocation location = locationComponent.Location;

        if (!RandomState.TryGetElement(location, out RandomStateElement element))
        {
            Plugin.Logger.LogWarning($"Problem getting element for location '{location.GetFullName()}'");
            return;
        }
        fill = element.hasObtainedSource ? 1f : (fill < 0.85f ? fill : 0f);
    }
}
