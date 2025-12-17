using Constance;
using HarmonyLib;
using Leo;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;
using System.Collections;

namespace Randomizer.Patches.Locations.Cousin;

[HarmonyPatch(typeof(CConBehaviour_LostShopKeeper))]
public class CConBehaviour_LostShopKeeper_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConBehaviour_LostShopKeeper.Start))]
    private static bool Start_Prefix(CConBehaviour_LostShopKeeper __instance)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Cousins)) return true;

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConBehaviour_LostShopKeeper.OnShopKeeperReturn))]
    private static bool OnShopKeeperReturn_Prefix(CConBehaviour_LostShopKeeper __instance, ref IEnumerator __result)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Cousins)) return true;

        ALocation location = __instance.GetComponent<LocationComponent>().Location;
        RandomState.TryGetItem(location);

        //__instance.gameObject.SetActive(false);
        __instance.returnFeedback.TryPlay(false);
        __result = CoroutineUtils.WaitUntil(() => !__instance.returnFeedback.IsPlaying);

        return false;
    }
}
