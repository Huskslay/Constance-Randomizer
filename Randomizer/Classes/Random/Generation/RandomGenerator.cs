using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;

namespace Randomizer.Classes.Random.Generation;

public class RandomGenerator : IStateGenerator
{
    public RandomState GenerateRandom(int seed, RandomizableItems items, SkipEntries allowedSkips, ItemEntries foundItems, EventsEntries foundEvents, out List<Spoiler> spoilerLog, SerializeState current = null)
    {
        System.Random rand = new(seed);
        int mapIndex = 0;
        spoilerLog = [];

        List<ALocation> toRando = [];
        List<ALocation> nonRando = [];
        foreach (Region region in RegionsHandler.I.GetAll())
        {
            if (region == null)
            {
                Plugin.Logger.LogWarning($"Region was null");
                continue;
            }

            toRando.AddRange(region.GetIncludedLocations(items));
            nonRando.AddRange(region.GetUnincludedLocations(items));
        }

        Dictionary<string, RandomStateElement> randoMap = [];
        foreach (ALocation location in toRando)
        {
            RandomStateElement element = new(location, null, HasObtained(current, ref mapIndex), isRandomized: true);
            randoMap.Add(element.source.GetFullName(), element);
        }
        foreach (ALocation location in nonRando)
        {
            RandomStateElement element = new(location, location, HasObtained(current, ref mapIndex), isRandomized: false);
            randoMap.Add(element.source.GetFullName(), element);
        }

        if (nonRando.Count + toRando.Count != randoMap.Count)
        {
            Plugin.Logger.LogError($"toRandomize size (({nonRando.Count} Non Rando + {toRando.Count} rando) = {nonRando.Count + toRando.Count}) != randoMap size ({randoMap.Count})");
            return null;
        }

        randoMap = RandomSearch.Generate(ref rand, randoMap, toRando, allowedSkips);
        if (randoMap == null) return null;
        RandomState state = new(seed, items, allowedSkips, foundItems, foundEvents, randoMap, []);

        foreach (ALocation location in toRando) spoilerLog.Add(new(randoMap[location.GetFullName()]));


        return state;
    }
    private bool HasObtained(SerializeState current, ref int mapIndex)
    {
        if (current == null) return false;
        if (mapIndex >= current.states.Count)
            throw new Exception("Saved randomizer data does not match item counts");
        return current.states[mapIndex++];
    }

    public RandomState NewRandomizer(int seed, RandomizableItems includedItems, SkipEntries includedSkips)
    {
        Plugin.Logger.LogMessage("Generating randomizer");
        return GenerateRandom(seed, includedItems, includedSkips, ItemEntries.None, EventsEntries.None, out _);
    }

    public bool TryLoadRandomizer(out RandomState state)
    {
        state = null;
        SerializeState current = RandomFiles.Load();
        if (current == null) return false;
        state = GenerateRandom(current.seed, current.includedItems, current.includedSkips, current.foundItems, current.foundEvents, out _, current);
        return state != null;
    }
}
