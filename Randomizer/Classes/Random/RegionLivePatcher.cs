using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Locations.Types;
using RandomizerCore.Classes.Storage.Locations.Types.Deposits;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Randomizer.Classes.Random;

public static class RegionLivePatcher
{
    public static void LoadedRegion()
    {
        if (!RandomState.Randomized) return;

        IConPlayerEntity player = ConMonoBehaviour.SceneRegistry.PlayerOne;
        ConLevelId levelId = player.Level.Current;

        if (levelId.IsEmpty()) return;

        Region region;

        if (levelId.StringValue.Contains("Flashback"))
        {
            CConLevel_Flashback flashback = Plugin.FindFirstObjectByType<CConLevel_Flashback>();
            if (flashback == null || !flashback.exitToCheckPoint.TryExtractLevelId(out ConLevelId exitRegion))
            {
                Plugin.Logger.LogMessage($"Could not find region from flashback at {levelId.StringValue}");
                return;
            }
            if (!RegionHandler.TryGetRegion(exitRegion.StringValue, out region))
            {
                Plugin.Logger.LogMessage($"Could not find region from flashback exit at {exitRegion.StringValue}");
                return;
            }
            Plugin.Logger.LogMessage($"Patching flashback {levelId.StringValue}");
            TearLocation.PatchLoadedLevel(flashback, region.tearLocation);
            return;
        }
        
        if (!RegionHandler.TryGetRegion(levelId.StringValue, out region))
        { 
            Plugin.Logger.LogMessage($"Could not find region for id {levelId.StringValue}");
            return;
        }
        Plugin.Logger.LogMessage($"Patching {levelId.StringValue}");

        ////////////////////////////////////////////////////////


        List<CConCurrencyDepositEntity> deposits =
            [.. UnityEngine.Object.FindObjectsByType<CConCurrencyDepositEntity>(DepositLocationFactory.FindInactive, FindObjectsSortMode.None)];
        DepositLocationFactory.PatchLoadedLevel(deposits, region.lightStoneLocations, region.currencyFlowerLocations);  

        List<CConChestEntity> chests =
            [.. UnityEngine.Object.FindObjectsByType<CConChestEntity>(ChestLocation.FindInactive, FindObjectsSortMode.None)];
        ChestLocation.PatchLoadedLevel(chests, region.chestLocations);

        List<CConUnlockAbilityCanvas> canvases =
            [.. UnityEngine.Object.FindObjectsByType<CConUnlockAbilityCanvas>(CanvasLocation.FindInactive, FindObjectsSortMode.None)];
        CanvasLocation.PatchLoadedLevel(canvases, region.canvasLocations);

        List<CConInspirationTriggerBehaviour> inspirations =
            [.. UnityEngine.Object.FindObjectsByType<CConInspirationTriggerBehaviour>(InspirationLocation.FindInactive, FindObjectsSortMode.None)];
        InspirationLocation.PatchLoadedLevel(inspirations, region.inspirationLocations);

        CConUiPanel_Shop shop = UnityEngine.Object.FindFirstObjectByType<CConUiPanel_Shop>(FindObjectsInactive.Include);
        ShopItemLocation.PatchLoadedLevel(shop, player.Level.Current, region.shopItemLocations);

        List<CConEntityDropBehaviour_TouchToCollect> dropBehaviours = [.. UnityEngine.Object.FindObjectsByType<CConEntityDropBehaviour_TouchToCollect>(DropBehaviourLocation.FindInactive, FindObjectsSortMode.None)];
        DropBehaviourLocation.PatchLoadedLevel(dropBehaviours, region.dropBehaviourLocations);

        List<ConFoundryPaintPipe_Valve> valves = [.. UnityEngine.Object.FindObjectsByType<ConFoundryPaintPipe_Valve>(FoundryPipeLocation.FindInactive, FindObjectsSortMode.None)];
        FoundryPipeLocation.PatchLoadedLevel(valves, region.foundryPipeLocations);

        CConBehaviour_LostShopKeeper cousin = Plugin.FindFirstObjectByType<CConBehaviour_LostShopKeeper>(CousinLocation.FindInactive);
        CousinLocation.PatchLoadedLevel(cousin, region.cousinLocation);
    }
}