using Constance;
using Randomizer.Classes.UI.Elements;
using Randomizer.Patches.Menu;
using RandomizerCore.Classes.State;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Randomizer.Classes.UI.Menus;

public class RandoItemsMenu : AConStartMenuPanel, IConSelectionLayer, ITransformProvider
{
    public override IConSelectionLayer SelectionLayer => this;

    public override bool AllowCancel => true;


    private static readonly Dictionary<RandomizableItems, string> normalizeName = new()
    {
        { RandomizableItems.LightStones, "Light Stones" },
        { RandomizableItems.CurrencyFlowers, "Currency Flowers" },
        { RandomizableItems.ShopItems, "Shop Items" },
        { RandomizableItems.DropBehaviours, "Collectables" }
    };


    public bool TryGetNewSelection(out ISelectHandler selectable)
    {
        return ConUiUtils.FindFirstSelectable(gameObject, out selectable);
    }

    public void Init()
    {
        float widthPer = 1400f, heightPer = 100f; int countW = 2, countH = 6;
        transform.position += Vector3.right * 3f;

        RectTransform rect = CConStartMenu_Patch.CreateGrid(widthPer, heightPer, countW, countH, transform);

        foreach (RandomizableItems entry in Enum.GetValues(typeof(RandomizableItems)))
        {
            if (entry == RandomizableItems.None || entry == RandomizableItems.All) continue;
            CConStartMenu_Patch.CreateButton(ButtonName(entry),
                rect.transform, (button) => Trigger(button, entry));
        }
        CConStartMenu_Patch.CreateBlock(50, 50, transform);
        CConStartMenu_Patch.CreateButton("<- Back <-", transform, Back);
    }


    private string ButtonName(RandomizableItems entry)
    {
        string name = normalizeName.ContainsKey(entry) ? normalizeName[entry] : entry.ToString();
        return $"{name}: {RandomLoader.chosenRandomizableItems.HasFlag(entry)}";
    }


    private void Trigger(RandoButton button, RandomizableItems entry)
    {
        RandomLoader.chosenRandomizableItems ^= entry;
        button.text = ButtonName(entry);
    }
    private void Back(RandoButton button)
    {
        CConStartMenu_Patch.SwitchMenu(RandomLoader.RandoMainMenu, this);
    }



    public override bool OpenPanel(IConPlayerEntity player, Leo.Void parameters)
    {
        gameObject.SetActive(true);
        return base.OpenPanel(player, parameters);
    }
    public override bool ClosePanel()
    {
        gameObject.SetActive(false);
        return base.ClosePanel();
    }
}
