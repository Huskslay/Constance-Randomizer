using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;

[Serializable]
public class TransitionRequirement : IRequirement
{
    public string transition;

    public bool possible = true;
    public int cousinCount = 0;
    public bool hasEventRequirements = false;
    public EventsEntries neededEvents = EventsEntries.None;
    public List<NeededEntry> options = [];
}
