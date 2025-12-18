using CheatMenu.Classes;
using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.Storage.Regions;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Regions;


public class RegionPage : GUIPage
{
    public override string Name => "Regions";

    private RegionSelectSoloPage soloPage;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        soloPage = new GameObject().AddComponent<RegionSelectSoloPage>();
        soloPage.Init(modGUI, transform, ModGUI.winId++);
    }


    public override void Open()
    {

    }


    public override void UpdateOpen()
    {
        if (RegionsHandler.I.IsEmpty())
        {
            GUILayout.Label("No regions loaded");
            return;
        }

        Region region = GUIElements.ListValue("Regions", null, RegionsHandler.I.GetAll(),
            (_, t2, _) => t2 != null && t2 == soloPage.Region, t => t == null ? "null" : t.GetFullName(), 4, setColor: NotSelectedColor);
        if (region != null) soloPage.Open(region);
    }
    public static Color? NotSelectedColor(Region current, Region test, int index)
    {
        if (test == null || test.GetSavedData() == null) return Color.red;

        return test.GetSavedData().completed ? null : Color.red;
    }


    public override void Close()
    {
        soloPage.Close();
    }
}
