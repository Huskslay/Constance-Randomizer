using CheatMenu.Classes;
using Constance;
using CreateRandomizer.Classes.Data;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Regions;

public class RegionSoloPage : SoloGUIPage
{
    public override string Name => Requirement == null ? "null" : Requirement.evnt.ToString();

    public EventRequirement Requirement { get; private set; }

    private int selectedTransition;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        windowRect.height = 600f;
        windowRect.width = 600f;
    }

    public void Open(EventRequirement requirement)
    {
        Requirement = requirement;
        Open();
    }
    public override void Open()
    {
        if (Requirement == null) return;
        selectedTransition = -1;
        base.Open();
    }

    public override void UpdateOpen()
    {
        base.UpdateOpen();
        if (Requirement == null) return;

        GUILayout.BeginHorizontal();
        GUILayout.Label(Requirement.evnt.ToString());

        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        Requirement.evnt = GUIElements.EnumValue("Event", Requirement.evnt, 3);

        PageHelpers.DrawTransitionRequirements(ref Requirement.requirements, ref selectedTransition, this);
    }



    public override void Close()
    {
        Requirement = null;
        base.Close();
    }
}
