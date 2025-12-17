using RandomizerCore.Classes.State;
using System;

namespace Randomizer.Classes.Random;

[Serializable]
public class Spoiler(RandomStateElement element)
{
    public readonly string source = element.source.GetFullName();
    public readonly string dest = element.dest == null ? "null" : element.dest.GetFullName();

    public readonly bool hasObtainedSource = element.hasObtainedSource;
    public readonly bool isRandomized = element.isRandomized;
}
