using Constance;
using FileHandler.Classes;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types.LootBag;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RandomizerCore.Classes.Storage.Locations.Types.Deposits;

[Serializable]
public class CurrencyFlowerLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.CurrencyFlowers;


    protected override string GetDisplayItemNameInner() => $"{((LootBagItems)item).GetMinCurrencyCount()} Glimmer";

    private readonly AItem item;
    public override AItem GetItem() => item;


    public CurrencyFlowerLocation(CConCurrencyDepositEntity deposit, Region region) : base(ConvertName(deposit), deposit.name, region)
    {
        item = new LootBagItems(this, deposit.depositStates);
    }
    private static string ConvertName(CConCurrencyDepositEntity deposit)
    {
        string hash = FileSaveLoader.FourDigitHash(deposit.persistable.persistenceId.StringValue);
        return $"CurrencyFlower-{hash}";
    }
}