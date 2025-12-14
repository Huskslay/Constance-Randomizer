using System;
using System.Collections.Generic;
using System.Text;
using AsmResolver.PE.DotNet.ReadyToRun;
using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers.State;
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

        ALocation location = __instance.GetComponent<LocationComponent>().Location;
        RandomState.TryGetItem(location);

        __instance.FinishCollection();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConInspirationTriggerBehaviour.Start))]
    private static bool Start_Prefix()
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Inspirations)) return true;

        return false;
    }
}
