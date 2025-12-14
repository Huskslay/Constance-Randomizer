using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using Constance;
using HarmonyLib;
using Leo;
using RandomizerCore.Classes.Handlers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RandomizerCore.Patches.AllowDashlessWallDive;

[HarmonyPatch(typeof(CConPlayerInventoryManager))]
internal class CConPlayerInventoryManager_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConPlayerInventoryManager.Has))]
    private static bool Has_Prefix(ref bool __result, SConCollectable collectable)
    {
        SConCollectable dash = CollectableHandler.NameToCollectable("dash");
        if (collectable != null && collectable == dash && AConState_Player_LeoVoid_Patch.FakeDash)
        {
            __result = true;
            return false;
        }
        return true;
    }
}