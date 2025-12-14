using Constance;
using Randomizer.Classes.UI.Elements;
using Randomizer.Patches.Menu;
using UnityEngine.EventSystems;

namespace Randomizer.Classes.UI.Menus;

public class RandoMainMenu : AConStartMenuPanel, IConSelectionLayer, ITransformProvider
{
    public override IConSelectionLayer SelectionLayer => this;

    public override bool AllowCancel => true;


    public void Init()
    {
        CConStartMenu_Patch.CreateButton("Items", transform, Items);
        CConStartMenu_Patch.CreateButton("Skips", transform, Skips);
        CConStartMenu_Patch.CreateButton("Settings", transform, Settings);
        CConStartMenu_Patch.CreateBlock(50, 50, transform);
        CConStartMenu_Patch.CreateButton("> Go! <", transform, Go);
        CConStartMenu_Patch.CreateBlock(50, 50, transform);
        CConStartMenu_Patch.CreateButton("<- Back <-", transform, Back);
    }



    public bool TryGetNewSelection(out ISelectHandler selectable)
    {
        return ConUiUtils.FindFirstSelectable(gameObject, out selectable);
    }

    private void Items(RandoButton button)
    {
        CConStartMenu_Patch.SwitchMenu(RandomLoader.RandoItemTypesMenu, this);
    }
    private void Skips(RandoButton button)
    {
        CConStartMenu_Patch.SwitchMenu(RandomLoader.RandoSkipEntriesMenu, this);
    }
    private void Settings(RandoButton button)
    {
        CConStartMenu_Patch.SwitchMenu(RandomLoader.RandoSettingsMenu, this);
    }
    private void Go(RandoButton button)
    {
        RandomLoader.NewSave();
    }
    private void Back(RandoButton button)
    {
        CConStartMenu_Patch.SwitchMenu(RandomLoader.RandoSelectMenu, this);
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
