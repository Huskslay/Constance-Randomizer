using CheatMenu.Classes;
using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Requirements.IRequirements.Types;
using RandomizerCore.Classes.Storage.Transitions.Types;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Regions;

public class RegionSelectSoloPage : SoloGUIPage
{
    public override string Name => Region == null ? "null" : Region.GetFullName();

    public Region Region { get; private set; }
    private RegionSavedData savedData;

    private RegionSoloPage soloPage;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);

        soloPage = new GameObject().AddComponent<RegionSoloPage>();
        soloPage.Init(modGUI, transform, ModGUI.winId++);
    }

    public void Open(Region region)
    {
        Region = region;
        savedData = Region.GetSavedData();
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

        savedData.completed = GUIElements.BoolValue("Completed", savedData.completed);
        if (GUILayout.Button("Teleport")) StartCoroutine(PageHelpers.LoadTransition(Region.transitions[0]));

        GUIElements.Line();

        GUILayout.Label("Givable events");
        Color bgColor = GUI.backgroundColor;
        foreach (EventRequirement requirement in savedData.obtainableEvents.requirements)
        {
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = requirement != null && soloPage.Requirement == requirement ? Color.green : Color.red;
            if (GUILayout.Button(requirement.evnt.ToString())) soloPage.Open(requirement);
            GUI.backgroundColor = bgColor;
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                savedData.obtainableEvents.requirements.Remove(requirement);
                if (soloPage.Requirement == requirement) soloPage.Close();
                break;
            }
            GUILayout.EndHorizontal();
        }
        GUIElements.Line();
        if (GUILayout.Button("New event"))
        {
            EventRequirement newReq = new();
            foreach (Transition transition in Region.transitions)
            {
                newReq.requirements.Add(new());
                newReq.requirements[^1].transition = transition.GetFullName();
            }
            savedData.obtainableEvents.requirements.Add(newReq);
        }

        RegionsHandler.I.SaveSaveData(savedData, log: false);
    }



    public override void Close()
    {
        Region = null;
        savedData = null;
        soloPage.Close();
        base.Close();
    }
}
