using Btf.SaveData;
using Constance;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Saved;
using System;

namespace RandomizerCore.Classes.Storage.Items;

[Serializable]
public abstract class AItem(string locationName)
{
    public readonly string locationName = locationName;

    public string GetFullName() => $"Item-{locationName}";
    public abstract void GiveToPlayer(IConPlayerEntity player, IConPlayerInventory inventoryManager);
}
