using Constance;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types.Progressive;
using RandomizerCore.Classes.Storage.Regions;
using System;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class TearLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.Tears;


    protected override string GetDisplayItemNameInner() => ProgressiveItemHandler.GetItemName(instanceName);

    private readonly string instanceName;
    private readonly AItem item;
    public override AItem GetItem() => item;


    public TearLocation(ProgressiveItemInstance progressiveItemInstance, SConCollectable tear, Region region) : base(ConvertName(tear), tear.name, region)
    {
        instanceName = progressiveItemInstance.name;
        item = progressiveItemInstance.AddItem(this);
    }
    private static string ConvertName(SConCollectable tear)
    {
        return tear.name.Replace("unlock_", "");
    }


    public static void PatchLoadedLevel(CConLevel_Flashback flashback, TearLocation location)
    {
        if (!RandomState.IsRandomized(RandomizableItems.Tears) || flashback == null) return;
        flashback.gameObject.AddComponent<LocationComponent>().Set(location);
    }
}
