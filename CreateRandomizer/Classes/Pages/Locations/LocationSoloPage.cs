using CheatMenu.Classes;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Locations;

public class LocationSoloPage : SoloGUIPage
{
    public override string Name => Location == null ? "null" : Location.GetFullName();

    public ALocation Location { get; private set; }
    private LocationSavedData savedData;
    private Region region;
    private Func<List<MonoBehaviour>> getMonos;

    private int selectedTransition;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        windowRect.height = 600f;
        windowRect.width = 600f;
    }

    public void Open(ALocation location, Region region, Func<List<MonoBehaviour>> getMonos)
    {
        Location = location;
        savedData = location.GetSavedData();
        this.region = region;
        this.getMonos = getMonos;
        Open();
    }
    public override void Open()
    {
        if (Location == null) return;
        selectedTransition = -1;
        base.Open();
    }

    public override void UpdateOpen()
    {
        base.UpdateOpen();
        if (Location == null) return;

        Location.GetSavedData().completed = GUIElements.BoolValue("Completed", Location.GetSavedData().completed);
        
        if (getMonos != null) if (GUILayout.Button("Teleport")) StartCoroutine(PageHelpers.LoadLocation(Location, getMonos));

        GUIElements.Line();

        Location.GetSavedData().used = GUIElements.BoolValue("Is used", Location.GetSavedData().used);

        GUILayout.Label("Given Items");
        GUILayout.BeginHorizontal();
        int index = 0;
        Color bgColor = GUI.backgroundColor;
        ItemEntries newItemEntries = Location.GetSavedData().givenItems;
        foreach (ItemEntries value in Enum.GetValues(typeof(ItemEntries)))
        {
            if (value == ItemEntries.None || value == ItemEntries.All) continue;
            if (index++ == 4)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                index = 0;
            }
            bool enabled = newItemEntries.HasFlag(value);
            GUI.backgroundColor = enabled ? Color.green : Color.red;
            if (GUILayout.Button(value.ToString()))
            {
                if (enabled) newItemEntries &= ~value;
                else newItemEntries |= value;
            }
        }
        if (newItemEntries != Location.GetSavedData().givenItems) Location.GetSavedData().givenItems = newItemEntries;
        GUILayout.EndHorizontal();

        GUILayout.Label("Given Events");
        GUILayout.BeginHorizontal();
        index = 0;
        EventsEntries newEventsEntries = Location.GetSavedData().givenEvents;
        foreach (EventsEntries value in Enum.GetValues(typeof(EventsEntries)))
        {
            if (value == EventsEntries.None || value == EventsEntries.All) continue;
            if (index++ == 5)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                index = 0;
            }
            bool enabled = newEventsEntries.HasFlag(value);
            GUI.backgroundColor = enabled ? Color.green : Color.red;
            if (GUILayout.Button(value.ToString()))
            {
                if (enabled) newEventsEntries &= ~value;
                else newEventsEntries |= value;
            }
        }
        if (newEventsEntries != Location.GetSavedData().givenEvents) Location.GetSavedData().givenEvents = newEventsEntries;
        GUI.backgroundColor = bgColor;

        GUILayout.EndHorizontal();

        GUIElements.Line();

        PageHelpers.DrawTransitionRequirements(ref Location.GetSavedData().neededRequirements.requirements, ref selectedTransition, this);

        GUIElements.Line();

        if (getMonos != null) if (GUILayout.Button("Teleport")) StartCoroutine(PageHelpers.LoadLocation(Location, getMonos));

        RegionHandler.SaveSaveData(Location.GetSavedData(), log: false);
    }

    public override void Close()
    {
        Location = null;
        region = null;
        base.Close();
    }
}
