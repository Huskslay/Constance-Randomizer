using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Saved;
using System;

namespace RandomizerCore.Classes.Storage.Transitions;

[Serializable]
public abstract class ATransition(string regionName, TransitionMethod method) : ISavedDataOwner<TransitionSavedData>
{
    private readonly string regionName = regionName;

    public TransitionMethod TransitionMethod => method;

    public virtual string GetFullName() => null;
    public Region GetRegion()
    {
        if (!RegionsHandler.I.TryGetFromName(regionName, out Region region)) return null;
        return region;
    }

    public void Init() { }

    private TransitionSavedData savedData = null;
    public TransitionSavedData GetSavedData() => savedData;
    public void SetSavedData(TransitionSavedData savedData) { this.savedData = savedData; }

}
