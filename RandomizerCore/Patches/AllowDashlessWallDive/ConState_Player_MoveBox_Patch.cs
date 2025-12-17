using Constance;
using HarmonyLib;

namespace RandomizerCore.Patches.AllowDashlessWallDive;

[HarmonyPatch(typeof(ConState_Player_MoveBox))]
internal class ConState_Player_MoveBox_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ConState_Player_MoveBox.TryInit))]
    private static bool TryInit_Prefix(ref bool __result)
    {
        if (!AConState_Player_LeoVoid_Patch.FakeDash) return true;
        __result = false;
        return false;
    }
}