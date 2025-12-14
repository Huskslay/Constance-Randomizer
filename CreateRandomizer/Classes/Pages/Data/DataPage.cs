using CheatMenu.Classes;
using Constance;
using CreateRandomizer.Classes.Pages.Locations;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Data;

public class DataPage : GUIPage
{
    public override string Name => "Data";

    private bool oneShot = false;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
    }


    public override void Open()
    {

    }


    public override void UpdateOpen()
    {
        Color bgColor = GUI.backgroundColor;

        if (RandomState.Randomized) GUILayout.Label("Cannot Generate Data When Randomized");
        else if (GUILayout.Button("Generate Random Data")) Scraper.Scrape();

        GUIElements.Line();

        bool newOneShot = GUIElements.BoolValue("One Shot", oneShot);
        if (newOneShot != oneShot) { 
            oneShot = newOneShot;
            ConDebugFlags.DebugOneShot(oneShot); 
        }

        GUI.backgroundColor = bgColor;
    }


    public override void Close()
    {
        
    }
}
