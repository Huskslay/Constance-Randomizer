using BepInEx;
using BepInEx.Logging;
using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Handlers;
using RandomizerItemDisplay.Class;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RandomizerItemDisplay;

[BepInDependency("RandomizerCore", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{ 
    internal static new ManualLogSource Logger;
    private static Plugin instance;

    public static Transform Transform => instance.transform;

    private void Awake()
    {
        Logger = base.Logger;
        instance = this;

        Tracker.Init();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        Tracker.Update();
    }
}
