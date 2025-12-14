using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Requirements.IRequirements;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;

[Serializable]
public class EventRequirement : IRequirement
{
    public EventsEntries evnt = EventsEntries.None;

    public List<TransitionRequirement> requirements = [];
}
