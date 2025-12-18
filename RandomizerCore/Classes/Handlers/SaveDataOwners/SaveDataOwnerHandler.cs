using FileHandler.Classes;
using Leo;
using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.Storage.Saved;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Handlers.SaveDataOwners;

public abstract class SaveDataOwnerHandler<T, T2> where T : ISavedDataOwner<T2> where T2 : SavedData
{
    protected static readonly List<string> NameFolder = ["Names"];
    private static readonly string savedDataFolderName = "Saved Data";

    private List<string> SavedDataFolder => [savedDataFolderName, GetName()];
    protected abstract string GetName();


    protected List<T> dataOwners = null;
    protected List<T2> savedDatas = null;

    public static void InitAll()
    {
        new RegionsHandler().Init();
        new LocationsHandler().Init();
        new TransitionsHandler().Init();
    }

    public virtual void Init()
    {
        LoadDatas();
        LoadSavedData();
    }
    private void LoadDatas()
    {
        dataOwners = [];
        HashSet<string> names = [];

        LoadDatas((dataOwner) => OwnerIntiationAction(ref names, dataOwner));

        Plugin.Logger.LogMessage($"{dataOwners.Count} {GetName()} found");
        FileSaveLoader.TrySaveClassToJson(names, NameFolder, GetName(), logSuccess: false);
    }
    private void OwnerIntiationAction(ref HashSet<string> names, T dataOwner)
    {
        if (dataOwner == null) Plugin.Logger.LogError($"Null {GetName()} found");
        else
        {
            if (names.Contains(dataOwner.GetFullName()))
                Plugin.Logger.LogError($"{GetName()} name '{dataOwner.GetFullName()}' is not unique");
            else names.Add(dataOwner.GetFullName());

            dataOwner.Init();
            dataOwners.Add(dataOwner);
        }
    }
    protected abstract void LoadDatas(Action<T> initiate);


    private void LoadSavedData()
    {
        savedDatas = FileSaveLoader.LoadClassesJson<T2>(SavedDataFolder);

        HashSet<string> names = [];
        int foundOwnerCount = 0;
        foreach (T2 savedData in savedDatas)
        {
            if (savedData == null) Plugin.Logger.LogError($"Null saved data found");
            else
            {
                if (names.Contains(savedData.GetConnection()))
                    Plugin.Logger.LogError($"Connection '{savedData.GetConnection()}' is not unique");
                names.Add(savedData.GetConnection());

                if (!TryGetFromName(savedData.GetConnection(), out T owner))
                {
                    Plugin.Logger.LogError($"{savedData.GetConnection()} saved data can not find connection ");
                    continue;
                }
                else
                {
                    owner.SetSavedData(savedData);
                    foundOwnerCount++;
                }
            }
        }

        Plugin.Logger.LogMessage($"{savedDatas.Count} saved datas found");
        if (foundOwnerCount < dataOwners.Count) Plugin.Logger.LogError($"{dataOwners.Count - foundOwnerCount} owners do not contain saved data");
    }
    public void SaveSaveData(T2 data, bool log)
    {
        FileSaveLoader.TrySaveClassToJson(data, SavedDataFolder, data.GetConnection(), logSuccess: log);
    }

    public bool TryGetFromName(string name, out T found)
    {
        found = default;
        if (IsEmpty()) return false;

        return dataOwners.TryGetFirst(x => x.GetFullName() == name, out found);
    }
    public List<T> GetAll()
    {
        return dataOwners;
    }
    public T2 GetSaveData(string name)
    {
        return savedDatas.Find(x => x.GetConnection() == name);
    }

    public bool IsEmpty()
    {
        return dataOwners == null || dataOwners.Count == 0;
    }
}

