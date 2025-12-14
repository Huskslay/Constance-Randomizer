using Constance;
using Leo;
using Newtonsoft.Json;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class CanvasLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.Canvases;
    public static FindObjectsInactive FindInactive => FindObjectsInactive.Exclude;


    private readonly string itemName;
    protected override string GetDisplayItemNameInner() => itemName;

    private readonly AItem item;
    public override AItem GetItem() => item;


    public CanvasLocation(CConUnlockAbilityCanvas canvas, Region region) : base(ConvertName(canvas), canvas.name, region)
    {
        itemName = GetColName(canvas.collectable);
        item = new CollectableItem(this, canvas.collectable);
    }
    private static string ConvertName(CConUnlockAbilityCanvas canvas)
    {
        return $"Canvas-{GetColName(canvas.collectable)}";
    }
    private static string GetColName(SConCollectable collectable)
    {
        return collectable.name.Replace("unlock_Ability_", "");
    }


    public static void PatchLoadedLevel(List<CConUnlockAbilityCanvas> canvases, List<CanvasLocation> canvasLocations)
    {
        if (!RandomState.IsRandomized(RandomizableItems.Canvases)) return;
        BasicPatch(canvases, canvasLocations);
    }
}
