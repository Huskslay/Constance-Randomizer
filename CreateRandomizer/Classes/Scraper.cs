using Constance;
using FileHandler.Classes;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Items.Types.Progressive;
using RandomizerCore.Classes.Storage.Locations.Types;
using RandomizerCore.Classes.Storage.Locations.Types.Deposits;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateRandomizer.Classes;

public class Scraper : MonoBehaviour
{
    private static Scraper I = null;

    public static bool Running { get; private set; } = false;


    public static Dictionary<string, List<Tuple<ConCheckPointId, ConCheckPointId>>> transitionInfos = [];
    private static List<string> hasShrines;


    private static ProgressiveItemInstance progressiveItem;


    public static void Scrape()
    {
        if (I == null)
        {
            I = new GameObject().AddComponent<Scraper>();
            I.transform.SetParent(Plugin.Transform);
        }

        CollectableHandler.TrueInit();
        progressiveItem = new("Tears", [
            CollectableHandler.goalsList[0].name,
            CollectableHandler.goalsList[1].name,
            CollectableHandler.goalsList[2].name,
            CollectableHandler.goalsList[3].name,
        ]);
        ProgressiveItemHandler.AddInstance(progressiveItem);

        if (Running)
        {
            Plugin.Logger.LogWarning("Already loading levels");
            return;
        }
        I.StartCoroutine(ILoadAllLevels());
    }

    private static IEnumerator ILoadAllLevels()
    {
        Running = true;
        hasShrines = ["Prod_V01:cp_Prod_V01_a15fffec-931b-4c37-8dac-6f4c1e742549"];

        FindFirstObjectByType<CConTimelinePlayerController>().enabled = false;
        CConPlayerEntity player = Plugin.FindFirstObjectByType<CConPlayerEntity>();


        Plugin.Logger.LogMessage($"~~~~~~~~~~~~~\nScene Editing");

        foreach (string scene in SceneHandler.scenes)
        {
            yield return ScrapeLevel(player, scene);
            if (!Running) break;
        }

        FileSaveLoader.TrySaveClassToJson(hasShrines, "Names", "Shrine Maps");


        RegionHandler.Init();


        Plugin.Logger.LogMessage($"~~~~~~~~~~~~~\nFlashback Editing");

        foreach (string scene in SceneHandler.flashbackScenes)
        {
            yield return ScrapeFlashback(player, scene);
            if (!Running) break;
        }
        if (!progressiveItem.AllLoaded(out _)) Plugin.Logger.LogError($"Did not load all progressive items for instance {progressiveItem.name}");

        RegionHandler.Init();


        Plugin.Logger.LogMessage($"~~~~~~~~~~~~~\nPost Editing");
        foreach (Region region in RegionHandler.Regions)
        {
            PostScraper.Run(region, transitionInfos[region.GetFullName()]);
            RegionHandler.SaveRegion(region, log: false);
        }


        RegionHandler.Init();


        Running = false;
    }

    private static IEnumerator ScrapeLevel(CConPlayerEntity player, string scene)
    {
        Plugin.Logger.LogMessage($"~~~~~~~~~~~~~\nLoading {scene}");

        ConLevelId levelId = new(scene);
        yield return RegionHandler.LoadLevel(levelId, player);


        List<CConTeleportPoint> transitions =
            [.. FindObjectsByType<CConTeleportPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None)];
        Plugin.Logger.LogMessage($"Found: {transitions.Count} Transitions");
        CConElevatorBehaviour elevator = FindFirstObjectByType<CConElevatorBehaviour>(FindObjectsInactive.Exclude);

        List<CConCurrencyDepositEntity> deposits =
            [.. FindObjectsByType<CConCurrencyDepositEntity>(DepositLocationFactory.FindInactive, FindObjectsSortMode.None)];
        List<CConChestEntity> chests =
            [.. FindObjectsByType<CConChestEntity>(ChestLocation.FindInactive, FindObjectsSortMode.None)];
        List<CConUnlockAbilityCanvas> canvases =
            [.. FindObjectsByType<CConUnlockAbilityCanvas>(CanvasLocation.FindInactive, FindObjectsSortMode.None)];
        List<CConInspirationTriggerBehaviour> inspirations =
            [.. FindObjectsByType<CConInspirationTriggerBehaviour>(InspirationLocation.FindInactive, FindObjectsSortMode.None)];
        List<SConCollectable_ShopItem> shopItems = scene == "Prod_J04" ?
            [.. ConMonoBehaviour.SceneRegistry.Collectables.ShopItems] : [];
        List<CConEntityDropBehaviour_TouchToCollect> dropBehaviours =
            [.. FindObjectsByType<CConEntityDropBehaviour_TouchToCollect>(DropBehaviourLocation.FindInactive, FindObjectsSortMode.None)];
        List<ConFoundryPaintPipe_Valve> foundryPipes =
            [.. FindObjectsByType<ConFoundryPaintPipe_Valve>(FoundryPipeLocation.FindInactive, FindObjectsSortMode.None)];
        CConBehaviour_LostShopKeeper cousin = FindFirstObjectByType<CConBehaviour_LostShopKeeper>(CousinLocation.FindInactive);

        string isACousin = cousin == null ? "No" : "1";
        Plugin.Logger.LogMessage($"Found: {deposits.Count} Deposits, {chests.Count} Chests, {canvases.Count} Canvases, {inspirations.Count} Inspirations, {shopItems.Count} Shop Items, {dropBehaviours.Count} Drop Behaviours, {foundryPipes.Count} Foundry Pipes, {isACousin} Cousin");

        Region region = new(levelId, transitions, elevator, deposits, chests, canvases, inspirations, shopItems, dropBehaviours, foundryPipes, cousin);
        RegionHandler.SaveRegion(region, log: false);


        CConLevel_Adventure level = FindFirstObjectByType<CConLevel_Adventure>();
        List<Tuple<ConCheckPointId, ConCheckPointId>> infos = [];
        foreach (SConLevelInfo.TransitionInfo info in level.LevelInfo.transitionsTo)
            infos.Add(new(info.id.checkPoint1, info.id.checkPoint2));
        transitionInfos[region.GetFullName()] = infos;


        CConMeditationPointEntity shrine = FindFirstObjectByType<CConMeditationPointEntity>();
        if (shrine != null) hasShrines.Add(levelId.StringValue + ":" + shrine.checkPoint.checkPointId.StringValue);
    }

    private static IEnumerator ScrapeFlashback(CConPlayerEntity player, string scene)
    {
        Plugin.Logger.LogMessage($"~~~~~~~~~~~~~\nLoading {scene}");

        ConLevelId levelId = new(scene);
        yield return RegionHandler.LoadLevel(levelId, player);

        CConLevel_Flashback level = FindFirstObjectByType<CConLevel_Flashback>();
        if (!level.exitToCheckPoint.TryExtractLevelId(out ConLevelId exit))
        {
            Plugin.Logger.LogError("Could not get exit id");
            yield break;
        }
        if (!RegionHandler.TryGetRegion(exit.StringValue, out Region exitRegion))
        {
            Plugin.Logger.LogError("Could not get exit region");
            yield break;
        }

        exitRegion.SetTearLocation(progressiveItem, level.tearUnlock);
        RegionHandler.SaveRegion(exitRegion, log: false);
    }
}