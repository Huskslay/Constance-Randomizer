using Constance;
using FileHandler.Classes;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types.LootBag;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class ChestLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.Chests;
    public static FindObjectsInactive FindInactive => FindObjectsInactive.Include;


    protected override string GetDisplayItemNameInner() => "Chest";

    private readonly AItem item;
    public override AItem GetItem() => item;


    public ChestLocation(CConChestEntity chest, Region region) : base(ConvertName(chest), chest.name, region)
    {
        item = new LootBagItems(this, chest.lootBags);
    }
    private static string ConvertName(CConChestEntity chest)
    {
        string hash = FileSaveLoader.FourDigitHash(chest.persistable.persistenceId.StringValue);
        return $"Chest-{hash}";
    }


    public static void PatchLoadedLevel(List<CConChestEntity> chests, List<ChestLocation> chestLocations)
    {
        if (!RandomState.IsRandomized(RandomizableItems.Chests)) return;
        BasicPatch(chests, chestLocations);
    }
}