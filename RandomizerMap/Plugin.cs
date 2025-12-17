using BepInEx;
using BepInEx.Logging;
using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.State;

namespace RandomizerMap;

[BepInDependency("RandomizerCore", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("Randomizer", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        Harmony patcher = new("HarmonyPatcher");
        patcher.PatchAll();

        RandomState.onLoadRandoSave.AddListener(GiveFullMap);

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void GiveFullMap()
    {
        Plugin.Logger.LogMessage("Giving full map");
        ConDebugFlags.DebugRevealMap = true;

        ConMonoBehaviour.SceneRegistry.Inventory.Collect(ConMonoBehaviour.SceneRegistry.PlayerOne, CollectableHandler.NameToCollectable("map"), 1);
    }
}
