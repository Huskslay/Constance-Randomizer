using Constance;
using RandomizerCore.Classes.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RandomizerCore.Classes.Handlers;

public static class CollectableHandler
{
    public static List<SConCollectable> collectablesList;

    private static readonly List<string> collectableNames = [
        "currency",
        "healFeather",
        "healFeatherPotion",
        "manaPiece",
        "healthPiece",
        "potion",
        "polaroid",
        "lightStone",
        "brush",
        "slice",
        "wallDive",
        "dash",
        "doubleJump",
        "pogo",
        "stab",
        "bombClone",
        "camera",
        "mapIcons",
        "architectsBlueprint",
        "map",
        "persevere",
        "fridaMask"
    ];
    private static readonly List<string> goalNames = [
        "tearFoundry",
        "tearVaults",
        "tearAcademy",
        "tearCarnival",
        "fridaMask"
    ];

    private static readonly string inspirationsName = "inspirations";
    private static readonly string shopItemsName = "shopItems";

    public static readonly Dictionary<string, string> nameDict = [];
    public static readonly Dictionary<string, SConCollectable> dict = [];
    public static readonly List<SConCollectable_InspirationDrawing> inspirationCollectables = [];
    public static readonly List<SConCollectable> goalsList = [];
    private static bool initiated = false;

    public static SConCollectable NameToCollectable(string name)
    {
        if (!nameDict.ContainsKey(name)) return null;
        return dict[nameDict[name]];
    }

    public static void Init()
    {
        RandomState.onLoadRandoSave.AddListener(TrueInit);
    }

    public static void TrueInit()
    {
        if (initiated || ConMonoBehaviour.SceneRegistry == null || ConMonoBehaviour.SceneRegistry.Collectables == null) return;
        initiated = true;

        SConCatalog_Collectable collectables = ConMonoBehaviour.SceneRegistry.Collectables;
        FieldInfo[] fields = typeof(SConCatalog_Collectable).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        SConCollectable_InspirationDrawing[] inspirations = null;
        SConCollectable_ShopItem[] shopItems = null;
        collectablesList = [];

        foreach (FieldInfo field in fields)
        {
            bool goal = goalNames.Contains(field.Name);

            if (field.Name == inspirationsName)
                inspirations = (SConCollectable_InspirationDrawing[])field.GetValue(collectables);
            else if (field.Name == shopItemsName)
                shopItems = (SConCollectable_ShopItem[])field.GetValue(collectables);
            else if (collectableNames.Contains(field.Name) || goal)
            {
                SConCollectable collectable = (SConCollectable)field.GetValue(collectables);
                collectablesList.Add(collectable);
                dict.Add(collectable.name, collectable);
                nameDict.Add(field.Name, collectable.name);
                if (goal) goalsList.Add(collectable);
            }
        }
        Plugin.Logger.LogMessage($"Found {collectablesList.Count} general collectables");

        if (inspirations != null)
        {
            foreach (SConCollectable_InspirationDrawing inspirationDrawing in inspirations)
            {
                collectablesList.Add(inspirationDrawing);
                dict.Add(inspirationDrawing.name, inspirationDrawing);
                inspirationCollectables.Add(inspirationDrawing);
            }

            Plugin.Logger.LogMessage($"Found {inspirations.Length} inspirations");
        }

        if (shopItems != null)
        {
            foreach (SConCollectable_ShopItem shopItem in shopItems)
            {
                collectablesList.Add(shopItem);
                dict.Add(shopItem.name, shopItem);
                if (!dict.Keys.Contains(shopItem.item.name))
                    dict.Add(shopItem.item.name, shopItem.item);
            }

            Plugin.Logger.LogMessage($"Found {shopItems.Length} shop items");
        }

        Plugin.Logger.LogMessage($"Found {collectablesList.Count} total collectables");
    }
}
