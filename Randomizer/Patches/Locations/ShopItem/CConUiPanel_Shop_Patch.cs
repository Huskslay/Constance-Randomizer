using System;
using System.Collections.Generic;
using System.Text;
using AsmResolver.PE.DotNet.ReadyToRun;
using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Locations;
using UnityEngine.Localization;

namespace Randomizer.Patches.Locations.ShopItem;

[HarmonyPatch(typeof(CConUiPanel_Shop))]
public class CConUiPanel_Shop_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConUiPanel_Shop.OnBuyItem))]
    private static bool OnBuyItem_Prefix(CConUiPanel_Shop __instance, CConUiShopItemButton itemButton)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.ShopItems)) return true;
        
        IConPlayerInventory inventory = ConMonoBehaviour.SceneRegistry.Inventory;
        List<ALocation> locations = __instance.GetComponent<ManyLocationComponent>().Locations;

        ALocation location = locations.Find(x => x.goName == itemButton.ShopItem.name);

        if (!RandomState.TryGetElement(location, out RandomStateElement element)) return true;
        if (element.hasObtainedSource)
        {
            Plugin.Logger.LogWarning($"Already obtained source: {element.source.GetFullName()}");
            return false;
        }
        if (!inventory.Pay(__instance.ShoppingPlayer, inventory.Catalog.Currency, itemButton.ShopItem.Price)) return false;

        inventory.Collect(__instance.ShoppingPlayer, itemButton.ShopItem, 1);
        RandomState.TryGetItem(location);

            __instance.ShowSelectionMenu();
        if (__instance.IsShopEmpty()) ConMonoBehaviour.SceneRegistry.UiPanelManager.ClosePanel(__instance);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConUiPanel_Shop.OnSelectionItemSelected))]
    private static bool OnSelectionItemSelected_Prefix(CConUiPanel_Shop __instance, SConCollectable_ShopItem item)
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.ShopItems)) return true;

        List<ALocation> locations = __instance.GetComponent<ManyLocationComponent>().Locations;
        ALocation location = locations.Find(x => x.goName == item.name);
        

        if (location == null)
        {
            Plugin.Logger.LogWarning($"Could not find location for shop item: {item.name}");
            return true;
        }
        if (!RandomState.TryGetElement(location, out RandomStateElement element))
        {
            Plugin.Logger.LogWarning($"Could not find element for shop item: {item.name}");
            return true;
        }


        __instance.imgItem.sprite = item.ImageSprite;
        __instance.txtName.text = element.dest.name;
        LocalizedString descriptionText = item.DescriptionText;
        descriptionText.SetStringVariable("Amount", "?".ToString());
        __instance.textDescription.text = descriptionText.TryGetText("");

        return false;
    }
}
