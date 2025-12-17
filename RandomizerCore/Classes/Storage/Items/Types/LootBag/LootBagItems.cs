using Constance;
using Leo;
using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Items.Types.LootBag;

[Serializable]
public class LootBagItems : AItem
{
    public readonly List<LootBagItem> lootBags;

    public LootBagItems(ALocation location, CConCurrencyDepositEntity.ConCurrencyDepositState[] depositStates) : base(location.GetFullName())
    {
        lootBags = [];
        foreach (CConCurrencyDepositEntity.ConCurrencyDepositState depositState in depositStates)
            lootBags.Add(new(depositState.LootBag));
    }
    public LootBagItems(ALocation location, InterfaceReference<IConLootBag>[] iLootBags) : base(location.GetFullName())
    {
        lootBags = [];
        foreach (InterfaceReference<IConLootBag> lootBag in iLootBags)
            lootBags.Add(new(lootBag.Value));
    }

    public override void GiveToPlayer(IConPlayerEntity player, IConPlayerInventory inventoryManager)
    {
        foreach (LootBagItem lootBag in lootBags) lootBag.GiveToPlayer(player, inventoryManager);
    }

    public int GetMinCurrencyCount()
    {
        int count = 0;
        foreach (LootBagItem lootBag in lootBags) count += lootBag.currencyMin;
        return count;
    }
    public int GetMinLightStoneCount()
    {
        int count = 0;
        foreach (LootBagItem lootBag in lootBags)
        {
            foreach (int val in lootBag.lightStoneMins) count += val;
        }
        return count;
    }
}
