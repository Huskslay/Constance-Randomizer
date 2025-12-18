using RandomizerCore.Classes.Storage.Regions;
using RandomizerCore.Classes.Storage.Transitions;
using RandomizerCore.Classes.Storage.Transitions.Types;
using System;

namespace RandomizerCore.Classes.Handlers.SaveDataOwners.Types;

public class TransitionsHandler : SaveDataOwnerHandler<ATransition, TransitionSavedData>
{
    public static TransitionsHandler I { get; private set; }
    public override void Init()
    {
        I = this;
        base.Init();
    }

    protected override string GetName() => "Transitions";


    protected override void LoadDatas(Action<ATransition> initiate)
    {
        foreach (Region region in RegionsHandler.I.GetAll())
        {
            foreach (ATransition transition in region.transitions)
                initiate(transition);
        }

        foreach (ElevatorTransition transition in RegionsHandler.I.GetElevatorTransitions())
            initiate(transition);
    }
}