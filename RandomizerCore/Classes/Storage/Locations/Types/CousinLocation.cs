using Constance;
using Newtonsoft.Json;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Items.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using static Btf.Log.Logger;

namespace RandomizerCore.Classes.Storage.Locations.Types;

[Serializable]
public class CousinLocation : ALocation
{
    public override RandomizableItems GetItemType() => RandomizableItems.Cousins;
    public static FindObjectsInactive FindInactive => FindObjectsInactive.Include;


    protected override string GetDisplayItemNameInner() => "Cousin";
    private readonly AItem item;
    public override AItem GetItem() => item;


    private readonly ConLevelId id; 
    private readonly SConQuest.ConQuestIntel questIntel; 


    public CousinLocation(CConBehaviour_LostShopKeeper cousin, Region region) : base(ConvertName(), cousin.name, region)
    {
        id = cousin.GetLevelId();
        questIntel = cousin.config.shopKeeperData.ToList().Find(x => x.levelId == id).questIntel;
        item = new ActionItem(this, GetFullName());
    }
    private static string ConvertName()
    {
        return "Cousin";
    }


    public override void Init()
    {
        ActionItem.AddAction(GetFullName(), OnCollect);
    }
    private void OnCollect()
    {
        RandomState.AddFoundCousin(id);

        ConPersistenceId conPersistenceId = SConConfig_QuestFindShopKeepers.BuildKeeperFoundPersistenceId(id);
        ConScriptableObject.SceneRegistry.Save.SetBool(conPersistenceId, true, default);
        if (!ConMonoBehaviour.SceneRegistry.QuestManager.TryGetQuest(new ConQuestId("quest_ShopKeeper"), out SConQuest quest))
        {
            Plugin.Logger.LogWarning("Could not get shopkeeper quest!");
            return;
        }
        quest.AddIntel(questIntel);
    }


    public static void PatchLoadedLevel(CConBehaviour_LostShopKeeper cousin, CousinLocation cousinLocation)
    {
        if (!RandomState.IsRandomized(RandomizableItems.Cousins) || cousin == null) return;
        BasicPatch([cousin], [cousinLocation], (cousin, location) =>
        {
            // Hide the inspiration if it has been collected already
            if (!RandomState.TryGetElement(location, out RandomStateElement element))
            {
                Plugin.Logger.LogWarning($"Could not find a random element for location: {location.GetFullName()}");
                return;
            }
            cousin.gameObject.SetActive(!element.hasObtainedSource);
        });
    }
}