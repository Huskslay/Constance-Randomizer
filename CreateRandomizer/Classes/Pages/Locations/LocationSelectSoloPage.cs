using CheatMenu.Classes;
using Constance;
using CreateRandomizer.Classes.Data;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Regions;
using System.Linq;
using UnityEngine;

namespace CreateRandomizer.Classes.Pages.Locations;

public class LocationSelectSoloPage : SoloGUIPage
{
    public override string Name => Region == null ? "null" : Region.GetFullName();

    public Region Region { get; private set; }

    private LocationSoloPage soloPage;

    public override void Init(ModGUI modGUI, Transform parent, int id = 1)
    {
        base.Init(modGUI, parent, id);
        soloPage = new GameObject().AddComponent<LocationSoloPage>();
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
        Color bg = GUI.backgroundColor;

        if (Region.lightStoneLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Light Stones", null, Region.lightStoneLocations,
                (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, () => FindObjectsByType<CConCurrencyDepositEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ConvertAll(x => (MonoBehaviour)x));
        }

        if (Region.currencyFlowerLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Currency Flowers", null, Region.currencyFlowerLocations,
            (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, () => FindObjectsByType<CConCurrencyDepositEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ConvertAll(x => (MonoBehaviour)x));
        }

        if (Region.chestLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Chests", null, Region.chestLocations,
            (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, () => FindObjectsByType<CConChestEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ConvertAll(x => (MonoBehaviour)x));
        }

        if (Region.canvasLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Canvases", null, Region.canvasLocations,
            (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, () => FindObjectsByType<CConUnlockAbilityCanvas>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ConvertAll(x => (MonoBehaviour)x));
        }

        if (Region.inspirationLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Inspirations", null, Region.inspirationLocations,
            (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, () => FindObjectsByType<CConInspirationTriggerBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ConvertAll(x => (MonoBehaviour)x));
        }

        if (Region.shopItemLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Shop Items", null, Region.shopItemLocations,
                (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, null);
        }

        if (Region.dropBehaviourLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Collectables", null, Region.dropBehaviourLocations,
                (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, () => FindObjectsByType<CConEntityDropBehaviour_TouchToCollect>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ConvertAll(x => (MonoBehaviour)x));
        }

        if (Region.foundryPipeLocations.Count > 0)
        {
            ALocation location = GUIElements.ListValue("Paint Foundries", null, Region.foundryPipeLocations,
                (_, t2, _) => t2 == soloPage.Location, t => t.name, 1, setColor: NotSelectedColor);
            if (location != null) soloPage.Open(location, Region, () => FindObjectsByType<ConFoundryPaintPipe_Valve>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList().ConvertAll(x => (MonoBehaviour)x));
        }

        if (Region.cousinLocation != null || Region.tearLocation != null)
        {
            GUILayout.Label("Others: ");

            if (Region.cousinLocation != null)
            {
                GUI.backgroundColor = soloPage.Location == Region.cousinLocation ? Color.green : (Region.cousinLocation.GetSavedData().completed ? bg : Color.red);
                if (GUILayout.Button("Cousin")) soloPage.Open(Region.cousinLocation, Region, () => [FindFirstObjectByType<CConBehaviour_LostShopKeeper>()]);
            }

            if (Region.tearLocation != null)
            {
                GUI.backgroundColor = soloPage.Location == Region.tearLocation ? Color.green : (Region.tearLocation.GetSavedData().completed ? bg : Color.red);
                if (GUILayout.Button("Tear")) soloPage.Open(Region.tearLocation, Region, null);
            }
        }

        GUI.backgroundColor = bg;
    }
    public Color? NotSelectedColor(ALocation current, ALocation test, int index)
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
