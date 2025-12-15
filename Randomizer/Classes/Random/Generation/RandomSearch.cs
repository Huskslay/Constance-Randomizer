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
using RandomizerCore.Classes.Storage.Skips;
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
    private readonly static bool log = false;

    private static List<ATransition> reachedTransitions;
    private static SkipEntries allowedSkips;

    public static Dictionary<string, RandomStateElement> Generate(ref System.Random rand, Dictionary<string, RandomStateElement> randoMap, List<ALocation> toRandomize, SkipEntries allowedSkips)
    {
        if (!RegionHandler.TryGetTransitionFromDestinationCheckpoint(searchCheckpointId, out Transition start)) {
            Plugin.Logger.LogError($"Could not find start transition '{start}' for generation");
            return null;
        }

        RandomSearch.allowedSkips = allowedSkips;

        for (int i = 0; i < maxAttempts; i++)
        {
            Plugin.Logger.LogMessage($"Attempt {i + 1}");
            int validationCount = 0;

            // Init 
            List<ALocation> toRando = [];
            List<ALocation> important = [];
            int newFoundCousins = 0;
            ItemEntries foundItems = ItemEntries.None;
            EventsEntries foundEvents = EventsEntries.None;


            // Reset toRando items and check off findable items and events for nonrandod items so they're included
            foreach (RandomStateElement element in randoMap.Values)
            {
                if (element.isRandomized) element.dest = null;
                else
                {
                    foundItems |= element.source.GetSavedData().givenItems;
                    foundEvents |= element.source.GetSavedData().givenEvents;
                }
            }
            // Check off region events existing
            foreach (Region region in RegionHandler.Regions)
            {
                foreach (EventRequirement requirement in region.GetSavedData().obtainableEvents.requirements)
                    foundEvents |= requirement.evnt;
            }
            // Find items that are important to rando first and items just to rando, and for important items, check off what is found
            foreach (ALocation item in toRandomize)
            {
                if (item.GetSavedData().givenItems > 0 && !item.GetSavedData().givenItems.HasFlag(ItemEntries.Tears)
                    || item.GetSavedData().givenEvents > 0)
                {
                    important.Add(item);
                    foundItems |= item.GetSavedData().givenItems;
                    foundEvents |= item.GetSavedData().givenEvents;
                    if (item.GetSavedData().givenItems.HasFlag(ItemEntries.Cousin)) newFoundCousins++;
                }
                else toRando.Add(item);
            }
            // Ensure everything needed is checked off
            if (ValidateFoundAll(foundItems, foundEvents, newFoundCousins, ++validationCount)) break;


            // Reset
            List<ALocation> reachable;
            foundItems = ItemEntries.None;
            foundEvents = EventsEntries.None;
            newFoundCousins = 0;
            // Randomize all the important items first in turn
            while (important.Count > 0)
            {
                reachable = GetReachableLocations(foundItems, foundEvents, newFoundCousins, start, (loc) => ShouldSkipLocation(loc, randoMap), out EventsEntries reachedEvents, updateEvents: true);
                if (reachable.Count == 0) break;

                // Choose a random reachabke to set to a random important item
                int chosenReachable = rand.Next(reachable.Count);
                int chosenImportant = rand.Next(important.Count);
                randoMap[reachable[chosenReachable].GetFullName()].dest = important[chosenImportant];

                // Check off found stuff
                ItemEntries newItems = important[chosenImportant].GetSavedData().givenItems;
                reachedEvents |= important[chosenImportant].GetSavedData().givenEvents;
                foundItems |= newItems;
                foundEvents |= reachedEvents;
                if (newItems.HasFlag(ItemEntries.Cousin)) newFoundCousins++;

                // Remove that important
                important.RemoveAt(chosenImportant);
            }
            // Did not manage to put in all important items before available spots ran out
            if (important.Count > 0) { 
                Plugin.Logger.LogError("Failed to place all important locations");
                foreach (ALocation location in important)
                    Plugin.Logger.LogWarning($"Did not find {location.GetFullName()}");
                continue;
            }
            // Ensure everything needed is checked off
            if (ValidateFoundAll(foundItems, foundEvents, newFoundCousins, ++validationCount)) break;

            // Do the final search confirming that every location is reachable, and getting a list of reachable, not randod yet locations
            reachedTransitions = [];
            reachable = GetReachableLocations(foundItems, foundEvents, newFoundCousins, start, (loc) => ShouldSkipLocation(loc, randoMap), out _, updateEvents: true);
            ValidateFoundTransitions();

            // The amount reachable should be equal to the amount left to randomize
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

            // Randomize all the left over locations
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
    private static bool ShouldSkipLocation(ALocation location, Dictionary<string, RandomStateElement> randoMap)
    {
        return randoMap[location.GetFullName()].dest != null;
    }

    private static bool ValidateFoundAll(ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, int validationCount)
    {
        bool doBreak = false;
        // Validate all items found
        foreach (ItemEntries item in Enum.GetValues(typeof(ItemEntries)))
        {
            if (item == ItemEntries.None || item == ItemEntries.All || item == ItemEntries.Tears) continue;
            if (!foundItems.HasFlag(item))
            {
                Plugin.Logger.LogError($"Does not collect item {item} on validation {validationCount}");
                doBreak = true;
            }
        }
        // Validate all events found
        foreach (EventsEntries evnt in Enum.GetValues(typeof(EventsEntries)))
        {
            if (evnt == EventsEntries.None || evnt == EventsEntries.All) continue;
            if (!foundEvents.HasFlag(evnt))
            {
                Plugin.Logger.LogError($"Does not collect event {evnt} on validation {validationCount}");
                doBreak = true;
            }
        }
        // Validate all cousins found
        if (foundCousins != 4)
        {
            Plugin.Logger.LogError($"Only {foundCousins} / 4 cousins on validation {validationCount}");
            doBreak = true;
        }
        return doBreak;
    }
    private static void ValidateFoundTransitions()
    {
        foreach (Region region in RegionHandler.Regions)
        {
            foreach (Transition trans in region.transitions)
            {
                if (!reachedTransitions.Contains(trans)) Plugin.Logger.LogWarning($"Did not reach transition: {trans.GetFullName()}");
            }
        }
        reachedTransitions = null;
    }

    public static List<ALocation> GetReachableLocations(ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, Transition start, Func<ALocation, bool> shouldSkipLocation, out EventsEntries reachedRegionEvents, bool updateEvents = false)
    {
        RandomSearchHolder<ATransition, ALocation> search = new(start, start.GetLinkedTransition());

        bool gottenToElevators = false;
        reachedRegionEvents = EventsEntries.None;

        // Search transitions while there are still transitions to search
        while (search.NotEmpty())
        {
            ATransition transition = search.PopOpen();
            if (reachedTransitions != null && !reachedTransitions.Contains(transition)) reachedTransitions.Add(transition);

            if (log)
            {
                Plugin.Logger.LogMessage("");
                Plugin.Logger.LogMessage("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Plugin.Logger.LogMessage($"Transition: {transition.GetFullName()}");
                Plugin.Logger.LogMessage("      -------------------------      ");

                Plugin.Logger.LogMessage("-> Items <-");
            }

            // Checks if region events can be reached, if 'updateEvents' is true, it will update found events
            // If it is true, it will also add transitions that need more items to a list to check later
            reachedRegionEvents |= TryReachEvents(ref search, foundItems, foundEvents, foundCousins, transition, updateEvents);
            if (updateEvents) foundEvents |= reachedRegionEvents;

            // Checks if it can reach that regions locations
            TryReachItems(ref search, foundItems, foundEvents, foundCousins, transition, shouldSkipLocation);

            // Checks if it can reach that regions other transitions
            if (log) Plugin.Logger.LogMessage("-> Transitions <-");
            AddReachableTransitions(ref search, foundItems, foundEvents, foundCousins, ref gottenToElevators, transition, updateEvents);
            
            // If not for future, add to closed
            search.AddToClosed(transition);
        }

        return search.GetFound();
    }


    private static EventsEntries TryReachEvents(ref RandomSearchHolder<ATransition, ALocation> search, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, ATransition transition, bool updateEvents)
    {
        EventsEntries newEvents = EventsEntries.None;

        // Check each event in the region and if it is reachable from that transition
        foreach (EventRequirement evnt in transition.GetRegion().GetSavedData().obtainableEvents.requirements)
        {
            // Do not bother if flag is already gotten
            if (foundEvents.HasFlag(evnt.evnt)) continue;
            // Check if reachable, if so add to new events, if not but could be with more events (and updating events), add to future checking
            ReachableState reachableState = IsReachableFromTransition(transition, foundItems, foundEvents, foundCousins, evnt.requirements);
            if (reachableState == ReachableState.Possible) newEvents |= evnt.evnt;
            else if (updateEvents && reachableState == ReachableState.NotEnoughEvents) search.AddToFuture(transition);
        }

        return newEvents;
    }

    
    private static void TryReachItems(ref RandomSearchHolder<ATransition, ALocation> search, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, ATransition start, Func<ALocation, bool> shouldSkipLocation)
    {
        foreach (ALocation location in start.GetRegion().GetAllLocations())
        {
            // Don't check already found locations or locations that should be skipped
            if (search.FoundContains(location) || shouldSkipLocation(location)) continue;

            // Get the needed requirements of that location, and check if it is reachable from this transition
            RequirementsOwner<TransitionRequirement> neededRequirements = location.GetSavedData().neededRequirements;
            ReachableState reachableState = IsReachableFromTransition(start, foundItems, foundEvents, foundCousins, neededRequirements.requirements);

            // If it is reachable add to the found locations
            if (log) Plugin.Logger.LogMessage($"- Location {location.GetFullName()}: {reachableState}");
            if (reachableState == ReachableState.Possible) search.AddToFound(location);
        }
    }


    private static void AddReachableTransitions(ref RandomSearchHolder<ATransition, ALocation> search, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, ref bool gottenToElevators, ATransition start, bool updateEvents)
    {
        List<ATransition> found = [];
        Region region = start.GetRegion();

        // Check regions transitions
        foreach (Transition transition in region.transitions)
            AddTransitionIfReachable(ref search, foundItems, foundEvents, foundCousins, ref gottenToElevators, start, transition, GetTransitionLinks, updateEvents);

        // See if you can reach a non-auto unlock in order to unlock the auto unlocks
        if (!gottenToElevators && region.elevator != null && !region.elevator.GetSavedData().autoUnlock && EntryInfo.UnlockedElevator(foundEvents))
            AddTransitionIfReachable(ref search, foundItems, foundEvents, foundCousins, ref gottenToElevators, start, region.elevator, GetElevatorLinks, updateEvents);
    }
    private static void AddTransitionIfReachable(ref RandomSearchHolder<ATransition, ALocation> search, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, ref bool gottenToElevators, ATransition start, ATransition transition, Func<ATransition, List<ATransition>> getLinks, bool updateEvents)
    {
        // Skip if already to search, already searched, or is the starting transition
        if (transition == start || search.SkipSearch(transition)) return;

        // Get if it is reachable from the start transition
        RequirementsOwner<TransitionRequirement> neededRequirements = transition.GetSavedData().neededRequirements;
        ReachableState reachableState = IsReachableFromTransition(start, foundItems, foundEvents, foundCousins, neededRequirements.requirements);

        // if it is reachable, add it and the transitions it links to, if it would be with more events and updating events, add to future
        if (log) Plugin.Logger.LogMessage($"- Transition {transition.GetFullName()}: {reachableState}");
        if (reachableState == ReachableState.Possible)
        {
            if (transition is ElevatorTransition) gottenToElevators = true;
            List<ATransition> links = getLinks(transition);
            search.AddToOpen(links);
        }
        else if (updateEvents && reachableState == ReachableState.NotEnoughEvents) search.AddToFuture(transition);
    }

    private static List<ATransition> GetTransitionLinks(ATransition aTransition)
    {
        // Gets linked portals for a normal transition

        Transition transition = aTransition as Transition;
        List<ATransition> links = [transition];
        ATransition linked = transition.GetLinkedTransition();
        if (linked != null) links.Add(linked);
        else Plugin.Logger.LogWarning($"Transition '{transition.GetFullName()} does not have a link");
        return links;
    }
    private static List<ATransition> GetElevatorLinks(ATransition aTransition)
    {
        // Gets linked portals for an elevator transition

        ElevatorTransition transition = aTransition as ElevatorTransition;
        List<ATransition> links = RegionHandler.GetElevatorTransitions().FindAll(x => x.GetSavedData().autoUnlock).ConvertAll(x => (ATransition)x);
        return links;
    }

    private static ReachableState IsReachableFromTransition(ATransition start, ItemEntries foundItems, EventsEntries foundEvents, int foundCousins, List<TransitionRequirement> neededRequirements)
    {
        TransitionRequirement requirement = neededRequirements.Find(x => x.transition == start.GetFullName());
        if (requirement == null)
        {
            Plugin.Logger.LogWarning($"Missing connection between transition '{start.GetFullName()}' and '{start.GetFullName()}");
            return ReachableState.Impossible;
        }

        // Is marked as impossible
        if (!requirement.possible) return ReachableState.Impossible;
        // Check cousin count
        if (foundCousins < requirement.cousinCount) return ReachableState.NotEnoughStuff;
        // Check needed events
        if (requirement.hasEventRequirements)
        {
            if ((foundEvents & requirement.neededEvents) != requirement.neededEvents) return ReachableState.NotEnoughEvents;
        }
        if (requirement.options.Count == 0) return ReachableState.Possible;


        ReachableState state = ReachableState.BadSkips;
        foreach (NeededEntry entry in requirement.options)
        {
            // Reaching location needs impossible skips
            if ((allowedSkips & entry.skips) != entry.skips) continue;

            // Check that you have enough items for the skip
            if (!SkipDataHandler.SkipIsPossible(entry.skips, foundItems)) continue;

            // Reaching location needs more items or events than found
            if (((foundItems & entry.items) != entry.items))
            {
                state = ReachableState.NotEnoughStuff;
                continue;
            }

            // Reaching is possible
            return ReachableState.Possible;
        }

        return state;
    }
    private enum ReachableState
    {
        Impossible,
        BadSkips,
        NotEnoughStuff,
        NotEnoughEvents,
        Possible
    }
}

