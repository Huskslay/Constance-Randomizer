using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Constance;
using HarmonyLib;

namespace RandomizerMap.Patches;

[HarmonyPatch(typeof(CConUiMapIcon))]
public class CConUiMapIcon_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch("Constance.IConUiMapSelectTarget.Selectable", MethodType.Getter)]
    private static bool Selectable_Prefix(CConUiMapIcon __instance, ref bool __result)
    {
        if (__instance.name != "Custom") return true;
        __result = true;
        return false;
    }
}
