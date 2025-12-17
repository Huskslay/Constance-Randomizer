using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;

namespace Randomizer.Patches.Locations.Canvas;

[HarmonyPatch(typeof(ConState_Player_AbilityUnlock))]
public class ConState_Player_AbilityUnlock_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ConState_Player_AbilityUnlock.Enter))]
    private static void Collectable_Postfix(CConUnlockAbilityCanvas unlockCanvas)
    {
        if (!RandomState.Randomized) return;
        if (!RandomState.IsRandomized(RandomizableItems.Canvases)) return;

        ALocation location = unlockCanvas.GetComponent<LocationComponent>().Location;
        RandomState.TryGetItem(location);
    }
}
