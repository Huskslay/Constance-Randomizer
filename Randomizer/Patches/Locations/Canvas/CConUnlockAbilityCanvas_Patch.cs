using System;
using System.Collections.Generic;
using System.Text;
using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Handlers.State;

namespace Randomizer.Patches.Locations.Canvas;

[HarmonyPatch(typeof(CConUnlockAbilityCanvas))]
public class CConUnlockAbilityCanvas_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConUnlockAbilityCanvas.Collectable), MethodType.Getter)]
    private static bool Collectable_Prefix(ref SConCollectable __result)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Canvases)) return true;

        __result = null;
        return false;
    }
}
