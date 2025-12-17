using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;

[Serializable]
public class EventRequirement : IRequirement
{
    public EventsEntries evnt = EventsEntries.None;

    public List<TransitionRequirement> requirements = [];
}
