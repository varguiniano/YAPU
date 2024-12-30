using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// CuteCharm ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/CuteCharm", fileName = "CuteCharm")]
    public class CuteCharm : Ability
    {
        /// <summary>
        /// Status to set.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private VolatileStatus Status;

        /// <summary>
        /// Chance to set the status.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float Chance;

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="damageDealt"></param>
        /// <param name="previousHP"></param>
        /// <param name="wasCritical"></param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits">Expected hits of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator AfterHitByMove(DamageMove move,
                                                   float effectiveness,
                                                   Battler owner,
                                                   Battler user,
                                                   int damageDealt,
                                                   uint previousHP,
                                                   bool wasCritical,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   int hitNumber,
                                                   int expectedMoveHits,
                                                   BattleManager battleManager)
        {
            if (!owner.IsOppositeGenderOf(user)) yield break;

            if (!move.DoesMoveMakeContact(user, owner, battleManager, false)
             || !(battleManager.RandomProvider.Value01() <= Chance))
                yield break;

            (BattlerType ownerType, int ownerIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            ShowAbilityNotification(owner);

            yield return
                battleManager.Statuses.AddStatus(Status,
                                                 Status.CalculateRandomCountdown(battleManager,
                                                     ownerType,
                                                     ownerIndex,
                                                     userType,
                                                     userIndex),
                                                 user,
                                                 ownerType,
                                                 ownerIndex,
                                                 false,
                                                 battleManager
                                                    .Battlers.GetBattlerFromBattleIndex(ownerType, ownerIndex));
        }
    }
}