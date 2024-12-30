using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability PerishBody.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/PerishBody", fileName = "PerishBody")]
    public class PerishBody : Ability
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
            if (!move.DoesMoveMakeContact(user, owner, battleManager, false)) yield break;
            (BattlerType ownerType, int ownerIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            ShowAbilityNotification(owner);

            yield return
                battleManager.Statuses.AddStatus(Status,
                                                 Status.CalculateRandomCountdown(battleManager,
                                                          ownerType,
                                                          ownerIndex,
                                                          ownerType,
                                                          ownerIndex),
                                                 owner,
                                                 ownerType,
                                                 ownerIndex,
                                                 false);

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
                                                 false);
        }
    }
}