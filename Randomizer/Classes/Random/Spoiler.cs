using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Randomizer.Classes.Random;

[Serializable]
public class Spoiler(RandomStateElement element)
{
    public readonly string source = element.source.GetFullName();
    public readonly string dest = element.dest == null ? "null" : element.dest.GetFullName();

    public readonly bool hasObtainedSource = element.hasObtainedSource;
    public readonly bool isRandomized = element.isRandomized;
}
