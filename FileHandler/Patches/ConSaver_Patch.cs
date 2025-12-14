using Constance;
using FileHandler.Classes;
using HarmonyLib;

namespace FileHandler.Patches;

[HarmonyPatch(typeof(ConSaver))]
public class ConSaver_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ConSaver.Delete))]
    private static void Delete_Prefix(ConSaver __instance, ConSaveStateId slotId)
    {
        GameDataActions.OnFileDelete.Invoke(__instance, slotId);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ConSaver.Save))]
    private static void Save_Prefix(ConSaver __instance, ConSaveStateId saveId)
    {
        GameDataActions.OnFileSave.Invoke(__instance, saveId);
    }
}