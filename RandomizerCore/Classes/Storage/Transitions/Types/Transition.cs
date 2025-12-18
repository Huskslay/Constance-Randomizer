using Constance;
using FileHandler.Classes;
using Leo;
using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.Storage.Regions;
using System;

namespace RandomizerCore.Classes.Storage.Transitions.Types;

[Serializable]
public class Transition(CConTeleportPoint teleportPoint, Region region) : ATransition(region.GetFullName(), (TransitionMethod)(int)teleportPoint.teleporterType)
{
    public readonly string name = ConvertName(teleportPoint);
    public readonly string goName = teleportPoint.name;
    public override string GetFullName() => $"{GetRegion().GetFullName()}-{name}";


    public readonly DirectionEnum enterDirection = teleportPoint.enterDirection;
    public TransitionLockType lockType = TransitionLockType.NotLocked;


    public readonly string teleportToCheckPoint = teleportPoint.teleportTo.StringValue;
    public string linkedTransition = "";
    public Transition GetLinkedTransition()
    {
        string output = GetSavedData().doOverrideTransition ? GetSavedData().overrideTransition : linkedTransition;
        if (!RegionsHandler.I.TryGetTransitionFromName(output, out Transition linked)) return null;
        return linked;
    }

    public static string ConvertName(CConTeleportPoint teleportPoint)
    {
        string level = teleportPoint.teleportTo.ExtractLevelId().StringValue.Replace("Prod_", "");
        string hash = FileSaveLoader.FourDigitHash(teleportPoint.teleportTo.stringValue);
        return $"Transition-{level}-{hash}";
    }
}
