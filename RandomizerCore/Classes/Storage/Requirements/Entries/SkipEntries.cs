using System;

namespace RandomizerCore.Classes.Storage.Requirements.Entries;

[Flags]
public enum SkipEntries : long
{
    None = 0,
    BlombCloneTp = 1 << 1,
    BlombCloneMidairPogo = 1 << 2,
    DarkRooms = 1 << 3,
    EnemyPogo = 1 << 4,
    EnemySlice = 1 << 5,
    EnemyKnockback = 1 << 6,
    SpikeStand = 1 << 7,
    All = ~0
}
