using Constance;
using HarmonyLib;
using RandomizerCore.Classes.State;

namespace Randomizer.Patches.Locations.Cousin;

[HarmonyPatch(typeof(SConConfig_QuestFindShopKeepers))]
public class SConConfig_QuestFindShopKeepers_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SConConfig_QuestFindShopKeepers.IsFound), [typeof(ConLevelId)])]
    private static bool Start_Prefix(ref bool __result, ConLevelId levelId)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Cousins)) return true;

        __result = RandomState.Instance.Cousins.Contains(levelId);

        return true;
    }
}
