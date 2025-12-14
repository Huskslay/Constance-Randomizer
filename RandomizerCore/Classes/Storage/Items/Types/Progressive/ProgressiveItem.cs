using Constance;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Items.Types.Progressive;

[Serializable]
public class ProgressiveItem(ALocation location, string instanceName) : AItem(location.GetFullName())
{
    public readonly string instanceName = instanceName;

    public override void GiveToPlayer(IConPlayerEntity player, IConPlayerInventory inventoryManager)
    {
        ProgressiveItemHandler.Claim(instanceName, player, inventoryManager);
    }
}
