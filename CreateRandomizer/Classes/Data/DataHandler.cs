using FileHandler.Classes;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Localization.SmartFormat.Core.Output;

namespace CreateRandomizer.Classes.Data;

public static class DataHandler
{
    public static readonly string folderName = "Data";

    public static void Init()
    {
        //Create<bool, DataList<bool>>(
        //    ref elevatorAutoUnlockDatas, elevatorAutoUnlockDatasName, fileName => new(fileName));
    }
    private static void Create<T, T2>(ref T2 list, string fileName, Func<string, T2> constructor) where T2 : DataList<T>
    {
        list = FileSaveLoader.LoadClassFromJson<T2>(folderName, fileName);
        if (list == null)
        {
            list = constructor(fileName);
            FileSaveLoader.TrySaveClassToJson(list, folderName, list.fileName);
        }
    }

    //////////////////////////////////////////////////////////////////

    //public static bool GetElevatorAutoUnlockData(Region region)
    //{
    //    string name = region.name;
    //    bool success = TryGetList(elevatorAutoUnlockDatas, name, out bool output);
    //    return success && output;
    //}
    //public static void EditElevatorAutoUnlockData(Region region, bool autoUnlocked)
    //{
    //    string name = region.name;
    //    EditData(elevatorAutoUnlockDatas, name, autoUnlocked);
    //}

    //////////////////////////////////////////////////////////////////

    private static bool TryGetList<T, T2>(T2 list, string name, out T output) where T2 : DataList<T>
    {
        if (list.TryGetData(name, out Data<T> data))
        {
            output = data.dataValue;
            return true;
        }
        output = default;
        return false;
    }
    private static void EditData<T, T2>(T2 list, string name, T newValue) where T2 : DataList<T>
    {
        if (list.EditData(name, newValue))
            FileSaveLoader.TrySaveClassToJson(list, folderName, list.fileName);
    }
}
