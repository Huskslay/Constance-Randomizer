using Constance;
using HarmonyLib;
using Randomizer.Classes.UI.Elements;
using RandomizerCore.Classes.State;

namespace Randomizer.Patches;

[HarmonyPatch(typeof(CConPlayerManager_Game))]
public class CConPlayerManager_Game_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CConPlayerManager_Game.Start))]
    private static void Start_Postfix()
    {
        if (RandomLoader.randomizing) RandomLoader.CreateRandomizer();
        else RandomState.TryLoadRandomizerState();
    }
}
