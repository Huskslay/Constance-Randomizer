using FileHandler.Classes;
using RandomizerCore.Classes.Storage.Items;
using RandomizerCore.Classes.Storage.Locations.Types.Progressive;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Handlers;

public static class ProgressiveItemHandler
{
    public static List<string> FolderName => ["Progressive Item Instances"];

    public static List<ProgressiveItemInstance> Instances { get; private set; } = [];


    public static void Init()
    {
        Instances = FileSaveLoader.LoadClassesJson<ProgressiveItemInstance>(FolderName);

        HashSet<string> names = [];
        foreach (ProgressiveItemInstance instance in Instances)
        {
            if (instance == null) Plugin.Logger.LogError("Null progressive item instance found");
            else
            {
                instance.Init();
                if (names.Contains(instance.GetType().ToString()))
                    Plugin.Logger.LogError($"Progressive item instance name '{instance.GetType()}' is not unique");
                names.Add(instance.GetType().ToString());
            }
        }
        Plugin.Logger.LogMessage($"{Instances.Count} progressive item instances found");
        FileSaveLoader.TrySaveClassToJson(names, ["Names"], FolderName[0], logSuccess: false);
    }
    public static void Save(ProgressiveItemInstance instance, bool log)
    {
        FileSaveLoader.TrySaveClassToJson(instance, FolderName, instance.GetProgressiveType().ToString(), logSuccess: log);
    }

    private static bool TryGetInstance(ProgressiveItemType type, out ProgressiveItemInstance instance)
    {
        instance = Instances.Find(x => x.GetProgressiveType() == type);
        if (instance == null)
        {
            Plugin.Logger.LogWarning($"Progressive Item Instance for type '{type}' does not exist, creating new");
            return false;
        }
        return true;
    }

    public static string GetItemName(IProgressiveLocation location)
    {
        if (TryGetInstance(location.GetProgressiveType(), out ProgressiveItemInstance instance)) return instance.GetItemName();
        return null;
    }

    public static void AddToInstance(IProgressiveLocation location)
    {
        ProgressiveItemType type = location.GetProgressiveType();
        if (!TryGetInstance(type, out ProgressiveItemInstance instance))
        {
            Plugin.Logger.LogWarning("Creating new");
            instance = new(type);
            Instances.Add(instance);
        }
        instance.AddItem(location);
        Save(instance, log: true);
    }

    internal static AItem GetItem(IProgressiveLocation location)
    {
        if (!TryGetInstance(location.GetProgressiveType(), out ProgressiveItemInstance instance)) return null;
        return instance.GetItem();
    }
}