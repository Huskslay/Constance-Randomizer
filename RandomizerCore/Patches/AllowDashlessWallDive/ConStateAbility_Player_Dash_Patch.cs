using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using Constance;
using HarmonyLib;
using Leo;
using UnityEngine;

namespace RandomizerCore.Patches.AllowDashlessWallDive;

[HarmonyPatch(typeof(ConStateAbility_Player_Dash))]
internal class ConStateAbility_Player_Dash_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ConStateAbility_Player_Dash.FindRelevantMagnetArea))]
    private static bool FindRelevantMagnetArea_Prefix(ref bool __result)
    {
        if (!AConState_Player_LeoVoid_Patch.FakeDash) return true;
        __result = false;
        return false;
    }
}