using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Skips;

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
