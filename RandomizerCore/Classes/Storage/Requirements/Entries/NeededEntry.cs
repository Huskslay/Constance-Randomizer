using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RandomizerCore.Classes.Storage.Requirements.Entries;

[Serializable]
public class NeededEntry
{
    public ItemEntries items = ItemEntries.None;
    public SkipEntries skips = SkipEntries.None;
    public DifficultyEntries difficulty = DifficultyEntries.Simple;

    public static NeededEntry Constructor(ItemEntries items, SkipEntries skips, DifficultyEntries difficulty)
    {
        return new NeededEntry() { items = items, skips = skips, difficulty = difficulty };
    }
}
