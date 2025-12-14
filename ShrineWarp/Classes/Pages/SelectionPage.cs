using CheatMenu.Classes;
using Constance;
using ShrineWarp.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShrineWarp.Classes.Pages;

public class SelectionPage : GUIPage
{
    public override string Name => "Selection";

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
    }


    public override void Open()
    {
        
    }


    public override void UpdateOpen()
    {
        Color origColor = GUI.backgroundColor;
        foreach (ShrineData shrine in ShrineDataHandler.loadedData)
        {
            GUI.backgroundColor = OptionsPage.unlockAll ? origColor : (shrine.unlocked ? Color.green : Color.red);
            if (GUILayout.Button(shrine.region))
            {
                if (shrine.unlocked || OptionsPage.unlockAll)
                {
                    CConUiPanel_Journal_Patch.journal.ClosePanel();
                    StartCoroutine(LoadLevel(new(shrine.region), new(shrine.checkpoint)));
                }
            }
        }
        GUI.backgroundColor = origColor;
    }

    public static IEnumerator LoadLevel(ConLevelId levelId, ConCheckPointId id)
    {
        CConPlayerEntity player = Plugin.FindFirstObjectByType<CConPlayerEntity>();
        ConStateAbility_Player_Transition transitionAbility = player.SM.TransitionAbility;
        CConTransitionManager transitionManager = transitionAbility.TransitionManager;

        ConTransitionCommand_Default trans = new(
            id,
            levelId,
            null,
            Vector2.zero,
            new(0, Color.green, FadeType.Fade, new(0.25f, new()))
        );

        transitionAbility.StartTransitionIn(trans);
        float start = Time.time;
        yield return new WaitUntil(() => !transitionManager.IsRunning || Time.time - start > 10);
        if (transitionManager.IsRunning) transitionManager.AbortTransition();
    }


    public override void Close()
    {

    }
}
