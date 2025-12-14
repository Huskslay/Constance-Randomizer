using Constance;
using CreateRandomizer.Classes.Data;
using FileHandler.Classes;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateRandomizer.Classes;

public class PostScraper : MonoBehaviour
{
    public static void Run(Region region, List<Tuple<ConCheckPointId, ConCheckPointId>> transitionInfos)
    {
        Region(region);
        Locations(region);
        Transitions(region, transitionInfos);
    }

    private static void Region(Region region)
    {
        // Saved Data
        RegionSavedData savedData = RegionSavedData(region);
        region.SetSavedData(savedData);
        FileSaveLoader.TrySaveClassToJson(savedData, "Region Saved Data", savedData.GetConnection(), logSuccess: false);
    }

    private static void Locations(Region region)
    {
        foreach (ALocation location in region.GetAllLocationsIncludeUnused())
        {
            // Saved Data
            LocationSavedData savedData = LocationSavedData(region, location);
            location.SetSavedData(savedData);
            FileSaveLoader.TrySaveClassToJson(savedData, "Location Saved Data", savedData.GetConnection(), logSuccess: false);
        }
    }

    private static void Transitions(Region region, List<Tuple<ConCheckPointId, ConCheckPointId>> transitionInfos) 
    {
        // Tp links 1
        foreach (Tuple<ConCheckPointId, ConCheckPointId> transitionInfo in transitionInfos)
        {
            if (!RegionHandler.TryGetTransitionFromDestinationCheckpoint(transitionInfo.Item1, out Transition one))
            {
                Plugin.Logger.LogWarning($"Could not find transition for checkpoint id: {transitionInfo.Item1.StringValue}");
                continue;
            }
            if (!RegionHandler.TryGetTransitionFromDestinationCheckpoint(transitionInfo.Item2, out Transition two))
            {
                Plugin.Logger.LogWarning($"Could not find transition for checkpoint id: {transitionInfo.Item1.StringValue}");
                continue;
            }
            one.linkedTransition = two.GetFullName();
            two.linkedTransition = one.GetFullName();
        }

        // Transition requirements
        foreach (Transition transition in region.transitions)
        {
            TransitionSavedData savedData = TransitionSavedData(region, transition);
            transition.SetSavedData(savedData);
            FileSaveLoader.TrySaveClassToJson(savedData, "Transition Saved Data", savedData.GetConnection(), logSuccess: false);
        }
        // Elevator requirements
        if (region.elevator != null)
        {
            TransitionSavedData savedData = TransitionSavedData(region, region.elevator, true);
            region.elevator.SetSavedData(savedData);
            FileSaveLoader.TrySaveClassToJson(savedData, "Transition Saved Data", savedData.GetConnection(), logSuccess: false);
        }
    }
    
    private static RegionSavedData RegionSavedData(Region region)
    {
        string FullName = region.GetFullName();
        RegionSavedData savedData = RegionHandler.RegionSavedData.Find(x => x.GetConnection() == FullName);
        savedData ??= new(region.GetFullName());
        return savedData;
    }
    private static TransitionSavedData TransitionSavedData(Region region, ATransition transition, bool isElevator = false)
    {
        string FullName = transition.GetFullName();
        TransitionSavedData savedData = RegionHandler.TransitionSavedData.Find(x => x.GetConnection() == FullName);
        savedData ??= new(transition.GetFullName());
        savedData.neededRequirements = CreateTransitionRequirements(savedData.neededRequirements, region, FullName, isElevator);
        return savedData;
    }
    private static LocationSavedData LocationSavedData(Region region, ALocation location)
    {
        string FullName = location.GetFullName();
        LocationSavedData savedData = RegionHandler.LocationSavedData.Find(x => x.GetConnection() == FullName);
        savedData ??= new(location.GetFullName());
        savedData.neededRequirements = CreateTransitionRequirements(savedData.neededRequirements, region, FullName);
        return savedData;
    }
    private static RequirementsOwner<TransitionRequirement> CreateTransitionRequirements(RequirementsOwner<TransitionRequirement> requirementsOwner, Region region, string FullName, bool isElevator = false)
    {
        // Requirements
        requirementsOwner ??= new(FullName);
        foreach (Transition transition in region.transitions)
        {
            TransitionRequirement transitionReq = requirementsOwner.requirements
                .Find(x => x.transition == transition.GetFullName());
            if (transition.GetFullName() != FullName && transitionReq == null)
            {
                requirementsOwner.requirements.Add(new());
                requirementsOwner.requirements[^1].transition = transition.GetFullName();
            } 
            else if (transition.GetFullName() == FullName && transitionReq != null)
                requirementsOwner.requirements.Remove(transitionReq);
        }

        if (!isElevator && region.elevator != null)
        {
            ElevatorTransition elevator = region.elevator;
            TransitionRequirement transitionReq = requirementsOwner.requirements
                .Find(x => x.transition == elevator.GetFullName());
            if (transitionReq == null)
            {
                requirementsOwner.requirements.Add(new());
                requirementsOwner.requirements[^1].transition = elevator.GetFullName();
            }
        }

        return requirementsOwner;
    }
}
