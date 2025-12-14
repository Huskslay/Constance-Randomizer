using System;
using System.Collections.Generic;
using System.Text;

namespace CreateRandomizer.Classes.Data;

public class StrictDataList<T>(string fileName, T defaultValue) : DataList<T>(fileName)
{
    public T defaultValue = defaultValue;

    public override bool EditData(string name, T dataValue)
    {
        if (!TryGetData(name, out Data<T> data))
        {
            if (!dataValue.Equals(defaultValue))
            {
                dataList.Add(new(name, dataValue));
                return true;
            }
            return false;
        }
        if (data.dataValue.Equals(dataValue)) return false;
        if (!dataValue.Equals(defaultValue))
        {
            dataList.Remove(data);
            return true;
        }
        data.dataValue = dataValue;
        return true;
    }
}
