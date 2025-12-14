using Constance;
using Randomizer.Classes.UI.Elements;
using Randomizer.Patches.Menu;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Randomizer.Classes.UI.Menus;

public class RandoSkipEntriesMenu : AConStartMenuPanel, IConSelectionLayer, ITransformProvider
{
    public override IConSelectionLayer SelectionLayer => this;

    public override bool AllowCancel => true;

    private static readonly Dictionary<SkipEntries, string> normalizeName = new()
    {
        { SkipEntries.BlombCloneTp, "Clone Teleport" },
        { SkipEntries.BlombCloneMidairPogo, "Pogo Midair Clone" },
        { SkipEntries.DarkRooms, "Traverse Dark Rooms" }
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

        foreach (SkipEntries entry in Enum.GetValues(typeof(SkipEntries)))
        {
            if (entry == SkipEntries.None || entry == SkipEntries.All) continue;
            CConStartMenu_Patch.CreateButton(ButtonName(entry),
                rect.transform, (button) => Trigger(button, entry));
        }
        CConStartMenu_Patch.CreateBlock(50, 50, transform);
        CConStartMenu_Patch.CreateButton("<- Back <-", transform, Back);
    }


    private string ButtonName(SkipEntries entry)
    {
        string name = normalizeName.ContainsKey(entry) ? normalizeName[entry] : entry.ToString();
        return $"{name}: {RandomLoader.chosenSkipEntries.HasFlag(entry)}";
    }


    private void Trigger(RandoButton button, SkipEntries entry)
    {
        RandomLoader.chosenSkipEntries ^= entry;
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
