using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Skips;

[Serializable]
public class SkipData(SkipEntries skip)
{
    public SkipEntries skip = skip;
    public List<ItemEntries> neededItems = [];
}
