using System;
using System.Collections.Generic;
using System.Text;
using Constance;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Leo;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Locations;
using UnityEngine;

namespace Randomizer.Patches.Locations.FoundryPipe;

[HarmonyPatch(typeof(ConFoundryPaintPipe_Valve))]
public class ConFoundryPaintPipe_Valve_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ConFoundryPaintPipe_Valve.HandleIncomingAttack))]
    private static bool HandleIncomingAttack_Prefix(ConFoundryPaintPipe_Valve __instance, ref ConAttackResult __result, ConAttackRequest request)
    {
        __result = default;
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.FoundryPipe)) return true;

        if (__instance.Unlocked)
        {
            __result = ConAttackResult.Ignored;
            return false;
        }
        if (!ConAttackUtils.HasPaint(request, 2))
        {
            __result = ConAttackResult.Ignored;
            return false;
        }
        __instance._fillAnimStartPoint = __instance._fill;
        __instance._fillAnimTimer.Start(__instance.fillAnimCurve, null);
        __instance._fill = Mathf.Clamp01(__instance._fill + __instance.stabFillAmount);
        if (__instance._fill >= 0.85f)
        {
            ALocation location = __instance.GetComponent<LocationComponent>().Location;
            RandomState.TryGetItem(location);
        }
        __instance.onPipeHit.InvokeSafe(null);
        __result = ConAttackResult.Hit;
        return false;
    }
}
