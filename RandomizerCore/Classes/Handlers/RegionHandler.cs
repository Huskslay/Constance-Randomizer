using Constance;
using FileHandler.Classes;
using Leo;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Saved;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomizerCore.Classes.Handlers;

public static class RegionHandler
{
    private static readonly string regionDataFolder = "Regions";
    public static List<Region> Regions { get; private set; }


    private static readonly string regionSavedDataFolder = "Region Saved Data";
    private static List<RegionSavedData> regionSavedData;
    public static List<RegionSavedData> RegionSavedData => regionSavedData;


    private static readonly string transitionSavedDataFolder = "Transition Saved Data";
    private static List<TransitionSavedData> transitionSavedData;
    public static List<TransitionSavedData> TransitionSavedData => transitionSavedData;


    private static readonly string locationSavedDataFolder = "Location Saved Data";
    private static List<LocationSavedData> locationSavedData;
    public static List<LocationSavedData> LocationSavedData => locationSavedData;



    private static List<ElevatorTransition> allElevatorTransitions = null;


    public static void Init()
    {
        ActionItem.Reset();
        allElevatorTransitions = null;

        LoadRegions();
        List<ISavedDataOwner<RegionSavedData>> regions = Regions.ConvertAll(x => (ISavedDataOwner<RegionSavedData>)x);
        LoadSavedData(ref regionSavedData, regions, regionSavedDataFolder);

        List<ISavedDataOwner<LocationSavedData>> locations = PrepLocations();
        LoadSavedData(ref locationSavedData, locations, locationSavedDataFolder);

        List<ISavedDataOwner<TransitionSavedData>> transitions = PrepTransitions();
        LoadSavedData(ref transitionSavedData, transitions, "Transition Saved Data");
    }
    private static void LoadRegions()
    {
        Regions = FileSaveLoader.LoadClasses<Region>("Regions");

        HashSet<string> names = [];
        foreach (Region region in Regions)
        {
            if (region == null) Plugin.Logger.LogError("Null region found");
            else
            {
                region.Init();
                if (names.Contains(region.GetFullName()))
                    Plugin.Logger.LogError($"Region name '{region.GetFullName()}' is not unique");
                names.Add(region.GetFullName());
            }
        }
        Plugin.Logger.LogMessage($"{Regions.Count} regions found");
        FileSaveLoader.TrySaveClassToJson(names, "Names", "Regions", logSuccess: false);
    }
    private static List<ISavedDataOwner<LocationSavedData>> PrepLocations()
    {
        List<ALocation> locations = [];
        HashSet<string> names = [];

        foreach (Region region in Regions) locations.AddRange(region.GetAllLocationsIncludeUnused()); ;

        foreach (ALocation location in locations)
        {
            if (names.Contains(location.GetFullName())) Plugin.Logger.LogError($"Location name '{location.GetFullName()}' is not unique");
            names.Add(location.GetFullName());
            location.Init();
        }
        Plugin.Logger.LogMessage($"{names.Count} locations found");
        FileSaveLoader.TrySaveClassToJson(names, "Names", "Locations", logSuccess: false);

        return locations.ConvertAll(x => (ISavedDataOwner<LocationSavedData>)x);
    }
    private static List<ISavedDataOwner<TransitionSavedData>> PrepTransitions()
    {
        List<ISavedDataOwner<TransitionSavedData>> saved = [];
        HashSet<string> names = [];
        foreach (Region region in Regions)
        {
            foreach (Transition transition in region.transitions)
            {
                saved.Add(transition);
                if (names.Contains(transition.GetFullName()))
                    Plugin.Logger.LogError($"Transition name '{transition.GetFullName()}' is not unique");
                names.Add(transition.GetFullName());
            }
        }
        foreach (ElevatorTransition transition in GetElevatorTransitions())
        {
            saved.Add(transition);
            if (names.Contains(transition.GetFullName()))
                Plugin.Logger.LogError($"Elevator transition name '{transition.GetFullName()}' is not unique");
            names.Add(transition.GetFullName());
        }
        Plugin.Logger.LogMessage($"{names.Count} transitions found");
        FileSaveLoader.TrySaveClassToJson(names, "Names", "Transitions", logSuccess: false);
        return saved;
    }

    private static void LoadSavedData<T>(ref List<T> savedDatas, List<ISavedDataOwner<T>> owners, string folder) where T : SavedData
    {
        savedDatas = FileSaveLoader.LoadClassesJson<T>(folder);

        HashSet<string> names = [];
        foreach (T savedData in savedDatas)
        {
            if (savedData == null) Plugin.Logger.LogError($"Null saved data found");
            else
            {
                if (names.Contains(savedData.GetConnection()))
                    Plugin.Logger.LogError($"Connection '{savedData.GetConnection()}' is not unique");
                names.Add(savedData.GetConnection());

                ISavedDataOwner<T> owner = owners.Find(x => x.GetFullName() == savedData.GetConnection());
                if (owner == null)
                {
                    Plugin.Logger.LogError($"{savedData.GetConnection()} saved data can not find connection ");
                    continue;
                }
                else
                {
                    owner.SetSavedData(savedData);
                    owners.Remove(owner);
                }
            }
        }

        Plugin.Logger.LogMessage($"{savedDatas.Count} saved datas found");
        if (owners.Count > 0) Plugin.Logger.LogError($"{owners.Count} owners do not contain saved data");
    }
    public static void SaveRegion(Region region, bool log)
    {
        FileSaveLoader.TrySaveClassToJson(region, regionDataFolder, region.GetFullName(), logSuccess: log);
    }
    public static void SaveSaveData(RegionSavedData data, bool log)
    {
        FileSaveLoader.TrySaveClassToJson(data, regionSavedDataFolder, data.GetConnection(), logSuccess: log);
    }
    public static void SaveSaveData(TransitionSavedData data, bool log)
    {
        FileSaveLoader.TrySaveClassToJson(data, transitionSavedDataFolder, data.GetConnection(), logSuccess: log);
    }
    public static void SaveSaveData(LocationSavedData data, bool log)
    {
        FileSaveLoader.TrySaveClassToJson(data, locationSavedDataFolder, data.GetConnection(), logSuccess: log);
    }


    public static List<ElevatorTransition> GetElevatorTransitions()
    {
        if (allElevatorTransitions == null)
        {
            allElevatorTransitions = [];
            foreach (Region region in Regions)
            {
                if (region.elevator != null) allElevatorTransitions.Add(region.elevator);
            }
        }
        return allElevatorTransitions;
    }


    public static bool TryGetRegion(string id, out Region region)
    {
        if (Regions != null)
        {
            region = Regions.Find(x => x.id.StringValue == id);
            return region != null;
        }
        else Plugin.Logger.LogMessage("No regions loaded");

        region = null;
        return false;
    }
    public static bool TryGetRegionFromName(string name, out Region region)
    {
        if (Regions != null)
        {
            region = Regions.Find(x => x.GetFullName() == name);
            return region != null;
        }
        else Plugin.Logger.LogMessage("No regions loaded");

        region = null;
        return false;
    }


    public static bool TryGetTransitionFromDestinationCheckpoint(ConCheckPointId id, out Transition transition)
    {
        transition = null;
        foreach (Region region in Regions)
        {
            transition = region.transitions.Find(x => x.teleportToCheckPoint == id.StringValue);
            if (transition != null) return true;
        }
        return false;
    }
    public static bool TryGetTransitionFromName(string name, out Transition transition)
    {
        transition = null;
        foreach (Region region in Regions)
        {
            transition = region.transitions.Find(x => x.GetFullName() == name);
            if (transition != null) return true;
        }
        return false;
    }

    public static bool TryGetLocationFromName(string name, out ALocation location)
    {
        location = null;
        foreach (Region region in Regions)
        {
            location = region.GetAllLocations().Find(x => x.GetFullName() == name);
            if (location != null) return true;
        }
        return false;
    }




    public static IEnumerator LoadLevel(ConCheckPointId id, CConPlayerEntity player = null, Direction direction = null)
    {
        yield return LoadLevel(id.ExtractLevelId(), player: player, id: id, direction: direction);
    }
    public static IEnumerator LoadLevel(Region region, CConPlayerEntity player = null, Direction direction = null)
    {
        yield return LoadLevel(region.id, player: player, direction: direction);
    }
    public static IEnumerator LoadLevel(string levelId, CConPlayerEntity player = null, Direction direction = null)
    {
        yield return LoadLevel(new ConLevelId(levelId), player: player, direction: direction);
    }
    public static IEnumerator LoadLevel(ConLevelId levelId, CConPlayerEntity player = null, ConCheckPointId? id = null, Direction direction = null)
    {
        player ??= Plugin.FindFirstObjectByType<CConPlayerEntity>();
        ConStateAbility_Player_Transition transitionAbility = player.SM.TransitionAbility;
        CConCheckPointManager checkPointManager = CConSceneRegistry.Instance.CheckPointManager as CConCheckPointManager;
        CConTransitionManager transitionManager = transitionAbility.TransitionManager;
        id ??= checkPointManager.GetFallbackCheckpointId(levelId);

        ConTransitionCommand_Default trans = new(
            (ConCheckPointId)id,
            levelId,
            direction,
            Vector2.zero,
            new(0, Color.green, FadeType.Fade, new(0.25f, new()))
        );

        transitionAbility.StartTransitionIn(trans);
        float start = Time.time;
        yield return new WaitUntil(() => !transitionManager.IsRunning || Time.time - start > 10);
        if (transitionManager.IsRunning) transitionManager.AbortTransition();
    }
}
