using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Randomizer.Classes.Random;
using Randomizer.Classes.Random.Generation;
using RandomizerCore.Classes.State;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Randomizer;

[BepInDependency("FileHandler", BepInDependency.DependencyFlags.HardDependency)]
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

        Harmony patcher = new("HarmonyPatcher");
        patcher.PatchAll();

        RandomFiles.Init();
        RandomState.Init(new RandomGenerator());

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (Keyboard.current.f6Key.wasPressedThisFrame) RandomFiles.SaveSpoilerLog();
    }

    public static void StartCoroutine(Func<IEnumerator> a)
    {
        instance.StartCoroutine(a());
    }
}
