using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the Imposter ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Imposter", fileName = "Imposter")]
    public class Imposter : Ability
    {
        /// <summary>
        /// Reference to the transformed status.
        /// </summary>
        [SerializeField]
        private Transformed TransformedStatus;

        /// <summary>
        /// Called when the monster is sent into battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster that entered the battle.</param>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            (BattlerType userType, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);
            BattlerType targetType = userType == BattlerType.Ally ? BattlerType.Enemy : BattlerType.Ally;

            if (!battleManager.Battlers.IsBattlerFighting(targetType, index)
             || !battler.CanTransform(battleManager)
             || battleManager.Statuses.HasStatus(TransformedStatus, targetType, index)
             || battleManager.Battlers.GetBattlerFromBattleIndex(targetType, index).Substitute.SubstituteEnabled)
                yield break;

            ShowAbilityNotification(battler);

            yield return battleManager.Battlers.TransformIntoTarget(userType,
                                                                    index,
                                                                    targetType,
                                                                    index,
                                                                    TransformedStatus);
        }
    }
}