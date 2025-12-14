using Constance;
using HarmonyLib;
using Randomizer.Classes.UI.Elements;

namespace Randomizer.Patches.Menu;

[HarmonyPatch(typeof(CConUiSaveSlot))]
public class CConUiSaveSlot_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConUiSaveSlot.OnSubmit))]
    private static bool OnSubmit_Prefix(CConUiSaveSlot __instance)
    {
        RandomLoader.chosenSlotId = __instance.SlotId;

        if (__instance._validSave)
        {
            RandomLoader.LoadSave();
            return false;
        }

        CConStartMenu_Patch.SwitchMenu(RandomLoader.RandoSelectMenu, CConStartMenu_Patch.SaveMenu);
        return false;
    }
}
