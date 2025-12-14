using System;
using System.Collections.Generic;
using System.Text;

namespace CreateRandomizer.Classes.Data;

[Serializable]
public class Data<T>(string name, T dataValue)
{
    public string name = name;
    public T dataValue = dataValue;
}
