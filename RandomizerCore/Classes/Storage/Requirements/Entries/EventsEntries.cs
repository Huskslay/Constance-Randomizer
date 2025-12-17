using System;

namespace RandomizerCore.Classes.Storage.Requirements.Entries;

[Flags]
public enum EventsEntries
{
    None = 0,
    SquarePipe = 1 << 0,
    TrianglePipe = 1 << 1,
    AweKing = 1 << 2,
    HighPatia = 1 << 3,
    HasCamera = 1 << 4,
    C03Eye = 1 << 5,
    C04Eye = 1 << 6,
    C05Eye = 1 << 7,
    All = ~0
}
