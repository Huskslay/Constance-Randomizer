using Constance;
using HarmonyLib;
using Leo;
using Randomizer.Classes.UI.Elements;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Randomizer.Patches.Menu;

[HarmonyPatch(typeof(CConStartMenu))]
public class CConStartMenu_Patch
{
    private static readonly List<GameObject> created = [];

    public static CConStartMenu StartMenu { get; private set; }
    public static CConStartMenu_SelectSave SaveMenu { get; private set; }


    private static IConUiPanelManager panelManager;
    private static RandoButton ButtonPrefab;
    private static Transform MenuPrefab;


    [HarmonyPostfix]
    [HarmonyPatch(nameof(CConStartMenu.Awake))]
    private static void Awake_Postfix(CConStartMenu __instance)
    {
        StartMenu = __instance;

        // Clear any created gis
        foreach (GameObject go in created) Plugin.DestroyImmediate(go);
        created.Clear();

        // Get object references
        panelManager = Plugin.FindFirstObjectByType<CConUiPanelManager>();
        if (panelManager == null) { Plugin.Logger.LogError("Error patching menu: A"); return; }

        SaveMenu = StartMenu.transform.GetChild(2).GetComponent<CConStartMenu_SelectSave>();
        if (SaveMenu.name != "Menu_Start") { Plugin.Logger.LogError("Error patching menu: B"); return; }

        Transform MainMenu = StartMenu.transform.GetChild(1);
        if (MainMenu.name != "Menu_Main") { Plugin.Logger.LogError("Error patching menu: C"); return; }

        // Create prefab menu
        MenuPrefab = Plugin.Instantiate(MainMenu, Plugin.Transform);
        Plugin.DestroyImmediate(MenuPrefab.gameObject.GetComponent<CConStartMenu_Main>());
        MenuPrefab.gameObject.SetActive(false);
        created.Add(MenuPrefab.gameObject);
        MenuPrefab.name = "PrefabRandoMenu";

        // Create prefab button
        ButtonPrefab = Plugin.Instantiate(MenuPrefab.GetChild(0), Plugin.Transform).gameObject.AddComponent<RandoButton>();
        ButtonPrefab.PreInit(ButtonPrefab.GetComponentInChildren<TextMeshProUGUI>(),
                             ButtonPrefab.GetComponent<Button>());
        created.Add(ButtonPrefab.gameObject);

        // Create menus
        MenuPrefab.DestroyAllChildrenImmediate();
        RandomLoader.CreateMenus();
    }

    public static T CreateMenu<T>(string name) where T : AConStartMenuPanel
    {
        T menu = Plugin.Instantiate(MenuPrefab, StartMenu.transform).gameObject.AddComponent<T>();
        menu.name = name;
        created.Add(menu.gameObject);
        return menu;
    }

    public static RectTransform CreateBlock(float width, float height, Transform parent)
    {
        RectTransform rect = new GameObject().AddComponent<RectTransform>();
        rect.sizeDelta = new(width, height);
        rect.SetParent(parent);
        rect.transform.localScale = Vector3.one;
        return rect;
    }
    public static RectTransform CreateGrid(float widthPer, float heightPer, int countW, int countH, Transform parent)
    {
        RectTransform rect = new GameObject().AddComponent<RectTransform>();

        GridLayoutGroup grid = rect.gameObject.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = countW;
        grid.cellSize = new(widthPer, heightPer);

        rect.sizeDelta = new(widthPer * countW, heightPer * countH);
        rect.SetParent(parent);
        rect.transform.localScale = Vector3.one;
        return rect;
    }
    public static RandoButton CreateButton(string name, Transform parent, Action<RandoButton> onClick, Action onUpdate = null)
    {
        RandoButton button = Plugin.Instantiate(ButtonPrefab, parent);
        button.Init(name, onClick, onUpdate);
        created.Add(button.gameObject);
        return button;
    }

    public static void SwitchMenu(AConStartMenuPanel open, AConStartMenuPanel close)
    {
        panelManager.OpenPanel(open, null, new());
        panelManager.ClosePanel(close);
    }
}
