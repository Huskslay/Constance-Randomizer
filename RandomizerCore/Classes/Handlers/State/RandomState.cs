using Constance;
using Newtonsoft.Json.Utilities;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace RandomizerCore.Classes.Handlers.State;

public class RandomState(int seed, RandomizableItems includedItems, SkipEntries includedSkips, ItemEntries foundItems, EventsEntries foundEvents, Dictionary<string, RandomStateElement> LocationMap, List<ConLevelId> cousins)
{
    public static RandomState Instance { get; private set; } = null;
    public static bool Randomized { get; private set; } = false;

    private static IStateGenerator generator;


    public static UnityEvent onLoadRandoSave = new();
    public static UnityEvent<RandomStateElement> onLocationGet = new();


    public int Seed { get; private set; } = seed;
    public RandomizableItems IncludedItems { get; private set; } = includedItems;
    public SkipEntries IncludedSkips { get; private set; } = includedSkips;
    public ItemEntries FoundItems { get; private set; } = foundItems;
    public EventsEntries FoundEvents { get; private set; } = foundEvents;
    public Dictionary<string, RandomStateElement> LocationMap { get; private set; } = LocationMap;
    public List<ConLevelId> Cousins { get; private set; } = cousins;


    public static void Init(IStateGenerator generatorMethod)
    {
        generator = generatorMethod;
    }

    public static void TryLoadRandomizerState()
    {
        if (Randomized) {
            Plugin.Logger.LogWarning("Already randomized");
            return;
        }

        if (!generator.TryLoadRandomizer(out RandomState state)) return;

        Instance = state;
        Randomized = true;
        onLoadRandoSave.Invoke();
    }
    public static void RandomizeState(int seed, RandomizableItems includedItems, SkipEntries includedSkips)
    {
        if (Randomized) {
            Plugin.Logger.LogWarning("Already randomized");
            return;
        }

        Instance = generator.NewRandomizer(seed, includedItems, includedSkips);
        Randomized = Instance != null;
        if (Randomized) onLoadRandoSave.Invoke();
    }
    public static void UnRandomizeState()
    {
        Instance = null;
        Randomized = false;
    }

    public static bool TryGetElement(ALocation location, out RandomStateElement element)
    {
        element = null;
        if (!Randomized) return false;
        return Instance.LocationMap.TryGetValue(location.GetFullName(), out element);
    }
    public static bool TryGetElementFromDestName(string location, out RandomStateElement element)
    {
        element = null;
        if (!Randomized) return false;
        element = Instance.LocationMap.Values.ToList().Find(x => x.dest.GetFullName() == location);
        return element != null;
    }
    public static void TryGetItem(ALocation source)
    {
        if (!Randomized) return;

        Plugin.Logger.LogMessage(source.GetFullName());

        if (!Instance.LocationMap.TryGetValue(source.GetFullName(), out RandomStateElement element))
        {
            Plugin.Logger.LogWarning($"Could not find randomization for {source.GetFullName()}");
            return;
        }
        if (element.hasObtainedSource)
        {
            Plugin.Logger.LogWarning($"Already obtained item for {source.GetFullName()}");
            return;
        }

        FindItem(element.dest.GiveItems());
        element.hasObtainedSource = true;
        onLocationGet?.Invoke(element);
    }

    private static void FindItem(ItemEntries item)
    {
        Instance.FoundItems |= item;
    }

    public static void AddFoundCousin(ConLevelId id)
    {
        Instance.Cousins.Add(id);
    }


    public static bool IsRandomized(RandomizableItems item)
    {
        return Instance.IncludedItems.HasFlag(item);
    }
    public static bool IsRandomized(SkipEntries skip)
    {
        return Instance.IncludedSkips.HasFlag(skip);
    }
}