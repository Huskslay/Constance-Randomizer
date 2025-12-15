using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Constance;
using HarmonyLib;
using Mono.Cecil;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Skips;
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

        SkipDataHandler.Init();
        RegionHandler.Init();
        CollectableHandler.Init();
        ProgressiveItemHandler.Init();

        Harmony patcher = new("HarmonyPatcher");
        patcher.PatchAll();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
