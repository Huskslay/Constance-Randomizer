using Constance;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace RandomizerCore.Classes.State;

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
        onLocationGet.AddListener(FindItem);
    }

    public static void TryLoadRandomizerState()
    {
        if (Randomized)
        {
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
        if (Randomized)
        {
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
    public static bool TryGetElementFromDest(ALocation dest, out RandomStateElement element)
    {
        element = null;
        if (!Randomized) return false;
        element = Instance.LocationMap.Values.ToList().Find(x => x.dest == dest);
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

        element.dest.GiveItems();
        onLocationGet?.Invoke(element);
        element.hasObtainedSource = true;
    }

    private static void FindItem(RandomStateElement element)
    {
        LocationSavedData saveData = element.dest.GetSavedData();
        Instance.FoundItems |= saveData.givenItems;
        if (saveData.givenEvents > EventsEntries.None) AchieveEvents(saveData.givenEvents);
    }
    public static void AchieveEvents(EventsEntries events)
    {
        Plugin.Logger.LogMessage($"Given event {events}");
        Instance.FoundEvents |= events;
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