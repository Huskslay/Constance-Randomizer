using System;
using System.Collections.Generic;
using System.Text;
using CheatMenu.Classes;
using Constance;
using HarmonyLib;
using ShrineWarp.Classes;
using ShrineWarp.Classes.Pages;

namespace ShrineWarp.Patches;

[HarmonyPatch(typeof(CConMeditationPointEntity))]
internal class CConMeditationPointEntity_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConMeditationPointEntity.OnPlayerMeditationStart))]
    private static void OnPlayerMeditationStart_Prefix()
    {
        if (OptionsPage.unlockAll) return;
        string region = ConMonoBehaviour.SceneRegistry.PlayerOne.Level.Current.StringValue;
        ShrineDataHandler.UpdateShrineData(region, true);
    }
}
