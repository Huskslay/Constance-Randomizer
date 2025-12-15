using BepInEx;
using BepInEx.Logging;
using CheatMenu.Classes;
using Constance;
using CreateRandomizer.Classes;
using CreateRandomizer.Classes.Pages.Data;
using CreateRandomizer.Classes.Pages.Locations;
using CreateRandomizer.Classes.Pages.Regions;
using CreateRandomizer.Classes.Pages.Transitions;
using HarmonyLib;
using RandomizerCore.Classes.Handlers;
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

        // Fly
        if (Keyboard.current.f9Key.wasPressedThisFrame)
            ConDebugFlags.DebugFly = !ConDebugFlags.DebugFly;

        // Invulnerability
        if (Keyboard.current.f8Key.wasPressedThisFrame)
            ConDebugFlags.DebugInvulnerability = !ConDebugFlags.DebugInvulnerability;

        // Give all abilities and inspirations
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            IConPlayerInventory inventoryManager = ConMonoBehaviour.SceneRegistry.Inventory;
            IConPlayerEntity player = ConMonoBehaviour.SceneRegistry.PlayerOne;

            string[] abilities = [
                "slice", "wallDive", "dash", "doubleJump", "pogo", "stab", "bombClone", "fridaMask"
            ];

            foreach (string ability in abilities)
                inventoryManager.Collect(player, CollectableHandler.dict[CollectableHandler.nameDict[ability]], 1);
            foreach (SConCollectable_InspirationDrawing inspiration in CollectableHandler.inspirationCollectables)
                inventoryManager.Collect(player, inspiration, 1);
        }

        // Heal
        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            IConPlayerEntity player = ConMonoBehaviour.SceneRegistry.PlayerOne;
            player.HealFully();
            player.RefillPaint();
        }
    }
}
