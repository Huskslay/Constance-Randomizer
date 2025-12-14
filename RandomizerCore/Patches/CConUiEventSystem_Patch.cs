using System;
using System.Collections.Generic;
using System.Text;
using Constance;
using HarmonyLib;

namespace RandomizerCore.Patches;

[HarmonyPatch(typeof(CConUiEventSystem))]
public class CConUiEventSystem_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConUiEventSystem.SetInputEnabled))]   
    private static bool SetInputEnabled_Prefix(CConUiEventSystem __instance)
    {
        return __instance.currentInputModule != null;
    }
}