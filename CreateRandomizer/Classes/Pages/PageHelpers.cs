using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RandomizerCore.Classes.Storage.Transitions;
using CheatMenu.Classes;
using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System.Linq;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Requirements.IRequirements;
using RandomizerCore.Classes.Storage.Saved;
using RandomizerCore.Classes.Storage.Regions;

namespace CreateRandomizer.Classes.Pages;

public static class PageHelpers
{
    public static List<NeededEntry> copiedEntry = [];


    public static IEnumerator LoadTransition(Transition transition)
    {
        if (transition.GetSavedData().doOverrideTransition)
        {
            yield return RegionHandler.LoadLevel(transition.GetRegion());

            CConTeleportPoint tp = Plugin.FindObjectsByType<CConTeleportPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().Find(x => x.teleportTo.StringValue == transition.teleportToCheckPoint);

            CConPlayerEntity player = Plugin.FindFirstObjectByType<CConPlayerEntity>();
            player.transform.position = tp.transform.position;
        }
        else yield return RegionHandler.LoadLevel(new ConCheckPointId(transition.GetLinkedTransition().teleportToCheckPoint));
    }
    public static IEnumerator LoadTransition(ElevatorTransition transition)
    {
        yield return RegionHandler.LoadLevel(transition.GetRegion());

        CConElevatorBehaviour tp = Plugin.FindFirstObjectByType<CConElevatorBehaviour>();

        CConPlayerEntity player = Plugin.FindFirstObjectByType<CConPlayerEntity>();
        player.transform.position = tp.transform.position;
    }



    public static IEnumerator LoadLocation(ALocation location, Func<List<MonoBehaviour>> getLocations)
    {
        yield return RegionHandler.LoadLevel(location.GetRegion());

        List<MonoBehaviour> behaviours = getLocations();
        MonoBehaviour tp = behaviours.Find(x => x.name == location.goName);

        CConPlayerEntity player = Plugin.FindFirstObjectByType<CConPlayerEntity>();
        player.transform.position = tp.transform.position;
    }

    public static void DrawTransitionRequirements(ref List<TransitionRequirement> requirements, ref int selectedTransition, MonoBehaviour page)
    {
        GUIElements.Line();
        GUILayout.Label("Transitions");
        GUILayout.Space(10);
        for (int i = 0; i < requirements.Count; i++)
        {
            GUIElements.Line();

            GUILayout.BeginHorizontal();
            Color bgColor = GUI.backgroundColor;
            TransitionRequirement transitionRequirement = requirements[i];
            GUI.backgroundColor = selectedTransition == i ? Color.green : Color.red;
            if (GUILayout.Button($"Transition {i} - {transitionRequirement.transition}")) selectedTransition = selectedTransition == i ? -1 : i;
            GUI.backgroundColor = bgColor;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (selectedTransition != i) continue;


            GUILayout.Space(5);
            
            DrawRequirement(ref transitionRequirement, page);

            GUILayout.Space(30);
        }
    }

    public static void DrawRequirement(ref TransitionRequirement requirement, MonoBehaviour page)
    {
        if (GUILayout.Button("Teleport") && RegionHandler.TryGetTransitionFromName(requirement.transition, out Transition goal))
            page.StartCoroutine(LoadTransition(goal));

        requirement.possible = GUIElements.BoolValue("Possible", requirement.possible);

        GUIElements.Line();

        if (requirement.possible)
        {
            requirement.hasEventRequirements = GUIElements.BoolValue("Needed Events", requirement.hasEventRequirements);
            if (requirement.hasEventRequirements)
            {
                GUILayout.BeginHorizontal();
                Color bg = GUI.backgroundColor;
                GUI.backgroundColor = requirement.cousinCount >= 0 && requirement.cousinCount <= 4 ? bg : Color.red;
                GUIElements.IntSelector((_) => "Needed Cousins Found", ref requirement.cousinCount);
                GUI.backgroundColor = bg;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                int index = 0;
                foreach (EventsEntries value in Enum.GetValues(typeof(EventsEntries)))
                {
                    if (EntryInfo.SkipEventsEntry(value)) continue;
                    if (index++ == 5)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        index = 0;
                    }
                    bool enabled = requirement.neededEvents.HasFlag(value);
                    GUI.backgroundColor = enabled ? Color.green : Color.red;
                    if (GUILayout.Button(value.ToString()))
                    {
                        if (enabled) requirement.neededEvents &= ~value;
                        else requirement.neededEvents |= value;
                    }
                }
                GUI.backgroundColor = bg;
                GUILayout.EndHorizontal();
            }

            GUIElements.Line();

            GUIElements.ElipseLine();
            for (int j = 0; j < requirement.options.Count; j++)
            {

                NeededEntry neededEntry = requirement.options[j];
                if (DrawNeededEntryAndReturnRemove(ref neededEntry))
                {
                    requirement.options.RemoveAt(j);
                    break;
                }

            }
            if (GUILayout.Button("Add Items")) requirement.options.Add(new());
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy"))
            {
                copiedEntry = [];
                foreach (NeededEntry neededEntry in requirement.options) 
                    copiedEntry.Add(NeededEntry.Constructor(neededEntry.items, neededEntry.skips, neededEntry.difficulty));
            }
            if (GUILayout.Button("Paste"))
            {
                requirement.options.Clear();
                foreach (NeededEntry neededEntry in copiedEntry)
                    requirement.options.Add(NeededEntry.Constructor(neededEntry.items, neededEntry.skips, neededEntry.difficulty));
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Teleport") && RegionHandler.TryGetTransitionFromName(requirement.transition, out goal))
                page.StartCoroutine(LoadTransition(goal));
        }
    }

    public static bool DrawNeededEntryAndReturnRemove(ref NeededEntry neededEntry)
    {
        Color bg = GUI.backgroundColor;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Needed Items");
        bool remove = GUILayout.Button("X Entry", GUILayout.Width(85));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        int index = 0;
        foreach (ItemEntries value in Enum.GetValues(typeof(ItemEntries)))
        { 
            if (EntryInfo.SkipItemEntry(value, neededEntry.skips)) continue;
            if (index++ == 7)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                index = 0;
            }
            bool enabled = neededEntry.items.HasFlag(value);
            GUI.backgroundColor = enabled ? Color.green : Color.red;
            if (GUILayout.Button(value.ToString())) neededEntry.items ^= value;
        }
        GUI.backgroundColor = bg;
        GUILayout.EndHorizontal();


        GUILayout.Label("Needed Skips");
        GUILayout.BeginHorizontal();
        index = 0;
        foreach (SkipEntries value in Enum.GetValues(typeof(SkipEntries)))
        {
            if (EntryInfo.SkipSkipEntry(value)) continue;
            if (index++ == 4)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                index = 0;
            }
            bool enabled = neededEntry.skips.HasFlag(value);
            GUI.backgroundColor = enabled ? Color.green : Color.red;
            if (GUILayout.Button(value.ToString()))
            {
                if (enabled) neededEntry.skips &= ~value;
                else neededEntry.skips |= value;
            }
        }
        GUI.backgroundColor = bg;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        neededEntry.difficulty = GUIElements.EnumValue("Difficulty", neededEntry.difficulty);
        GUILayout.EndHorizontal();

        GUI.backgroundColor = bg;
        GUILayout.Label(" or ");

        return remove;
    }

}
