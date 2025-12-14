using Constance;
using FileHandler.Classes;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Items.Types.Progressive;
using Sonity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Handlers;

public static class ProgressiveItemHandler
{
    public static readonly string folderName = "Progressive Item Instances";

    public static List<ProgressiveItemInstance> Instances { get; private set; }  = [];


    public static void Init()
    {
        Instances = FileSaveLoader.LoadClasses<ProgressiveItemInstance>(folderName);

        HashSet<string> names = [];
        foreach (ProgressiveItemInstance instance in Instances)
        {
            if (instance == null) Plugin.Logger.LogError("Null progressive item instance found");
            else
            {
                if (!instance.AllLoaded(out Tuple<int, int> counts)) Plugin.Logger.LogError($"{instance.name} does not have matching insides - {counts.Item2} / {counts.Item1}");

                if (names.Contains(instance.name))
                    Plugin.Logger.LogError($"Progressive item instance name '{instance.name}' is not unique");
                names.Add(instance.name);
            }
        }
        Plugin.Logger.LogMessage($"{Instances.Count} progressive item instances found");
        FileSaveLoader.TrySaveClassToJson(names, "Names", folderName, logSuccess: false);
    }


    public static void AddInstance(ProgressiveItemInstance instance)
    {
        Instances.Add(instance);
    }

    public static void Claim(string instanceName, IConPlayerEntity player, IConPlayerInventory inventoryManager)
    {
        ProgressiveItemInstance instance = Instances.Find(x => x.name == instanceName);
        instance.Claim(player, inventoryManager);
    }

    public static string GetItemName(string instanceName)
    {
        ProgressiveItemInstance instance = Instances.Find(x => x.name == instanceName);
        return instance.GetItemName();
    }
}