using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Requirements.Entries;

namespace Randomizer.Patches.Events.Eyes;

[HarmonyPatch(typeof(CConCarnivalHeadlightEye))]
public class CConCarnivalHeadlightEye_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConCarnivalHeadlightEye.Interact))]
    private static void Interact_Prefix()
    {
        if (!RandomState.Randomized) return;

        ConLevelId id = ConMonoBehaviour.SceneRegistry.PlayerOne.Level.Current;
        RandomState.AchieveEvents(LevelToEyeEntry(id.StringValue));
    }

    private static EventsEntries LevelToEyeEntry(string level)
    {
        return level switch
        {
            "Prod_C03" => EventsEntries.C03Eye,
            "Prod_C04" => EventsEntries.C04Eye,
            "Prod_C05" => EventsEntries.C05Eye,
            _ => EventsEntries.None,
        };
    }
}
