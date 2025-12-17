using CheatMenu.Classes;
using Constance;
using System.Collections;
using UnityEngine;

namespace ShrineWarp.Classes.Pages;

public class OptionsPage : GUIPage
{
    public override string Name => "Options";

    public static bool unlockAll = false;

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
        GUI.backgroundColor = unlockAll ? Color.green : Color.red;
        if (GUILayout.Button("Toggle Unlock All")) unlockAll = !unlockAll;
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