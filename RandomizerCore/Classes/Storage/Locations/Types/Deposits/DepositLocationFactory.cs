using Constance;
using Newtonsoft.Json;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace RandomizerCore.Classes.Storage.Locations.Types.Deposits;

public static class DepositLocationFactory
{
    public static FindObjectsInactive FindInactive => FindObjectsInactive.Exclude;

    public static RandomizableItems EntityToDepositType(CConCurrencyDepositEntity deposit)
    {
        string lowerName = deposit.name.ToLower();
        if (lowerName.Contains("lightstone")) return RandomizableItems.LightStones;
        else if (lowerName.Contains("currencyflower")) return RandomizableItems.CurrencyFlowers;

        throw new NotSupportedException($"No ALocation for deposit of name {deposit.name}");
    }

    public static void CreateDepositLocation(CConCurrencyDepositEntity deposit, Region region, ref List<LightStoneLocation> lightStoneLocations, ref List<CurrencyFlowerLocation> currencyFlowerLocations)
    {
        switch (EntityToDepositType(deposit))
        {
            case RandomizableItems.LightStones:
                lightStoneLocations.Add(new LightStoneLocation(deposit, region));
                return;
            case RandomizableItems.CurrencyFlowers:
                currencyFlowerLocations.Add(new CurrencyFlowerLocation(deposit, region));
                return;
            default:
                throw new NotSupportedException($"No ALocation for deposit of name {deposit.name}");
        }
    }

    public static void PatchLoadedLevel(List<CConCurrencyDepositEntity> deposits, List<LightStoneLocation> depositLocations, List<CurrencyFlowerLocation> currencyFlowerLocations)
    {
        List<CConCurrencyDepositEntity> toRando = [];
        foreach (CConCurrencyDepositEntity deposit in deposits)
        {
            RandomizableItems type = EntityToDepositType(deposit);
            if (RandomState.IsRandomized(type)) toRando.Add(deposit);
        }

        List<ALocation> locations = depositLocations.ConvertAll(x => (ALocation)x);
        locations.AddRange(currencyFlowerLocations);
        ALocation.BasicPatch(toRando, locations);
    }
}