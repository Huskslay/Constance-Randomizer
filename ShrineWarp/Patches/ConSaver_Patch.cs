//using Constance;
//using HarmonyLib;
//using ShrineWarp.Classes;

//namespace ShrineWarp.Patches;

//[HarmonyPatch(typeof(ConSaver))]
//public class ConSaver_Patch
//{
//    [HarmonyPrefix]
//    [HarmonyPatch(nameof(ConSaver.Delete))]
//    private static void Delete_Prefix(ConSaveStateId slotId)
//    {
//        ShrineDataHandler.DeleteShrineData(slotId);
//    }
//}