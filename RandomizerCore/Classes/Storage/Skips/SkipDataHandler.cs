using FileHandler.Classes;
using RandomizerCore.Classes.Storage.Requirements.Entries;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Skips;
public static class SkipDataHandler
{
    private static readonly string folder = "Skip Data";

    public static List<SkipData> skipDatas;

    public static void Init()
    {
        skipDatas = FileSaveLoader.LoadClassesJson<SkipData>(folder);
        foreach (SkipEntries skip in Enum.GetValues(typeof(SkipEntries)))
        {
            if (skip == SkipEntries.None || skip == SkipEntries.All) continue;

            SkipData data = skipDatas.Find(x => x.skip == skip);
            if (data == null)
            {
                Plugin.Logger.LogWarning($"Skip data does not exist for skip '{skip}', creating some");
                SkipData newData = new(skip);
                skipDatas.Add(newData);
                SaveSkipData(newData, log: true);
            }
        }
    }

    public static bool SkipIsPossible(SkipEntries neededSkips, ItemEntries foundItems)
    {
        foreach (SkipEntries skip in Enum.GetValues(typeof(SkipEntries)))
        {
            if (skip == SkipEntries.None || skip == SkipEntries.All) continue;
            if (!neededSkips.HasFlag(skip)) continue;

            SkipData data = skipDatas.Find(x => x.skip == skip);
            bool passed = data.neededItems.Count == 0;
            foreach (ItemEntries entry in data.neededItems)
            {
                if ((foundItems & entry) == entry)
                {
                    passed = true;
                    break;
                }
            }
            if (!passed) return false;
        }
        return true;
    }

    public static void SaveSkipData(SkipData data, bool log)
    {
        FileSaveLoader.TrySaveClassToJson(data, folder, data.skip.ToString(), logSuccess: log);
    }
}
