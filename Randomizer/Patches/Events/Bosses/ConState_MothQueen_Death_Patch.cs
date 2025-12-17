using Constance;
using HarmonyLib;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Requirements.Entries;

namespace Randomizer.Patches.Events.Bosses;

[HarmonyPatch(typeof(ConState_MothQueen_Death))]
public class ConState_MothQueen_Death_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ConState_MothQueen_Death.OnEnter))]
    private static void OnEnter_Prefix()
    {
        if (!RandomState.Randomized) return;

        Plugin.Logger.LogMessage("High Patia (Moth Queen) defeated");
        RandomState.AchieveEvents(EventsEntries.HighPatia);
        return;
    }
}
