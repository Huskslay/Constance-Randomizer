using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Requirements.IRequirements;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using Sonity;
using System;
using System.Collections.Generic;

namespace Randomizer.Classes.Random.Generation;

public static class RandomSearch
{
    public static readonly ConCheckPointId startCheckpointId = new("cp_Prod_V01_7d6bf550-4ce0-11ef-a7cf-35ea7ade9f40");

    private static readonly ConCheckPointId searchCheckpointId = new("cp_Prod_P02_7c925e30-4ce0-11ef-a7cf-0d4ca85fd37f");
    private static readonly int maxAttempts = 1;//100;
    private static readonly bool log = false;

    private static SkipEntries allowedSkips;
    private static bool finalSearch = false;

    public static Dictionary<string, RandomStateElement> Generate(ref System.Random rand, Dictionary<string, RandomStateElement> randoMap, List<ALocation> toRandomize, SkipEntries allowedSkips, ItemEntries foundItems, EventsEntries foundEvents)
    {
        if (!RegionHandler.TryGetTransitionFromDestinationCheckpoint(searchCheckpointId, out Transition start)) {
            Plugin.Logger.LogError($"Could not find start transition '{start}' for generation");
            return null;
        }

        RandomSearch.allowedSkips = allowedSkips;


        for (int i = 0; i < maxAttempts; i++)
        {
            Plugin.Logger.LogMessage($"Attempt {i + 1}");

            foreach (RandomStateElement element in randoMap.Values)
            {
                if (element.isRandomized) element.dest = null;
            }

            List<ALocation> toRando = [];
            List<ALocation> important = []; 
            ItemEntries newFoundItems = foundItems;
            EventsEntries newFoundEvents = foundEvents;
            int newFoundCousins = 0;

            foreach (Region region in RegionHandler.Regions)
            {
                foreach (EventRequirement requirement in region.GetSavedData().obtainableEvents.requirements)
                    newFoundEvents |= requirement.evnt;
            }
            foreach (ALocation item in toRandomize)
            {
                if (item.GetSavedData().givenItems > 0 && !item.GetSavedData().givenItems.HasFlag(ItemEntries.Tears)
                    || item.GetSavedData().givenEvents > 0)
                {
                    important.Add(item);
                    newFoundItems |= item.GetSavedData().givenItems;
                    newFoundEvents |= item.GetSavedData().givenEvents;
                    if (item.GetSavedData().givenItems.HasFlag(ItemEntries.Cousin)) newFoundCousins++;
                }
                else toRando.Add(item);
            }
            if (ValidateFoundAll(newFoundItems, newFoundEvents, newFoundCousins)) break;


            List<ALocation> reachable;
            newFoundItems = foundItems;
            newFoundEvents = foundEvents;
            newFoundCousins = 0;

            while (important.Count > 0)
            {
                reachable = GetReachableUnrandodLocations(ref randoMap, newFoundItems, newFoundEvents, newFoundCousins, start, out EventsEntries reachedEvents);
                if (reachable.Count == 0) break;

                int chosenReachable = rand.Next(reachable.Count);
                int chosenImportant = rand.Next(important.Count);
                randoMap[reachable[chosenReachable].GetFullName()].dest = important[chosenImportant];

                ItemEntries newItems = important[chosenImportant].GetSavedData().givenItems;
                reachedEvents |= important[chosenImportant].GetSavedData().givenEvents;

                newFoundItems |= newItems;
                newFoundEvents |= reachedEvents;
                if (newItems.HasFlag(ItemEntries.Cousin)) newFoundCousins++;
                important.RemoveAt(chosenImportant);
            }
            if (important.Count > 0) { 
                Plugin.Logger.LogError("Failed to place all important locations");
                foreach (ALocation location in important)
                    Plugin.Logger.LogWarning($"Did not find {location.GetFullName()}");
                continue; 
            }
            if (ValidateFoundAll(newFoundItems, newFoundEvents, newFoundCousins)) break;


            finalSearch = true;
            reachable = GetReachableUnrandodLocations(ref randoMap, newFoundItems, newFoundEvents, newFoundCousins, start, out _);
            finalSearch = false;

            if (reachable.Count != toRando.Count) {

                Plugin.Logger.LogError($"Some locations cannot not reached after all importants put in {reachable.Count} / {toRando.Count}");
                foreach (Region region in RegionHandler.Regions)
                {
                    foreach (ALocation location in region.GetAllLocations())
                    {
                        if (randoMap[location.GetFullName()].dest != null) continue;
                        if (!reachable.Contains(location)) Plugin.Logger.LogWarning($"Could not find location {location.GetFullName()}");
                    }
                }
                continue; 
            }


            while (toRando.Count > 0)
            {
                int chosenReachable = rand.Next(reachable.Count);
                int chosenToRando = rand.Next(toRando.Count);
                randoMap[reachable[chosenReachable].GetFullName()].dest = toRando[chosenToRando];
                toRando.RemoveAt(chosenToRando);
                reachable.RemoveAt(chosenReachable);
            }
            break;
        } 

        return randoMap;
    }

    private static bool ValidateFoundAll(ItemEntries foundItems, EventsEntries foundEvents, int foundCousins)
    {
        bool doBreak = false;
        foreach (ItemEntries item in Enum.GetValues(typeof(ItemEntries)))
        {
            if (item == ItemEntries.All || item == ItemEntries.Tears) continue;
            if (!foundItems.HasFlag(item))
            {
                Plugin.Logger.LogError($"Does not collect item {item}");
                doBreak = true;
            }
        }
        foreach (EventsEntries evnt in Enum.GetValues(typeof(EventsEntries)))
        {
            if (evnt == EventsEntries.All) continue;
            if (!foundEvents.HasFlag(evnt))
            {
                Plugin.Logger.LogError($"Does not collect event {evnt}");
                doBreak = true;
            }
        }
        if (foundCousins != 4)
        {
            Plugin.Logger.LogError($"Only {foundCousins} / 4 cousins");
            doBreak = true;
        }
        return doBreak;
    }


    private static List<ALocation> GetReachableUnrandodLocations(ref Dictionary<string, RandomStateElement> randoMap, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, Transition start, out EventsEntries reachedRegionEvents)
    {
        RandomSearchHolder<ATransition, ALocation> search = new(start, start.GetLinkedTransition());

        bool gottenToElevators = false;
        List<Region> foundRegions = [];
        reachedRegionEvents = EventsEntries.None;

        while (search.NotEmpty())
        {
            ATransition transition = search.PopOpen();

            if (log)
            {
                Plugin.Logger.LogMessage("");
                Plugin.Logger.LogMessage("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Plugin.Logger.LogMessage($"Transition: {transition.GetFullName()}");
                Plugin.Logger.LogMessage("      -------------------------      ");

                Plugin.Logger.LogMessage("-> Items <-");
            }
            
            if (!foundRegions.Contains(transition.GetRegion()))
            {
                foundRegions.Add(transition.GetRegion());
                reachedRegionEvents |= TryReachEvents(foundItems, foundEvents, foundCousins, transition.GetRegion());
            }

            TryReachItems(ref search, randoMap, foundItems, foundEvents, foundCousins, transition);

            if (log) Plugin.Logger.LogMessage("-> Transitions <-");
            AddReachableTransitions(ref search, foundItems, foundEvents, foundCousins, ref gottenToElevators, transition);

            search.AddToClosed(transition);
        }

        return search.GetFound();
    }


    private static EventsEntries TryReachEvents(ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, Region region)
    {
        EventsEntries newEvents = EventsEntries.None;

        foreach (EventRequirement evnt in region.GetSavedData().obtainableEvents.requirements)
        {
            foreach (TransitionRequirement requirement in evnt.requirements)
            {
                ReachableState reachableState = IsReachableFromRequirement(foundItems, foundEvents, foundCousins, requirement);

                if (reachableState == ReachableState.Possible)
                {
                    newEvents |= evnt.evnt;
                    break;
                }
                else if (finalSearch) Plugin.Logger.LogError($"Could not reach region '{region.GetFullName()}' event '{evnt.evnt}' on the final search");
            }
        }

        return newEvents;
    }

    
    private static void TryReachItems(ref RandomSearchHolder<ATransition, ALocation> search, Dictionary<string, RandomStateElement> randoMap, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, ATransition start)
    {
        foreach (ALocation location in start.GetRegion().GetAllLocations())
        {
            if (randoMap[location.GetFullName()].dest != null || search.FoundContains(location)) continue;

            RequirementsOwner<TransitionRequirement> neededRequirements = location.GetSavedData().neededRequirements;
            ReachableState reachableState = IsReachableFromTransition(start, foundItems, foundEvents, foundCousins, neededRequirements);

            if (log) Plugin.Logger.LogMessage($"- Location {location.GetFullName()}: {reachableState}");
            if (reachableState == ReachableState.Possible) search.AddToFound(location);
            else if (finalSearch) Plugin.Logger.LogError($"Could not reach location '{location.GetFullName()}' on the final search");
        }
    }


    private static void AddReachableTransitions(ref RandomSearchHolder<ATransition, ALocation> search, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, ref bool gottenToElevators, ATransition start)
    {
        List<ATransition> found = [];
        
        Region region = start.GetRegion();
        foreach (Transition transition in region.transitions)
            AddTransitionIfReachable(ref search, foundItems, foundEvents, foundCousins, ref gottenToElevators, start, transition, GetTransitionLinks);

        // See if you can reach a non-auto unlock in order to unlock the auto unlocks
        if (!gottenToElevators && region.elevator != null && !region.elevator.GetSavedData().autoUnlock && EntryInfo.UnlockedElevator(foundEvents))
            AddTransitionIfReachable(ref search, foundItems, foundEvents, foundCousins, ref gottenToElevators, start, region.elevator, GetElevatorLinks);
    }
    private static void AddTransitionIfReachable(ref RandomSearchHolder<ATransition, ALocation> search, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, ref bool gottenToElevators, ATransition start, ATransition transition, Func<ATransition, List<ATransition>> getLinks)
    {
        if (transition == start || search.SkipSearch(transition)) return;

        RequirementsOwner<TransitionRequirement> neededRequirements = transition.GetSavedData().neededRequirements;
        ReachableState reachableState = IsReachableFromTransition(start, foundItems, foundEvents, foundCousins, neededRequirements);

        if (log) Plugin.Logger.LogMessage($"- Transition {transition.GetFullName()}: {reachableState}");
        if (reachableState == ReachableState.Possible)
        {
            if (transition is ElevatorTransition) gottenToElevators = true;
            List<ATransition> links = getLinks(transition);
            search.AddToOpen(links);
        }
        else if (finalSearch) Plugin.Logger.LogError($"Could not reach transition '{transition.GetFullName()}' on the final search");
    }

    private static List<ATransition> GetTransitionLinks(ATransition aTransition)
    {
        Transition transition = aTransition as Transition;
        List<ATransition> links = [transition];
        ATransition linked = transition.GetLinkedTransition();
        if (linked != null) links.Add(linked);
        else Plugin.Logger.LogWarning($"Transition '{transition.GetFullName()} does not have a link");
        return links;
    }
    private static List<ATransition> GetElevatorLinks(ATransition aTransition)
    {
        ElevatorTransition transition = aTransition as ElevatorTransition;
        List<ATransition> links = RegionHandler.GetElevatorTransitions().FindAll(x => x.GetSavedData().autoUnlock).ConvertAll(x => (ATransition)x);
        return links;
    }

    private static ReachableState IsReachableFromTransition(ATransition start, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, RequirementsOwner<TransitionRequirement> neededRequirements)
    {
        TransitionRequirement transitionRequirement = neededRequirements.requirements.Find(x => x.transition == start.GetFullName());
        if (transitionRequirement == null)
        {
            Plugin.Logger.LogWarning($"Missing connection between transition '{start.GetFullName()}' and '{neededRequirements.GetConnection()}");
            return ReachableState.Impossible;
        }

        return IsReachableFromRequirement(foundItems, foundEvents, foundCousins, transitionRequirement);
    }
    private static ReachableState IsReachableFromRequirement(ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, TransitionRequirement requirement)
    { 
        if (!requirement.possible) return ReachableState.Impossible;

        if (foundCousins < requirement.cousinCount)
        {
            if (finalSearch) Plugin.Logger.LogError($"Final search cannot get to requirement: Not enough cousins");
            return ReachableState.NotEnoughStuff;
        }

        if (requirement.hasEventRequirements)
        {
            if ((foundEvents | requirement.neededEvents) > foundEvents) return ReachableState.NotEnoughStuff;
        }

        if (requirement.options.Count == 0) return ReachableState.Possible;

        ReachableState state = ReachableState.BadSkips;
        foreach (NeededEntry entry in requirement.options)
        {
            // Reaching location needs impossible skips
            if ((allowedSkips | entry.skips) > allowedSkips) continue;

            // Reaching location needs more items or events than found
            if (((foundItems | entry.items) > foundItems))
            {
                state = ReachableState.NotEnoughStuff;
                continue;
            }

            // Reaching is possible
            return ReachableState.Possible;
        }

        if (finalSearch) Plugin.Logger.LogError($"Final search cannot get to requirement: {state}");

        return state;
    }
    private enum ReachableState
    {
        Impossible,
        BadSkips,
        NotEnoughStuff,
        Possible
    }
}

