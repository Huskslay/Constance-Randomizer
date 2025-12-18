using Constance;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Locations.Types.Progressive.Types;

[Serializable]
public class TearLocation : ALocation, IProgressiveLocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.Tears;

    protected override string GetDisplayItemNameInner() => ProgressiveItemHandler.GetItemName(this);

    private static readonly List<string> order = [
        "tearFoundry",
        "tearVaults",
        "tearAcademy",
        "tearCarnival"
    ];

    private readonly int progressiveIndex;
    public int GetProgressiveIndex() => progressiveIndex;

    private readonly CollectableItem item;
    public override AItem GetItem() => ProgressiveItemHandler.GetItem(this);

    public ProgressiveItemType GetProgressiveType() => ProgressiveItemType.Tears;
    public AItem GetProgressiveItem() => item;


    public TearLocation(SConCollectable tear, Region region) : base(ConvertName(tear), tear.name, region)
    {
        progressiveIndex = -1;
        for (int i = 0; i < order.Count; i++)
        {
            if (CollectableHandler.nameDict[order[i]] == tear.name)
            {
                progressiveIndex = i;
                break;
            }
        }
        if (progressiveIndex == -1) Plugin.Logger.LogError("Index was not found");

        item = new CollectableItem(this, tear);
        ProgressiveItemHandler.AddToInstance(this);
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
