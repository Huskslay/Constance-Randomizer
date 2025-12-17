using Constance;
using HarmonyLib;
using Leo;
using UnityEngine;

namespace RandomizerMap.Patches;

[HarmonyPatch(typeof(CConUiJournalPage_MapMicro))]
public class CConUiJournalPage_MapMicro_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConUiJournalPage_MapMicro.ExecuteCursorRaycast))]
    private static bool OnMoveInput_Prefix(CConUiJournalPage_MapMicro __instance, ref IConUiMapSelectTarget __result)
    {
        Vector2 vector = __instance.mapDefaultCursor.position;
        float num = 1f * __instance.mapPage.CanvasCam.Zoom;
        __result = null;
        foreach (IConUiMapSelectTarget conUiMapSelectTarget in __instance.mapRoot.SelectTargets)
        {
            float dist = Vector2.Distance(conUiMapSelectTarget.RectTransform.position, vector);
            if (conUiMapSelectTarget.Exists() && dist < num)
            {
                __result = conUiMapSelectTarget;
                num = dist;
            }
        }
        return false;
    }
}

