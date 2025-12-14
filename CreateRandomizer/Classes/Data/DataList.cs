using System;
using System.Collections.Generic;
using System.Text;

namespace CreateRandomizer.Classes.Data;

[Serializable]
public class DataList<T>(string fileName)
{
    public string fileName = fileName;
    public List<Data<T>> dataList = [];

    public void AddIfNotIn(string name, T dataValue)
    {
        if (TryGetData(name, out _)) return;
        EditData(name, dataValue);
    }

    public bool TryGetData(string name, out Data<T> data)
    {
        data = dataList.Find(x => x.name == name);
        return data != null;
    }

    public virtual bool EditData(string name, T dataValue)
    {
        Plugin.Logger.LogMessage(name);
        if (!TryGetData(name, out Data<T> data))
        {
            dataList.Add(new(name, dataValue));
            return true;
        }
        if (data.dataValue.Equals(dataValue)) return false;
        data.dataValue = dataValue;
        return true;
    }
}
