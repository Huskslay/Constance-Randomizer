using RandomizerCore.Classes.Storage.Requirements.IRequirements;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Requirements;

[Serializable]
public class RequirementsOwner<T>(string connection) where T : IRequirement
{
    public string connection = connection;
    public List<T> requirements = [];

    public string GetConnection() => connection;
}