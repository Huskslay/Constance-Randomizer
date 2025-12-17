using BepInEx;
using BepInEx.Logging;
using CheatMenu.Classes;
using HarmonyLib;
using ShrineWarp.Classes;
using ShrineWarp.Classes.Pages;
using System.Collections.Generic;
using UnityEngine;

namespace ShrineWarp;

[BepInDependency("CheatMenu", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("FileHandler", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public static ModGUI modGUI;
    public static Transform Transform { get; private set; }

    public static List<string> regions = [
      "Prod_V01:cp_Prod_V01_a15fffec-931b-4c37-8dac-6f4c1e742549",
      "Prod_A03:cp_Prod_A03_ec8cf423-84b2-4ed9-919d-b12e48411235",
      "Prod_A07:cp_Prod_A07_86f03c0e-c470-4e19-9ac8-1f7cb972d2b5",
      "Prod_A30:cp_Prod_A30_395e361c-ed6c-4da7-816d-4ca96679fb8a",
      "Prod_C01:cp_Prod_C01_Shrine",
      "Prod_C03:cp_Prod_C03_e677b432-7549-39ac-40ce-62eca475adfa",
      "Prod_C04:cp_Prod_C04_51312ba7-fbf6-ad4d-5fc1-bc622dcd422e",
      "Prod_C05:cp_Prod_C05_9121dfac-f84e-abb5-1e94-1bdc8b654da7",
      "Prod_F05:cp_Prod_F05_8a10f036-a376-49db-ae02-f380eaa60261",
      "Prod_F27:cp_Prod_F27_6b56f6d6-1824-48ed-ab70-498c33b49c6a",
      "Prod_J04:cp_Prod_J04_Shrine",
      "Prod_J10:cp_Prod_J10_bc43b473-b7e2-4d10-9f82-0bde9891d96d",
      "Prod_V14:cp_Prod_V14_7a5f0136-457a-4587-b6e1-db72e66b2758",
      "Prod_V17:cp_Prod_V17_685c34a5-a4e6-417a-8ac2-aeef682d26d1",
      "Prod_V23:cp_Prod_V23_48f5c62d-b3ff-4321-9c19-583c43185186"
    ];


    private void Awake()
    {
        Logger = base.Logger;
        Transform = transform;

        InitializeModGUI();
        ShrineDataHandler.Init();

        Harmony patcher = new("HarmonyPatcher");
        patcher.PatchAll();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void InitializeModGUI()
    {
        modGUI = ModGUI.Create(KeyCode.Home, windowRect: new(Screen.currentResolution.width - 300f, 140f, 200f, 600f), windowName: "Shrine Warp");
        modGUI.AddPage<SelectionPage>();
        modGUI.AddPage<OptionsPage>();
    }
}
