using CheatMenu.Classes;
using CreateRandomizer.Classes.Data;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Locations;


public class LocationPage : GUIPage
{
    public override string Name => "Locations";

    private LocationSelectSoloPage soloPage;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        soloPage = new GameObject().AddComponent<LocationSelectSoloPage>();
        soloPage.Init(modGUI, transform, ModGUI.winId++);
    }


    public override void Open()
    {

    }


    public override void UpdateOpen()
    {
        if (RegionHandler.Regions == null || RegionHandler.Regions.Count == 0)
        {
            GUILayout.Label("No regions loaded");
            return;
        }

        Region region = GUIElements.ListValue("Regions", null, RegionHandler.Regions, 
            (_, t2, _) => t2 != null && t2 == soloPage.Region, t => t == null ? "null" : t.GetFullName(), 4, setColor: NotSelectedColor);
        if (region != null) soloPage.Open(region);
    }

    public static Color? NotSelectedColor(Region current, Region test, int index)
    {
        bool completed = true;
        if (test == null) return Color.red;
        foreach (ALocation location in test.GetAllLocationsIncludeUnused())
        {
            if (location.GetSavedData() == null) continue;
            if (!location.GetSavedData().completed) completed = false;
        }
        return completed ? null : Color.red;
    }


    public override void Close()
    {
        soloPage.Close();
    }
}
