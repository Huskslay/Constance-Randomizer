using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Saved;
using System;

namespace RandomizerCore.Classes.Storage.Regions;

[Serializable]
public class RegionSavedData(string connection) : SavedData
{
    public string connection = connection;
    public bool completed = false;

    public RequirementsOwner<EventRequirement> obtainableEvents = new(connection);

    public override string GetConnection() => connection;
}
