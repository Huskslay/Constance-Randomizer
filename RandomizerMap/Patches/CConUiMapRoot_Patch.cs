using Constance;
using HarmonyLib;
using Randomizer.Classes.Random.Generation;
using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomizerMap.Patches;

[HarmonyPatch(typeof(CConUiMapRoot))]
public class CConUiMapRoot_Patch
{
    public readonly static List<CConUiMapIcon> icons = [];

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConUiMapRoot.RebuildSelectTargets))]
    private static void RebuildSelectTargets_Prefix(CConUiMapRoot __instance)
    {
        foreach (CConUiMapIcon icon in icons) Object.Destroy(icon.gameObject);
        icons.Clear();

        if (!RegionsHandler.I.TryGetFromName("V01", out Region startRegion))
        {
            Plugin.Logger.LogError("Could not find region to start from");
            return;
        }
        List<ALocation> reachableLocations = RandomSearch.GetReachableLocations(
            RandomState.Instance.FoundItems, RandomState.Instance.FoundEvents, RandomState.Instance.Cousins.Count, startRegion.transitions[0], (_) => false, out _
        );

        IConUiMapSelectTarget a = __instance.iconParent.GetComponentsInChildren<IConUiMapSelectTarget>(includeInactive: true).First(x =>
        {
            if (x.RectTransform.name != "Custom" && x.RectTransform.gameObject.TryGetComponent(out CConUiMapIcon icon))
            { return icon.icon == ConMapIcon.Shrine; }
            return false;
        });
        foreach (RandomStateElement element in RandomState.Instance.LocationMap.Values)
        {
            if (!element.isRandomized || element.hasObtainedSource) continue;
            MakeIcon(__instance, a, element.source.GetRegion().id, reachableLocations.Contains(element.source));
        }

        //IConUiMapSelectTarget b = __instance.iconParent.GetComponentsInChildren<IConUiMapSelectTarget>(includeInactive: true).First(x => {
        //    if (x.RectTransform.gameObject.TryGetComponent(out CConUiMapIcon icon))
        //    { return icon.icon == ConMapIcon.Elevator; }
        //    return false;
        //});
        //List<Region> regions = FilesHandler.LoadClasses<Region>("Regions");
        //foreach (Region region in regions)
        //{
        //    foreach (Transition transition in region.transitions)
        //    {
        //        CConUiMapIcon icon = MakeIcon(__instance, b, region.id.stringValue);
        //        icons.Add(icon, transition);
        //    }
        //}
    }

    private static CConUiMapIcon MakeIcon(CConUiMapRoot __instance, IConUiMapSelectTarget a, ConLevelId level, bool reachable)
    {
        CConUiMapIcon icon = UnityEngine.Object.Instantiate(a.RectTransform.gameObject).GetComponent<CConUiMapIcon>();
        IConUiMapSelectTarget target = icon.GetComponent<IConUiMapSelectTarget>();

        icon.level = __instance.id2Level.Dictionary[level];

        icon.transform.SetParent(icon.level.iconParent);
        icon.transform.localScale = a.RectTransform.localScale;
        icon.transform.localPosition = a.RectTransform.localPosition;
        target.RectTransform.sizeDelta /= 2f;

        if (!reachable) icon.image.color = Color.gray;
        icon.name = "Custom";
        icon.gameObject.SetActive(true);

        return icon;
    }
}
