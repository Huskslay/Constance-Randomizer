using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Saved;
using System;

namespace RandomizerCore.Classes.Storage.Transitions;

[Serializable]
public class TransitionSavedData(string connection) : SavedData
{
    public string connection = connection;
    public bool completed = false;

    public bool autoUnlock = false;
    public bool doOverrideTransition = false;
    public string overrideTransition = "";
    public TransitionLockType lockType = TransitionLockType.NotLocked;
    public RequirementsOwner<TransitionRequirement> neededRequirements = new(connection);

    public override string GetConnection() => connection;
}
