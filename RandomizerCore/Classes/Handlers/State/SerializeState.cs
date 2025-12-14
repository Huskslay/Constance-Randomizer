using Constance;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Handlers.State;

[Serializable]
public class SerializeState(int seed, RandomizableItems includedItems, SkipEntries includedSkips, ItemEntries foundItems, EventsEntries foundEvents, List<bool> states, List<ConLevelId> cousins)
{
    public int seed = seed;
    public RandomizableItems includedItems = includedItems;
    public SkipEntries includedSkips = includedSkips;
    public ItemEntries foundItems = foundItems;
    public EventsEntries foundEvents = foundEvents;
    public List<bool> states = states;
    public List<ConLevelId> cousins = cousins;

    public static SerializeState Constructor(RandomState state)
    {
        List<bool> bools = [];
        foreach (KeyValuePair<string, RandomStateElement> kvp in RandomState.Instance.LocationMap)
            bools.Add(kvp.Value.hasObtainedSource);

        return new(state.Seed, state.IncludedItems, state.IncludedSkips, state.FoundItems, state.FoundEvents, bools, state.Cousins);
    }
}