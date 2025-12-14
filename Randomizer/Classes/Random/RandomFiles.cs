using Constance;
using FileHandler.Classes;
using Randomizer.Classes.Random.Generation;
using Randomizer.Classes.UI.Elements;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Handlers.State;
using System.Collections.Generic;

namespace Randomizer.Classes.Random;

public static class RandomFiles
{
    private static readonly string folder = "Mods";
    private static readonly string file = "Rando Data";
    private static readonly string spoiler = "Rando Spoiler";


    public static void Init()
    {
        GameDataActions.OnFileSave.AddListener(Save);
        GameDataActions.OnFileDelete.AddListener(Delete);
    }

    public static void Save(ConSaver conSaver, ConSaveStateId id)
    {
        Save(conSaver, (ConSaveStateId?)id);
    }
    public static void Save(ConSaver conSaver, ConSaveStateId? id = null)
    {
        if (!RandomState.Randomized) return;
        SerializeState current = SerializeState.Constructor(RandomState.Instance);
        FileSaveLoader.TrySaveClassToJson(current, folder, file, id == null ? CConSaveStateManager.LoadedSaveStateId : id, conSaver: conSaver);
    }
    public static SerializeState Load()
    {
        if (FileSaveLoader.ClassExistsInJson(folder, file, id: RandomLoader.chosenSlotId))
        {
            SerializeState current = FileSaveLoader.LoadClassFromJson<SerializeState>(folder, file, id: RandomLoader.chosenSlotId);
            if (current != null)
            {
                Plugin.Logger.LogMessage("Loaded randomizer");
                return current;
            }
            Plugin.Logger.LogMessage("An error occured reading the randomizer data");
        }
        Plugin.Logger.LogMessage("No randomizer data found, loadng vanilla");
        return null;
    }
    public static void Delete(ConSaver saver, ConSaveStateId id)
    {
        if (FileSaveLoader.ClassExistsInJson(folder, file, id: id))
            FileSaveLoader.DeleteClassInJson(folder, file, id: id);
    }


    public static void SaveSpoilerLog()
    {
        if (!RandomState.Randomized)
        {
            Plugin.Logger.LogWarning("Cannot create spoiler log when not randomized");
            return;
        }
        RandomGenerator generator = new();

        SerializeState current = SerializeState.Constructor(RandomState.Instance);

        generator.GenerateRandom(current.seed, current.includedItems, current.includedSkips, current.foundItems, current.foundEvents, out List<Spoiler> spoilerLog, current);
        if (!FileSaveLoader.TrySaveClassToJson(spoilerLog, folder, spoiler, CConSaveStateManager.LoadedSaveStateId, logSuccess: false))
        {
            Plugin.Logger.LogWarning("Error occured when saving the spoiler");
            return;
        }
        Plugin.Logger.LogMessage($"Saved spoiler log to save directory: ${FileSaveLoader.GetFilePath(
            CConSaveStateManager.LoadedSaveStateId, folder, spoiler, ".json")}");
    }
}
