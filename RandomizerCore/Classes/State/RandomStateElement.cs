using RandomizerCore.Classes.Storage.Locations;
using System;

namespace RandomizerCore.Classes.State;

[Serializable]
public class RandomStateElement(ALocation source, ALocation dest, bool hasObtainedSource, bool isRandomized = true)
{
    public readonly ALocation source = source;
    public ALocation dest = dest;

    public bool hasObtainedSource = hasObtainedSource;
    public readonly bool isRandomized = isRandomized;
}
