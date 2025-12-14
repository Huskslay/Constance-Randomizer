using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Items.Types.LootBag;

[Serializable]
public class LootBagItem
{
    public int currencyMin;
    public int currencyMax;

    public List<int> lightStoneMins;
    public List<int> lightStoneMaxs;

    public LootBagItem(IConLootBag lootBag)
    {
        currencyMin = lootBag.CurrencyDropRange.x;
        currencyMax = lootBag.CurrencyDropRange.y;

        lightStoneMins = [];
        lightStoneMaxs = [];
        if (lootBag.Entries == null) return;
        foreach (ConLootEntry entry in lootBag.Entries)
        {
            lightStoneMins.Add(entry.amount.x);
            lightStoneMaxs.Add(entry.amount.y);
        }
    }

    public void GiveToPlayer(IConPlayerEntity player, IConPlayerInventory inventoryManager)
    {
        Random rand = new();

        int count = rand.Next(currencyMin, currencyMax + 1);
        Plugin.Logger.LogMessage($"Giving {count} currency ({currencyMin}-{currencyMax})");
        inventoryManager.Collect(player, CollectableHandler.NameToCollectable("currency"), count);

        for (int i = 0; i < lightStoneMins.Count; i++)
        {
            count = rand.Next(lightStoneMins[i], lightStoneMaxs[i] + 1);
            Plugin.Logger.LogMessage($"Giving {count} light stones ({lightStoneMins[i]}-{lightStoneMaxs[i]})");
            inventoryManager.Collect(player, CollectableHandler.NameToCollectable("lightStone"), count);
        }
    }
}
