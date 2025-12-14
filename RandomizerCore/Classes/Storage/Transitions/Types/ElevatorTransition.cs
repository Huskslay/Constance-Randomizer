using Constance;
using Leo;
using RandomizerCore.Classes.Handlers;
using RandomizerCore.Classes.Storage.Regions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.Classes.Storage.Transitions.Types;

[Serializable]
public class ElevatorTransition(Region region) : ATransition(region.GetFullName(), TransitionMethod.Elevator)
{
    public override string GetFullName() => $"{GetRegion().GetFullName()}-Elevator";
}
