using Constance;
using HarmonyLib;
using RandomizerCore.Classes.Adapters;
using RandomizerCore.Classes.State;
using RandomizerCore.Classes.Storage.Locations;
using RandomizerCore.Classes.Storage.Locations.Types.Deposits;
using Sonity;
using UnityEngine;

namespace Randomizer.Patches.Locations.Deposit;

[HarmonyPatch(typeof(CConCurrencyDepositEntity))]
public class CConBehaviour_LostShopKeeper_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch("Constance.IConAttackable.HandleIncomingAttack")]
    private static bool HandleIncomingAttack_Prefix(CConCurrencyDepositEntity __instance, ref ConAttackResult __result, ConAttackRequest request)
    {
        if (!RandomState.Randomized) return true;
        RandomizableItems type = DepositLocationFactory.EntityToDepositType(__instance);
        if (!RandomState.IsRandomized(type)) return true;

        if (__instance._health == 0)
        {
            __result = ConAttackResult.Ignored;
            return false;
        }
        if (!request.IsPhysical())
        {
            __result = ConAttackResult.Ignored;
            return false;
        }

        __instance._health--;
        if (__instance._health == 0)
        {
            ALocation location = __instance.GetComponent<LocationComponent>().Location;
            RandomState.TryGetItem(location);
        }

        __instance._lastAttack = request;
        __instance.persistable.SetIntState(__instance._health);
        Vector2 vector = request.HitPos ?? __instance.Body.Center;

        SoundEvent[] array = __instance.hitSounds;
        if (array != null && array.Length > 0)
        {
            __instance.hitSounds[__instance._health % __instance.hitSounds.Length].PlayAtPosition(__instance.transform, vector);
        }
        __instance.UpdateState();

        //CConCurrencyDepositEntity.ConCurrencyDepositState conCurrencyDepositState = __instance.depositStates[__instance._health];
        //conCurrencyDepositState.LootBag.SpawnLoot(this, this._lastAttack);

        SConAttackCommandHandler_DamageDefault.ConAttackCommandHandler_DamageDefault.InitCreatureHurtFlash(__instance.Get<CConEntityRenderController>(), ConMonoBehaviour.SceneRegistry.GlobalCharacterData);
        __result = ConAttackResult.Hit;

        return false;
    }
}
