using RandomizerCore.Classes.Storage.Regions;
using System;

namespace RandomizerCore.Classes.Storage.Transitions.Types;

[Serializable]
public class ElevatorTransition(Region region) : ATransition(region.GetFullName(), TransitionMethod.Elevator)
{
    public override string GetFullName() => $"{GetRegion().GetFullName()}-Elevator";
}
