using CheatMenu.Classes;
using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Transitions;

public class TransitionSoloPage : SoloGUIPage
{
    public override string Name => ATransition == null ? "null" : ATransition.GetFullName();

    public ATransition ATransition { get; private set; }
    private TransitionSavedData savedData;

    private int selectedTransition;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        windowRect.height = 600f;
        windowRect.width = 600f;
    }

    public void Open(ATransition transition)
    {
        ATransition = transition;
        savedData = transition.GetSavedData();
        Open();
    }
    public override void Open()
    {
        if (ATransition == null) return;
        selectedTransition = -1;
        base.Open();
    }

    public override void UpdateOpen()
    {
        base.UpdateOpen();
        if (ATransition == null) return;

        savedData.completed = GUIElements.BoolValue("Completed", savedData.completed);

        if (ATransition.TransitionMethod == TransitionMethod.Elevator)
        {
            if (GUILayout.Button("Teleport")) StartCoroutine(PageHelpers.LoadTransition((ElevatorTransition)ATransition));

            GUIElements.Line();

            savedData.autoUnlock = GUIElements.BoolValue("Auto Unlock", savedData.autoUnlock);
        }
        else if (ATransition.TransitionMethod == TransitionMethod.Touch || ATransition.TransitionMethod == TransitionMethod.Prompt)
        {
            Transition transitionTransition = (Transition)ATransition;

            GUILayout.Space(10);
            savedData.doOverrideTransition = GUIElements.BoolValue("Override transition", savedData.doOverrideTransition);
            if (savedData.doOverrideTransition)
            {
                GUILayout.BeginHorizontal();
                savedData.overrideTransition = GUIElements.StringValue("Override", savedData.overrideTransition);
                if (GUILayout.Button("To Nearest"))
                {
                    CConTeleportPoint[] tps = FindObjectsByType<CConTeleportPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    CConTeleportPoint tp = null;
                    CConPlayerEntity player = FindFirstObjectByType<CConPlayerEntity>();
                    float closest = 10f;
                    foreach (CConTeleportPoint test in tps)
                    {
                        float dist = Vector3.Distance(player.transform.position, test.transform.position);
                        if (dist < closest)
                        {
                            closest = dist;
                            tp = test;
                        }
                    }
                    if (tp == null) Plugin.Logger.LogWarning("No nearby tps");
                    else savedData.overrideTransition = player.Level.Current.StringValue.Replace("Prod_", "") + "-" + Transition.ConvertName(tp);
                }
                GUILayout.EndHorizontal();
            }

            if (transitionTransition.GetLinkedTransition() != null)
            {
                if (GUILayout.Button((savedData.doOverrideTransition ? "Jank " : "") + "Teleport")) StartCoroutine(PageHelpers.LoadTransition(transitionTransition));
                if (GUILayout.Button("Teleport other side")) StartCoroutine(PageHelpers.LoadTransition(transitionTransition.GetLinkedTransition()));
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Transition Link is null");
                if (GUILayout.Button("Load Region")) StartCoroutine(RegionHandler.LoadLevel(ATransition.GetRegion()));
                GUILayout.EndHorizontal();
            }

            GUIElements.Line();

            savedData.lockType = GUIElements.EnumValue("Lock type", savedData.lockType);
        }

        GUIElements.Line();

        PageHelpers.DrawTransitionRequirements(ref savedData.neededRequirements.requirements, ref selectedTransition, this);

        GUIElements.Line();
        if (ATransition.TransitionMethod == TransitionMethod.Elevator)
        {
            if (GUILayout.Button("Teleport")) StartCoroutine(PageHelpers.LoadTransition((ElevatorTransition)ATransition));
        }
        else if (ATransition.TransitionMethod == TransitionMethod.Touch || ATransition.TransitionMethod == TransitionMethod.Prompt)
        {
            if (GUILayout.Button((savedData.doOverrideTransition ? "Jank " : "") + "Teleport")) StartCoroutine(PageHelpers.LoadTransition((Transition)ATransition));
        }

        RegionHandler.SaveSaveData(savedData, log: false);
    }



    public override void Close()
    {
        ATransition = null;
        savedData = null;
        base.Close();
    }
}
