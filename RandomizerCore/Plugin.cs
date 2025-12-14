using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Constance;
using HarmonyLib;
using Mono.Cecil;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

namespace RandomizerCore;

[BepInDependency("FileHandler", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    private void Awake()
    {
        Logger = base.Logger;

        RegionHandler.Init();
        CollectableHandler.Init();
        ProgressiveItemHandler.Init();

        Harmony patcher = new("HarmonyPatcher");
        patcher.PatchAll();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
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

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            IConPlayerEntity player = ConMonoBehaviour.SceneRegistry.PlayerOne;
            player.HealFully();
            player.RefillPaint();
        }
    }
}
