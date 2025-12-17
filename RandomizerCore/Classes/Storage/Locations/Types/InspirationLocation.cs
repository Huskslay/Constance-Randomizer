using Constance;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class InspirationLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.Inspirations;
    public static FindObjectsInactive FindInactive => FindObjectsInactive.Exclude;


    private readonly string itemName;
    protected override string GetDisplayItemNameInner() => itemName;

    private readonly AItem item;
    public override AItem GetItem() => item;


    public InspirationLocation(CConInspirationTriggerBehaviour inspiration, Region region) : base(ConvertName(inspiration), inspiration.name, region)
    {
        itemName = GetColName(inspiration.inspiration);
        item = new CollectableItem(this, inspiration.inspiration);
    }
    private static string ConvertName(CConInspirationTriggerBehaviour inspiration)
    {
        return $"Inspiration-{GetColName(inspiration.inspiration)}";
    }
    private static string GetColName(SConCollectable collectable)
    {
        return collectable.name.Replace("inspDrawing_", "");
    }


    public static void PatchLoadedLevel(List<CConInspirationTriggerBehaviour> inspirations, List<InspirationLocation> inspirationLocations)
    {
        if (!RandomState.IsRandomized(RandomizableItems.Inspirations)) return;
        BasicPatch(inspirations, inspirationLocations.ConvertAll(x => (ALocation)x),
            (inspiration, location) =>
            {
                inspiration.inspiration = null;

                // Hide the inspiration if it has been collected already
                if (!RandomState.TryGetElement(location, out RandomStateElement element))
                {
                    Plugin.Logger.LogWarning($"Could not find a random element for location: {location.GetFullName()}");
                    return;
                }
                if (element.hasObtainedSource)
                {
                    inspiration.gameObject.SetActive(false);
                    inspiration.SetOwned();
                }
                if (!inspiration.cinematicOnly)
                {
                    inspiration.vfxFloating.Play();
                    inspiration.vfxGroundGlow.Play();
                    inspiration.sfxIdle.TryPlay(inspiration.Entity);
                }
            }
        );
    }
}