using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Saved;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Locations;

[Serializable]
public class LocationSavedData(string connection) : SavedData
{
    public string connection = connection;
    public bool completed = false;

    public bool used = true;
    public ItemEntries givenItems = ItemEntries.None;
    public EventsEntries givenEvents = EventsEntries.None;
    public RequirementsOwner<TransitionRequirement> neededRequirements = new(connection);

    public override string GetConnection() => connection;
}
