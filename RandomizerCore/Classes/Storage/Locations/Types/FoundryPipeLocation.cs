using Constance;
using FileHandler.Classes;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class FoundryPipeLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.FoundryPipe;
    public static FindObjectsInactive FindInactive => FindObjectsInactive.Exclude;


    protected override string GetDisplayItemNameInner() => "Foundry Pipe";

    private readonly AItem item;
    public override AItem GetItem() => item;


    private readonly ConPersistenceId id;


    public FoundryPipeLocation(ConFoundryPaintPipe_Valve valve, Region region) : base(ConvertName(valve), valve.name, region)
    {
        id = valve.PersistenceId;
        item = new ActionItem(this, GetFullName());
    }
    private static string ConvertName(ConFoundryPaintPipe_Valve valve)
    {
        string hash = FileSaveLoader.FourDigitHash(valve.PersistenceId.StringValue);
        return $"FoundryPipe-{hash}";
    }

    public override void Init()
    {
        ActionItem.AddAction(GetFullName(), OnCollect);
    }

    private void OnCollect()
    {
        ConMonoBehaviour.SceneRegistry.Persistence.Save.SetBool(id, true, default);
        ConQuestManager questManager = ConMonoBehaviour.SceneRegistry.QuestManager;
        int fixedPipeCount = ConFoundryPaintPipe_Valve.GetFixedPipeCount();
        SConQuest.ConQuestIntel conQuestIntel;
        if (fixedPipeCount != 1)
        {
            if (fixedPipeCount != 2)
            {
                conQuestIntel = SConQuest.ConQuestIntel.None;
            }
            else
            {
                conQuestIntel = SConQuest.ConQuestIntel.C;
            }
        }
        else
        {
            conQuestIntel = SConQuest.ConQuestIntel.B;
        }
        questManager.AddIntel(ConQuests.Foundry, conQuestIntel, false);
    }


    public static void PatchLoadedLevel(List<ConFoundryPaintPipe_Valve> valves, List<FoundryPipeLocation> valveLocations)
    {
        if (!RandomState.IsRandomized(RandomizableItems.FoundryPipe)) return;
        BasicPatch(valves, valveLocations);
    }
}