using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Saved;
using System;

namespace RandomizerCore.Classes.Handlers.SaveDataOwners.Types;

public class LocationsHandler : SaveDataOwnerHandler<ISavedDataOwner<LocationSavedData>, LocationSavedData>
{
    public static LocationsHandler I { get; private set; }
    public override void Init()
    {
        I = this;
        base.Init();
    }

    protected override string GetName() => "Locations";


    protected override void LoadDatas(Action<ISavedDataOwner<LocationSavedData>> initiate)
    {
        foreach (Region region in RegionsHandler.I.GetAll())
        {
            foreach (ALocation location in region.GetAllLocations())
                initiate(location);
        }
    }
}
