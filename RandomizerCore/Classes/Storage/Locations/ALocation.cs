using Constance;
using Newtonsoft.Json;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types.Progressive;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Saved;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RandomizerCore.Classes.Storage.Locations;

[Serializable]
public abstract class ALocation : ISavedDataOwner<LocationSavedData>
{
    public readonly string name;
    public readonly string goName;
    private readonly string regionName;
    public string GetFullName() => regionName + "-" + name;

    public string GetDisplayItemName() => $"{GetRegion().GetFullName()} - {GetDisplayItemNameInner()}";
    protected abstract string GetDisplayItemNameInner();


    public Region GetRegion()
    {
        if (!RegionHandler.TryGetRegionFromName(regionName, out Region region)) return null;
        return region;
    }


    private LocationSavedData savedData = null;
    public LocationSavedData GetSavedData() => savedData;
    public void SetSavedData(LocationSavedData savedData) { this.savedData = savedData; }


    public abstract AItem GetItem();
    public abstract RandomizableItems GetItemType();



    public ALocation(string name, string goName, Region region)
    {
        this.name = name;
        this.goName = goName;
        this.regionName = region.GetFullName();
    }
    [JsonConstructor]
    public ALocation(string name, string goName, string regionName)
    {
        this.name = name;
        this.goName = goName;
        this.regionName = regionName;
    }

    
    public virtual void Init()
    { }

    public void GiveItems()
    {
        Plugin.Logger.LogMessage($"Giving '{GetFullName()}'");

        IConPlayerEntity player = ConMonoBehaviour.SceneRegistry.PlayerOne;
        IConPlayerInventory inventoryManager = ConMonoBehaviour.SceneRegistry.Inventory;

        GetItem().GiveToPlayer(player, inventoryManager);
    }


    public static void BasicPatch<T, T2>(List<T> objects, List<T2> locations, Action<T, ALocation> onEach = null) where T : MonoBehaviour where T2 : ALocation
    {
        foreach (T obj in objects)
        {
            ALocation location = locations.Find(location => location.goName == obj.name);
            if (location == null)
            {
                Plugin.Logger.LogWarning($"Could not find a location for object: {obj.name}");
                return;
            }
            obj.gameObject.AddComponent<LocationComponent>().Set(location);
            onEach?.Invoke(obj, location);
        }
    }
}