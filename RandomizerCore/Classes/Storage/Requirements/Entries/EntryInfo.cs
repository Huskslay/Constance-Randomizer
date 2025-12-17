using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Requirements.Entries;

public static class EntryInfo
{
    public static readonly Dictionary<SkipEntries, ItemEntries> skipEntriesRemoveItems = new() {
        { SkipEntries.BlombCloneTp, ItemEntries.BombClone },
        { SkipEntries.BlombCloneMidairPogo, ItemEntries.BombClone | ItemEntries.Pogo },
        { SkipEntries.DarkRooms, ItemEntries.None },
        { SkipEntries.EnemyPogo, ItemEntries.Pogo },
        { SkipEntries.EnemySlice, ItemEntries.Slice },
    };


    private readonly static ItemEntries dontDisplayItems = ItemEntries.MilkshakeInsp | ItemEntries.CloneTpInsp | ItemEntries.Cousin | ItemEntries.Tears;


    public static bool SkipItemEntry(ItemEntries items, SkipEntries skips)
    {
        ItemEntries skip = ItemEntries.None;
        foreach (SkipEntries value in Enum.GetValues(typeof(SkipEntries)))
        {
            if (skips.HasFlag(value) && skipEntriesRemoveItems.ContainsKey(value))
                skip |= skipEntriesRemoveItems[value];
        }
        foreach (ItemEntries value in Enum.GetValues(typeof(ItemEntries)))
        {
            if (dontDisplayItems.HasFlag(value) && items == value) return true;
        }
        return items == ItemEntries.None || items == ItemEntries.All || skip.HasFlag(items);
    }
    public static bool SkipSkipEntry(SkipEntries entry)
    {
        return entry == SkipEntries.None || entry == SkipEntries.All;
    }
    public static bool SkipEventsEntry(EventsEntries entry)
    {
        return entry == EventsEntries.None || entry == EventsEntries.All;
    }

    public static bool UnlockedElevator(EventsEntries foundEvents)
    {
        return foundEvents.HasFlag(EventsEntries.SquarePipe) && foundEvents.HasFlag(EventsEntries.TrianglePipe);
    }
}