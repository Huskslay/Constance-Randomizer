using System;
using System.Collections.Generic;
using System.Text;
using CheatMenu.Classes;
using Constance;
using HarmonyLib;
using ShrineWarp.Classes;

namespace ShrineWarp.Patches;

[HarmonyPatch(typeof(AConUiPanel<IConUiPanel_Journal.PageType>))]
internal class CConUiPanel_Journal_Patch
{
    public static CConUiPanel_Journal journal;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AConUiPanel<IConUiPanel_Journal.PageType>.OpenPanel))]
    private static void OpenPage_Postfix(AConUiPanel<IConUiPanel_Journal.PageType> __instance)
    {
        if (__instance is not CConUiPanel_Journal) return;
        journal = (CConUiPanel_Journal)__instance;
        ShrineDataHandler.LoadShrineData();
        Plugin.modGUI.ShowGUI = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AConUiPanel<IConUiPanel_Journal.PageType>.ClosePanel))]
    private static void ClosePanel_Postfix(AConUiPanel<IConUiPanel_Journal.PageType> __instance)
    {
        if (__instance is not CConUiPanel_Journal) return;
        Plugin.modGUI.ShowGUI = false;
    }
}
