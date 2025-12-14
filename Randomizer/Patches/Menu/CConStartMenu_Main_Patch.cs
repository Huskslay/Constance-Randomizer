using Constance;
using HarmonyLib;
using Randomizer.Classes.UI.Elements;

namespace Randomizer.Patches.Menu;

[HarmonyPatch(typeof(AConStartMenuPanel))]
public class CConStartMenu_Main_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(AConStartMenuPanel.OpenPanel))]
    private static void OpenPanel_Postfix(AConStartMenuPanel __instance)
    {
        if (__instance is not CConStartMenu_Main) return;
        RandomLoader.QuitSave();
    }
}
