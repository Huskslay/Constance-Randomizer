using Constance;
using Randomizer.Classes.UI.Menus;
using Randomizer.Patches.Menu;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Requirements.Entries;

namespace Randomizer.Classes.UI.Elements;

public static class RandomLoader
{
    public static bool randomizing;
    private static int seed = -1;
    public static string chosenSeed;
    public static bool skipIntro = true;

    public static SkipEntries chosenSkipEntries = SkipEntries.All;
    public static RandomizableItems chosenRandomizableItems = RandomizableItems.All;

    public static ConSaveStateId chosenSlotId;


    public static RandoSelectMenu RandoSelectMenu;
    public static RandoMainMenu RandoMainMenu;
    public static RandoItemsMenu RandoItemTypesMenu;
    public static RandoSkipEntriesMenu RandoSkipEntriesMenu;
    public static RandoSettingsMenu RandoSettingsMenu;


    public static void NewSave()
    {
        if (!int.TryParse(chosenSeed, out seed))
        {
            Plugin.Logger.LogError($"Seed to int conversion failed on seed: {chosenSeed}");
            return;
        }
        LoadSave();
    }
    public static void LoadSave()
    {
        CConStartMenu_Patch.StartMenu.LoadSaveSlot(chosenSlotId);
    }
    public static void QuitSave()
    {
        randomizing = false;
        chosenSlotId = default;
        RandomState.UnRandomizeState();
    }


    public static void CreateMenus()
    {
        RandoSelectMenu = CConStartMenu_Patch.CreateMenu<RandoSelectMenu>("RandoSelectMenu");
        RandoSelectMenu.Init();

        RandoMainMenu = CConStartMenu_Patch.CreateMenu<RandoMainMenu>("RandoMainMenu");
        RandoMainMenu.Init();

        RandoItemTypesMenu = CConStartMenu_Patch.CreateMenu<RandoItemsMenu>("RandoItemsMenu");
        RandoItemTypesMenu.Init();

        RandoSkipEntriesMenu = CConStartMenu_Patch.CreateMenu<RandoSkipEntriesMenu>("RandoSkipEntriesMenu");
        RandoSkipEntriesMenu.Init();

        RandoSettingsMenu = CConStartMenu_Patch.CreateMenu<RandoSettingsMenu>("RandoSettingsMenu");
        RandoSettingsMenu.Init();
    }

    internal static void CreateRandomizer()
    {
        RandomState.RandomizeState(seed, chosenRandomizableItems, chosenSkipEntries);
    }
}