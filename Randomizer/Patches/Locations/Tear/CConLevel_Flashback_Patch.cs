using System;
using System.Collections.Generic;
using System.Text;
using AsmResolver.PE.DotNet.ReadyToRun;
using Constance;
using HarmonyLib;
using Leo;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.Handlers.State;
using RandomizerCore.Classes.Storage.Locations;
using UnityEngine;
using UnityEngine.Localization;

namespace Randomizer.Patches.Locations.Tear;

[HarmonyPatch(typeof(CConLevel_Flashback))]
public class CConLevel_Flashback_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CConLevel_Flashback.ExitFlashback))]
    private static bool ExitFlashback_Prefix()
    {
        if (!RandomState.Randomized) return true;
        if (!RandomState.IsRandomized(RandomizableItems.Tears)) return true;

        IConPlayerEntity playerOne = ConMonoBehaviour.SceneRegistry.PlayerOne;
        CConLevel_Flashback cconLevel_Flashback;
        if (!CConSceneRegistry.Instance.LevelSceneManager.TryGetLevel<CConLevel_Flashback>(playerOne.Level.Current, out cconLevel_Flashback))
        {
            return false;
        }
        if (cconLevel_Flashback.tearUnlock)
        {
            ALocation location = cconLevel_Flashback.GetComponent<LocationComponent>().Location;
            RandomState.TryGetItem(location);

            if (ConMonoBehaviour.SceneRegistry.Inventory.Catalog.TearCollectedCount == 3)
            {
                IConPlayerLevelController level = playerOne.Level;
                ConCheckPointId voidGlimpseStart = ConCheckPoints.VoidGlimpseStart;
                ConLevelId conLevelId = ConCheckPoints.VoidGlimpseStart.ExtractLevelId();
                Direction direction = null;
                Vector2 zero = Vector2.zero;
                ConTransitionFadeInConfig conTransitionFadeInConfig = cconLevel_Flashback.exitFadeInConfig;
                bool flag = false;
                bool flag2 = false;
                ConRespawnOrigin conRespawnOrigin = ConRespawnOrigin.Undefined;
                object obj = new ConCheckPoints.VoidGlimpseInfo(cconLevel_Flashback.exitToCheckPoint);
                level.SetPendingTransition(new ConTransitionCommand_Default(voidGlimpseStart, conLevelId, direction, zero, conTransitionFadeInConfig, flag, flag2, conRespawnOrigin, null, obj));
                return false;
            }
        }
        IConPlayerLevelController level2 = playerOne.Level;
        ConCheckPointId conCheckPointId = cconLevel_Flashback.exitToCheckPoint;
        Direction direction2 = null;
        bool flag3 = false;
        bool flag4 = false;
        ConRespawnOrigin conRespawnOrigin2 = ConRespawnOrigin.Undefined;
        ConTransitionFadeInConfig? conTransitionFadeInConfig2 = new ConTransitionFadeInConfig?(cconLevel_Flashback.exitFadeInConfig);
        level2.InitTransition(conCheckPointId, direction2, flag3, flag4, conRespawnOrigin2, null, conTransitionFadeInConfig2);

        return false;
    }
}
