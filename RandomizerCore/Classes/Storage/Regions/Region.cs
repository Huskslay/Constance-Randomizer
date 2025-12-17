using Constance;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items.Types.Progressive;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Locations.Types;
using RandomizerCore.Classes.Storage.Locations.Types.Deposits;
using RandomizerCore.Classes.Storage.Saved;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Regions;

[Serializable]
public class Region : ISavedDataOwner<RegionSavedData>
{
    private readonly string name;
    public readonly ConLevelId id;

    public readonly List<Transition> transitions = [];
    public readonly ElevatorTransition elevator = null;

    public readonly List<LightStoneLocation> lightStoneLocations = [];
    public readonly List<CurrencyFlowerLocation> currencyFlowerLocations = [];
    public readonly List<ChestLocation> chestLocations = [];
    public readonly List<CanvasLocation> canvasLocations = [];
    public readonly List<InspirationLocation> inspirationLocations = [];
    public readonly List<ShopItemLocation> shopItemLocations = [];
    public readonly List<DropBehaviourLocation> dropBehaviourLocations = [];
    public readonly List<FoundryPipeLocation> foundryPipeLocations = [];
    public readonly CousinLocation cousinLocation = null;
    public TearLocation tearLocation = null;

    private List<ALocation> allLocations = null;
    private List<ALocation> allLocationsIncludeUnused = null;

    private RegionSavedData savedData = null;

    public Region(ConLevelId id,
                  List<CConTeleportPoint> foundTransitions,
                  CConElevatorBehaviour foundElevator,
                  List<CConCurrencyDepositEntity> foundDeposits,
                  List<CConChestEntity> foundChests,
                  List<CConUnlockAbilityCanvas> foundCanvases,
                  List<CConInspirationTriggerBehaviour> foundInspirations,
                  List<SConCollectable_ShopItem> foundShopItems,
                  List<CConEntityDropBehaviour_TouchToCollect> foundDropBehaviours,
                  List<ConFoundryPaintPipe_Valve> foundFoundryPipes,
                  CConBehaviour_LostShopKeeper foundCousin)
    {
        name = id.StringValue.Replace("Prod_", "");
        this.id = id;

        foreach (var transition in foundTransitions) transitions.Add(new(transition, this));
        if (foundElevator != null) elevator = new(this);

        foreach (var deposit in foundDeposits) DepositLocationFactory.CreateDepositLocation(
            deposit, this, ref lightStoneLocations, ref currencyFlowerLocations);
        foreach (var chest in foundChests) chestLocations.Add(new(chest, this));
        foreach (var canvas in foundCanvases) canvasLocations.Add(new(canvas, this));
        foreach (var inspiration in foundInspirations) inspirationLocations.Add(new(inspiration, this));
        foreach (var shopItem in foundShopItems) shopItemLocations.Add(new(shopItem, this));
        foreach (var dropBehaviour in foundDropBehaviours) dropBehaviourLocations.Add(new(dropBehaviour, this));
        foreach (var foundryPipe in foundFoundryPipes) foundryPipeLocations.Add(new(foundryPipe, this));
        if (foundCousin != null) cousinLocation = new(foundCousin, this);
    }

    public void SetTearLocation(ProgressiveItemInstance progressiveItemInstance, SConCollectable tear)
    {
        tearLocation = new(progressiveItemInstance, tear, this);
    }

    public void Init()
    {
        allLocations = GetLocations(RandomizableItems.All, true);
        allLocationsIncludeUnused = GetLocations(RandomizableItems.All, true, onlyUsed: false);
    }


    public List<ALocation> GetIncludedLocations(RandomizableItems items)
    {
        return GetLocations(items, true);
    }
    public List<ALocation> GetUnincludedLocations(RandomizableItems items)
    {
        return GetLocations(items, false);
    }
    public List<ALocation> GetAllLocations()
    {
        return allLocations;
    }
    public List<ALocation> GetAllLocationsIncludeUnused()
    {
        return allLocationsIncludeUnused;
    }

    private List<ALocation> GetLocations(RandomizableItems includedItems, bool include, bool onlyUsed = true)
    {
        List<ALocation> locations = [];
        TryAdd(ref locations, lightStoneLocations, includedItems, include, onlyUsed);
        TryAdd(ref locations, currencyFlowerLocations, includedItems, include, onlyUsed);
        TryAdd(ref locations, chestLocations, includedItems, include, onlyUsed);
        TryAdd(ref locations, canvasLocations, includedItems, include, onlyUsed);
        TryAdd(ref locations, inspirationLocations, includedItems, include, onlyUsed);
        TryAdd(ref locations, shopItemLocations, includedItems, include, onlyUsed);
        TryAdd(ref locations, dropBehaviourLocations, includedItems, include, onlyUsed);
        TryAdd(ref locations, foundryPipeLocations, includedItems, include, onlyUsed);
        if (cousinLocation != null) TryAdd(ref locations, [cousinLocation], includedItems, include, onlyUsed);
        if (tearLocation != null) TryAdd(ref locations, [tearLocation], includedItems, include, onlyUsed);
        return locations;
    }
    private void TryAdd<T>(ref List<ALocation> locations, List<T> toAdd, RandomizableItems includedItems, bool include, bool onlyUsed = true) where T : ALocation
    {
        if (toAdd == null || toAdd.Count == 0) return;
        if (include == includedItems.HasFlag(toAdd[0].GetItemType()))
        {
            foreach (ALocation location in toAdd)
            {
                if (!onlyUsed || location.GetSavedData() != null && location.GetSavedData().used)
                    locations.Add(location);
            }
        }
    }

    public string GetFullName()
    {
        return name;
    }

    public RegionSavedData GetSavedData()
    {
        return savedData;
    }

    public void SetSavedData(RegionSavedData savedData)
    {
        this.savedData = savedData;
    }
}