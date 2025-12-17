using Constance;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class ShopItemLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.ShopItems;


    private readonly string itemName;
    protected override string GetDisplayItemNameInner() => itemName;

    private readonly AItem item;
    public override AItem GetItem() => item;


    public ShopItemLocation(SConCollectable_ShopItem shopItem, Region region) : base(ConvertName(shopItem), shopItem.name, region)
    {
        itemName = ConvertName(shopItem);
        item = new CollectableItem(this, shopItem, shopItem.ItemAmount);
    }
    private static string ConvertName(SConCollectable_ShopItem shopItem)
    {
        return shopItem.name.Replace("shopItem_", "Shop-");
    }


    public static void PatchLoadedLevel(CConUiPanel_Shop shop, ConLevelId level, List<ShopItemLocation> locations)
    {
        if (!RandomState.IsRandomized(RandomizableItems.ShopItems)) return;
        if (shop != null && level.StringValue == "Prod_J04")
        {
            List<ALocation> itemLocations = [];
            foreach (SConCollectable_ShopItem shopItem in ConMonoBehaviour.SceneRegistry.Collectables.ShopItems)
            {
                ALocation location = locations.Find(location => location.goName == shopItem.name);
                if (location == null)
                {
                    Plugin.Logger.LogWarning($"Could not find a location for shop item: {shopItem.name}");
                    continue;
                }
                itemLocations.Add(location);
            }
            shop.gameObject.AddComponent<ManyLocationComponent>().Set(locations);
        }
    }
}