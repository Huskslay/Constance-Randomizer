using Constance;
using FileHandler.Classes;
using System.Collections.Generic;

namespace ShrineWarp.Classes;

public static class ShrineDataHandler
{
    private static readonly string folder = "Mods";
    private static readonly string file = "Shrine Warp";

    public static List<ShrineData> loadedData = null;


    public static void Init()
    {
        GameDataActions.OnFileSave.AddListener(SaveShrineData);
        GameDataActions.OnFileDelete.AddListener(DeleteShrineData);
    }

    public static void UpdateShrineData(string region, bool unlocked = true)
    {
        LoadShrineData();
        ShrineData data = loadedData.Find(x => x.region == region);
        if (data == null)
        {
            Plugin.Logger.LogError($"Could not find region '{region}' in data");
            return;
        }
        data.unlocked = unlocked;
    }



    public static void SaveShrineData(ConSaver conSaver, ConSaveStateId id)
    {
        if (loadedData == null) NewShrineData();
        FileSaveLoader.TrySaveClassToJson(loadedData, folder, file, id);
        loadedData = null;
    }
    public static void LoadShrineData()
    {
        if (loadedData != null) return;
        loadedData = FileSaveLoader.LoadClassFromJson<List<ShrineData>>(folder, file, CConSaveStateManager.LoadedSaveStateId);
        if (loadedData == null) NewShrineData();
    }
    private static void NewShrineData()
    {
        Plugin.Logger.LogWarning("No shrine warp data found, creating new");
        loadedData = [];
        foreach (string region in Plugin.regions)
        {
            string[] split = region.Split(':');
            loadedData.Add(new(split[0], split[1], false));
        }
        loadedData[0].unlocked = true;
    }
    public static void DeleteShrineData(ConSaver conSaver, ConSaveStateId id)
    {
        if (FileSaveLoader.ClassExistsInJson(folder, file, id: id))
            FileSaveLoader.DeleteClassInJson(folder, file, id: id);
    }

}
