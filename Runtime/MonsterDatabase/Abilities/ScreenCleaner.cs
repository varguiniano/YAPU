using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability ScreenCleaner.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ScreenCleaner", fileName = "ScreenCleaner")]
    public class ScreenCleaner : Ability
    {
        /// <summary>
        /// Side statuses cleared on both sides by this ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<SideStatus> ClearedSideStatusesOnBothSides;

        /// <summary>
        /// Trigger on ETB.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            BattlerType userType = battleManager.Battlers.GetTypeAndIndexOfBattler(battler).Type;
            BattlerType opponentsType = userType == BattlerType.Ally ? BattlerType.Enemy : BattlerType.Ally;

            Queue<SideStatus> toRemove = new();

            foreach (KeyValuePair<SideStatus, int> statusSlot in battleManager.Statuses.GetSideStatuses(opponentsType)
                                                                              .Where(statusSlot =>
                                                                                   ClearedSideStatusesOnBothSides
                                                                                      .Contains(statusSlot
                                                                                          .Key)))
                toRemove.Enqueue(statusSlot.Key);

            if (toRemove.Count > 0) ShowAbilityNotification(battler);

            while (toRemove.TryDequeue(out SideStatus status))
                yield return battleManager.Statuses.RemoveStatus(status, opponentsType);

            foreach (KeyValuePair<SideStatus, int> statusSlot in battleManager.Statuses.GetSideStatuses(userType)
                                                                              .Where(statusSlot =>
                                                                                   ClearedSideStatusesOnBothSides
                                                                                      .Contains(statusSlot
                                                                                          .Key)))
                toRemove.Enqueue(statusSlot.Key);

            if (toRemove.Count > 0) ShowAbilityNotification(battler);

            while (toRemove.TryDequeue(out SideStatus status))
                yield return battleManager.Statuses.RemoveStatus(status, userType);
        }
    }
}