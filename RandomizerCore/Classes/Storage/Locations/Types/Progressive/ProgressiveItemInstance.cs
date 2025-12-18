using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Locations.Types.Progressive;

[Serializable]
public class ProgressiveItemInstance(ProgressiveItemType type)
{
    public readonly ProgressiveItemType type = type;
    public readonly List<string> locationNames = [];
    public ProgressiveItemType GetProgressiveType() => type;

    [NonSerialized] private readonly List<ALocation> locations = [];



    public void Init()
    {
        locations.Clear();
        foreach (string locationName in locationNames)
        {
            if (!LocationsHandler.I.TryGetFromName(locationName, out ALocation location))
            {
                Plugin.Logger.LogError($"Could not find location '{locationName}' for type '{type}'");
                continue;
            }
            else AddProgressiveLocation(location);
        }
    }
    private void AddProgressiveLocation(ALocation location)
    {
        if (location is not IProgressiveLocation progressiveLocation)
        {
            Plugin.Logger.LogError($"Location {location.GetFullName()} is not an IProgressiveLocation");
            return;
        }
        int index = progressiveLocation.GetProgressiveIndex();
        while (locations.Count <= index) locations.Add(null);
        if (locations[index] != null) Plugin.Logger.LogError($"Progressive instance type '{type}' has multiple locations of index '{index}'");
        locations[index] = location;
    }

    public void AddItem(IProgressiveLocation location)
    {
        locationNames.Add(location.GetFullName());
    }

    private int GetIndex()
    {
        int collected = 0;
        foreach (ALocation location in locations)
        {
            if (!RandomState.TryGetElementFromDest(location, out RandomStateElement element))
            {
                Plugin.Logger.LogError($"Could not find element for destination: {location.GetFullName()}");
                continue;
            }
            if (element.hasObtainedSource) collected++;
        }
        return collected;
    }

    public string GetItemName()
    {
        int collected = GetIndex();
        return $"{type} - {collected + 1}/{locationNames.Count}";
    }

    public AItem GetItem()
    {
        int collected = GetIndex();
        Plugin.Logger.LogMessage($"Collecting progressive type '{type}' at index {collected + 1}/{locationNames.Count}");

        ALocation location = locations[collected];
        if (location is not IProgressiveLocation progressiveLocation)
        {
            Plugin.Logger.LogError($"Location {location.GetFullName()} is not an IProgressiveLocation");
            return null;
        }
           
        return progressiveLocation.GetProgressiveItem();
    }
}
