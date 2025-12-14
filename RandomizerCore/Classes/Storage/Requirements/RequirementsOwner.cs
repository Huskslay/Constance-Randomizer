using Newtonsoft.Json;
using RandomizerCore.Classes.Storage.Requirements.IRequirements;
using RandomizerCore.Classes.Storage.Saved;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Requirements;

[Serializable]
public class RequirementsOwner<T>(string connection) where T : IRequirement
{
    public string connection = connection;
    public List<T> requirements = [];

    public string GetConnection() => connection;
}