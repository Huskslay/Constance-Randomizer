using Constance;
using HarmonyLib;
using Randomizer.Classes.Random;
using Randomizer.Classes.Random.Generation;
using Randomizer.Classes.UI.Elements;
using RandomizerCore.Classes.Handlers.SaveDataOwners.Types;
using RandomizerCore.Classes.State;
using System.Collections;
using UnityEngine;

namespace Randomizer.Patches;

[HarmonyPatch(typeof(ConStateAbility_Player_Transition))]
public class ConStateAbility_Player_Transition_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ConStateAbility_Player_Transition.CompleteTransitionIn))]
    private static void CompleteTransitionIn_Postfix()
    {
        string level = ConMonoBehaviour.SceneRegistry.PlayerOne.Level.Current.StringValue;

        // Skip Intro
        if (level == "Prod_Flashback_IntroCutscene" && RandomLoader.randomizing && RandomLoader.skipIntro)
        {
            Plugin.StartCoroutine(Skip);
            return;
        }

        // Setup Rando Room
        if (!RandomState.Randomized) return;
        RegionLivePatcher.LoadedRegion();
    }

    private static IEnumerator Skip()
    {
        // Get references
        CConPlayerEntity player = Plugin.FindFirstObjectByType<CConPlayerEntity>();
        ConStateAbility_Player_Transition transitionAbility = player.SM.TransitionAbility;
        CConCheckPointManager checkPointManager = CConSceneRegistry.Instance.CheckPointManager as CConCheckPointManager;
        CConTransitionManager transitionManager = transitionAbility.TransitionManager;

        // Load level and set player to start
        yield return new WaitUntil(() => !transitionManager.IsRunning);
        yield return new WaitForSeconds(0.1f);
        yield return RegionsHandler.I.LoadLevel(RandomSearch.startCheckpointId, player);
        player.transform.position += new Vector3(50, 0, 0);
    }
}