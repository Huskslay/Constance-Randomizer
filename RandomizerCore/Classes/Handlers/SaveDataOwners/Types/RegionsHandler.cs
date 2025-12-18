using Constance;
using FileHandler.Classes;
using Leo;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomizerCore.Classes.Handlers.SaveDataOwners.Types;

public class RegionsHandler : SaveDataOwnerHandler<Region, RegionSavedData>
{
    public static RegionsHandler I { get; private set; }

    public override void Init()
    {
        ActionItem.Reset();
        allElevatorTransitions = null;
        I = this;
        base.Init();
        GetElevatorTransitions();
    }

    protected override string GetName() => "Regions";


    private List<ElevatorTransition> allElevatorTransitions = null;


    protected override void LoadDatas(Action<Region> initiate)
    {
        List<Region> regions = FileSaveLoader.LoadClasses<Region>([GetName()]);

        foreach (Region region in regions) initiate(region);
    }

    public void Save(Region region, bool log)
    {
        FileSaveLoader.TrySaveClassToFile(region, [GetName()], region.GetFullName(), logSuccess: log);
    }

    public List<ElevatorTransition> GetElevatorTransitions()
    {
        if (allElevatorTransitions == null)
        {
            allElevatorTransitions = [];
            foreach (Region region in GetAll().FindAll(x => x.elevator != null))
                allElevatorTransitions.Add(region.elevator);
        }
        return allElevatorTransitions;
    }

    public bool TryGetFromId(string id, out Region found)
    {
        found = default;
        if (IsEmpty()) return false;

        return dataOwners.TryGetFirst(x => x.id.StringValue == id, out found);
    }


    public bool TryGetTransitionFromDestinationCheckpoint(ConCheckPointId id, out Transition transition)
    {
        transition = null;
        foreach (Region region in GetAll())
        {
            transition = region.transitions.Find(x => x.teleportToCheckPoint == id.StringValue);
            if (transition != null) return true;
        }
        return false;
    }
    public bool TryGetTransitionFromName(string name, out Transition transition)
    {
        transition = null;
        foreach (Region region in GetAll())
        {
            transition = region.transitions.Find(x => x.GetFullName() == name);
            if (transition != null) return true;
        }
        return false;
    }
    public bool TryGetLocationFromName(string name, out ALocation location)
    {
        location = null;
        foreach (Region region in GetAll())
        {
            location = region.GetAllLocations().Find(x => x.GetFullName() == name);
            if (location != null) return true;
        }
        return false;
    }




    public IEnumerator LoadLevel(ConCheckPointId id, CConPlayerEntity player = null, Direction direction = null)
    {
        yield return LoadLevel(id.ExtractLevelId(), player: player, id: id, direction: direction);
    }
    public IEnumerator LoadLevel(Region region, CConPlayerEntity player = null, Direction direction = null)
    {
        yield return LoadLevel(region.id, player: player, direction: direction);
    }
    public IEnumerator LoadLevel(string levelId, CConPlayerEntity player = null, Direction direction = null)
    {
        yield return LoadLevel(new ConLevelId(levelId), player: player, direction: direction);
    }
    public IEnumerator LoadLevel(ConLevelId levelId, CConPlayerEntity player = null, ConCheckPointId? id = null, Direction direction = null)
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
        yield return new WaitUntil(() => !transitionManager.IsRunning || Time.time - start > 15);
        if (transitionManager.IsRunning)
        {
            Plugin.Logger.LogError($"Transition did not end fast enough, forcably ending");
            transitionManager.AbortTransition();
        }
        yield return new WaitForSeconds(0.1f);
    }
}
