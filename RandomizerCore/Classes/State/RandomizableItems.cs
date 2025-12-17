using System;

namespace RandomizerCore.Classes.State;

[Flags]
public enum RandomizableItems
{
    None = 0,
    LightStones = 1 << 0,
    CurrencyFlowers = 1 << 1,
    Chests = 1 << 2,
    Canvases = 1 << 3,
    Inspirations = 1 << 4,
    ShopItems = 1 << 5,
    DropBehaviours = 1 << 6,
    FoundryPipe = 1 << 7,
    Cousins = 1 << 8,
    Tears = 1 << 9,
    All = ~0
}