using System;

namespace ShrineWarp.Classes;

[Serializable]
public class ShrineData(string region, string checkpoint, bool unlocked)
{
    public readonly string region = region;
    public readonly string checkpoint = checkpoint;
    public bool unlocked = unlocked;
}
