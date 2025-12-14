using System;
using System.Collections.Generic;
using System.Text;
using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Requirements.Entries;

namespace Randomizer.Patches.Events.Bosses;

[HarmonyPatch(typeof(ConState_AweKing_Death))]
public class ConState_AweKing_Death_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ConState_AweKing_Death.OnEnter))]
    private static void OnEnter_Prefix()
    {
        if (!RandomState.Randomized) return;

        Plugin.Logger.LogMessage("Awe King defeated");
        RandomState.AchieveEvents(EventsEntries.AweKing);
        return;
    }
}
