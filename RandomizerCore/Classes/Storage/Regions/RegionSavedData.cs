using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Saved;
using System;
using System.Collections.Generic;
using System.Text;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;

namespace RandomizerCore.Classes.Storage.Regions;

[Serializable]
public class RegionSavedData(string connection) : SavedData
{
    public string connection = connection;
    public bool completed = false;

    public RequirementsOwner<EventRequirement> obtainableEvents = new(connection);

    public override string GetConnection() => connection;
}
