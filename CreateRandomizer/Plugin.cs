using BepInEx;
using BepInEx.Logging;
using CheatMenu.Classes;
using Constance;
using CreateRandomizer.Classes;
using CreateRandomizer.Classes.Data;
using CreateRandomizer.Classes.Pages.Data;
using CreateRandomizer.Classes.Pages.Locations;
using CreateRandomizer.Classes.Pages.Regions;
using CreateRandomizer.Classes.Pages.Transitions;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CreateRandomizer;

[BepInDependency("CheatMenu", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("FileHandler", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("RandomizerCore", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private ModGUI modGUI;
    public static Transform Transform { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        Transform = transform;

        DataHandler.Init();
        SceneHandler.Init();
        InitializeModGUI();

        Harmony patcher = new("HarmonyPatcher");
        patcher.PatchAll();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void InitializeModGUI()
    {
        modGUI = ModGUI.Create(KeyCode.Home);
        modGUI.AddPage<TransitionPage>();
        modGUI.AddPage<LocationPage>();
        modGUI.AddPage<RegionPage>();
        modGUI.AddPage<DataPage>();
    }

    private void Update()
    {
        if (Keyboard.current.homeKey.wasPressedThisFrame)
        {
            modGUI.ShowGUI = !modGUI.ShowGUI;
        }
    }
}
