using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Handlers.State;

public interface IStateGenerator
{
    RandomState NewRandomizer(int seed, RandomizableItems includedItems, SkipEntries includedSkips);
    bool TryLoadRandomizer(out RandomState state);
}
