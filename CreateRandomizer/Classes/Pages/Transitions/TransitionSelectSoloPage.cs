using CheatMenu.Classes;
using Constance;
using CreateRandomizer.Classes.Data;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Transitions;

public class TransitionSelectSoloPage : SoloGUIPage
{
    public override string Name => Region == null ? "null" : Region.GetFullName();

    public Region Region { get; private set; }

    private TransitionSoloPage soloPage;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        soloPage = new GameObject().AddComponent<TransitionSoloPage>();
        soloPage.Init(modGUI, transform, ModGUI.winId++);
    }

    public void Open(Region region)
    {
        Region = region;
        Open();
    }
    public override void Open()
    {
        if (Region == null) return;
        base.Open();
    }


    public override void UpdateOpen()
    {
        base.UpdateOpen();
        if (Region == null) return;

        Transition transition = GUIElements.ListValue("Transition", null, Region.transitions, 
            (_, t2, _) => t2 ==  soloPage.ATransition, t => t.name, 1, setColor: NotSelectedColor, afterEachNewLine: (_, _, _) => GUILayout.Space(5));
        if (transition != null) soloPage.Open(transition);

        if (Region.elevator != null)
        {
            Color origColor = GUI.backgroundColor;
            Color? notSelected = NotSelectedColor(null, Region.elevator, 0);
            GUI.backgroundColor = soloPage.ATransition == Region.elevator ? Color.green : (notSelected == null ? origColor : (Color)notSelected);
            if (GUILayout.Button("Elevator")) soloPage.Open(Region.elevator);
            GUI.backgroundColor = origColor;
        }
    }
    public Color? NotSelectedColor(ATransition current, ATransition test, int index)
    {
        return test.GetSavedData().completed ? null : Color.red;
    }

    public override void Close()
    {
        Region = null;
        soloPage.Close();
        base.Close();
    }
}
