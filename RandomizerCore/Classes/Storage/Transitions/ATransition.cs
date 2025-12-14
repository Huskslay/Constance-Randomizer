using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Saved;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Transitions;

[Serializable]
public abstract class ATransition(string regionName, TransitionMethod method) : ISavedDataOwner<TransitionSavedData>
{
    private readonly string regionName = regionName;

    public TransitionMethod TransitionMethod => method;

    public virtual string GetFullName() => null;
    public Region GetRegion()
    {
        if (!RegionHandler.TryGetRegionFromName(regionName, out Region region)) return null;
        return region;
    }

    private TransitionSavedData savedData = null;
    public TransitionSavedData GetSavedData() => savedData;
    public void SetSavedData(TransitionSavedData savedData) { this.savedData = savedData; }

}
