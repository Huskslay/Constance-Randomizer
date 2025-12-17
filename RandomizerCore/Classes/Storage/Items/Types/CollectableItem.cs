using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Locations;
using System;

namespace RandomizerCore.Classes.Storage.Items.Types;

[Serializable]
public class CollectableItem(ALocation location, SConCollectable collectable, int amount = 1) : AItem(location.GetFullName())
{
    public readonly string collectable = collectable.name;
    public readonly int amount = amount;

    public override void GiveToPlayer(IConPlayerEntity player, IConPlayerInventory inventoryManager)
    {
        Plugin.Logger.LogMessage($"Giving {amount} {collectable}");
        foreach (string name in CollectableHandler.dict.Keys) Plugin.Logger.LogMessage(name);
        inventoryManager.Collect(player, CollectableHandler.dict[collectable], amount);
    }
}
