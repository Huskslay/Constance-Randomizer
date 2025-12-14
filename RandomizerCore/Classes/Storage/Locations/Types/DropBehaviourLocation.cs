using Constance;
using Newtonsoft.Json;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class DropBehaviourLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.DropBehaviours;
    public static FindObjectsInactive FindInactive => FindObjectsInactive.Exclude;


    private readonly string itemName;
    protected override string GetDisplayItemNameInner() => itemName;
    
    private readonly AItem item;
    public override AItem GetItem() => item;


    public DropBehaviourLocation(CConEntityDropBehaviour_TouchToCollect dropBehaviour, Region region) : base(ConvertName(dropBehaviour), dropBehaviour.name, region)
    {
        itemName = GetColName(dropBehaviour.collectable);
        item = new CollectableItem(this, dropBehaviour.collectable);
    }
    private static string ConvertName(CConEntityDropBehaviour_TouchToCollect dropBehaviour)
    {
        return $"Collectable-{GetColName(dropBehaviour.collectable)}";
    }
    private static string GetColName(SConCollectable collectable)
    {
        return collectable.name.Replace("item_", "");
    }


    public static void PatchLoadedLevel(List<CConEntityDropBehaviour_TouchToCollect> dropBehaviours, List<DropBehaviourLocation> dropBehaviourLocations)
    {
        if (!RandomState.IsRandomized(RandomizableItems.DropBehaviours)) return;
        BasicPatch(dropBehaviours, dropBehaviourLocations, 
            (dropBehaviour, location) => { dropBehaviour.collectable = null; });
    }
}