using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Requirements.Entries;

[Flags]
public enum ItemEntries : long
{
    None = 0,
    Dash = 1 << 0,
    Stab = 1 << 1,
    Pogo = 1 << 2,
    WallDive = 1 << 3,
    Slice = 1 << 4,
    DoubleJump = 1 << 5,
    BombClone = 1 << 6,
    MilkshakeInsp = 1 << 7,
    CloneTpInsp = 1 << 8,
    Cousin = 1 << 9,
    Tears = 1 << 10,
    All = ~0
}
