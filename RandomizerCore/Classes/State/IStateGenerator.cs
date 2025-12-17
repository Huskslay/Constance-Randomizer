using RandomizerCore.Classes.Storage.Requirements.Entries;

namespace RandomizerCore.Classes.State;

public interface IStateGenerator
{
    RandomState NewRandomizer(int seed, RandomizableItems includedItems, SkipEntries includedSkips);
    bool TryLoadRandomizer(out RandomState state);
}
