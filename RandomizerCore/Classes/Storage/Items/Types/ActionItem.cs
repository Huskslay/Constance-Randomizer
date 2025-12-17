using Constance;
using RandomizerCore.Classes.Storage.Locations;
using System;
using System.Collections.Generic;

namespace RandomizerCore.Classes.Storage.Items.Types;

[Serializable]
public class ActionItem(ALocation location, string onCollect) : AItem(location.GetFullName())
{
    private readonly static Dictionary<string, Action> actionDictionary = [];

    private readonly string onCollect = onCollect;

    public override void GiveToPlayer(IConPlayerEntity player, IConPlayerInventory inventoryManager)
    {
        Plugin.Logger.LogMessage($"Calling action for item {GetFullName()}");
        actionDictionary[onCollect]?.Invoke();
    }

    public static void Reset()
    {
        actionDictionary.Clear();
    }
    public static void AddAction(string name, Action action)
    {
        actionDictionary.Add(name, action);
    }
}
