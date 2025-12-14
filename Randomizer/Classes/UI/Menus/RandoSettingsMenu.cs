using Constance;
using Randomizer.Classes.UI.Elements;
using Randomizer.Patches.Menu;
using System;
using UnityEngine.EventSystems;

namespace Randomizer.Classes.UI.Menus;

public class RandoSettingsMenu : AConStartMenuPanel, IConSelectionLayer, ITransformProvider
{
    public override IConSelectionLayer SelectionLayer => this;
    public override bool AllowCancel => true;


    private static readonly System.Random rand = new();

    private RandoInputField seedInput;


    public bool TryGetNewSelection(out ISelectHandler selectable)
    {
        return ConUiUtils.FindFirstSelectable(gameObject, out selectable);
    }

    public void Init()
    {
        CConStartMenu_Patch.CreateButton($"Skip Into: {RandomLoader.skipIntro}", transform, ToggleSkipIntro);
        
        seedInput = new(transform, "Seed: ", "0", OnUpdateSeed);
        GenerateRandomSeed(null);
        CConStartMenu_Patch.CreateButton($"Generate Random Seed", transform, GenerateRandomSeed);

        CConStartMenu_Patch.CreateBlock(50, 50, transform);
        CConStartMenu_Patch.CreateButton("<- Back <-", transform, Back);
    }

    private void ToggleSkipIntro(RandoButton button)
    {
        RandomLoader.skipIntro = !RandomLoader.skipIntro;
        button.text = $"Skip Into: {RandomLoader.skipIntro}";
    }
    private void OnUpdateSeed(string newValue)
    {
        if (newValue == string.Empty) return;
        RandomLoader.chosenSeed = newValue;
    }
    private void GenerateRandomSeed(RandoButton _)
    {
        int randomStart = rand.Next(100000000);
        seedInput.SetInput(randomStart.ToString());
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
