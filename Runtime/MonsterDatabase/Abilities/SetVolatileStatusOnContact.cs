using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that set a status to the attacker when hit with a contact move.
    /// </summary>
    public abstract class SetVolatileStatusOnContact : Ability
    {
        /// <summary>
        /// Dictionary with the statuses that can be inflicted and the chance to inflict them.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializedDictionary<VolatileStatus, float> StatusChances;

        /// <summary>
        /// Types immune to this ability.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<MonsterType> ImmuneTypes;

        /// <summary>
        /// Items immune to this ability.
        /// </summary>
        [FoldoutGroup("Immunities")]
        [SerializeField]
        private List<Item> ImmuneItems;

        /// <summary>
        /// Should the status be forever?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected bool InfiniteDuration;

        /// <summary>
        /// Useful for moves that need a target but set on the user, like Mind Reader.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool SetStatusOnSelf;

        /// <summary>
        /// Affected by contact moves or any damaging?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool OnlyContactMoves = true;

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
            Battler target = SetStatusOnSelf ? owner : user;

            if ((OnlyContactMoves && !move.DoesMoveMakeContact(user, owner, battleManager, ignoresAbilities))
             || !AffectsUserOfEffect(target, owner, IgnoresOtherAbilities(battleManager, owner, null), battleManager))
                yield break;

            if (target.IsOfAnyType(ImmuneTypes, battleManager.YAPUSettings)) yield break;

            if (target.CanUseHeldItemInBattle(battleManager) && ImmuneItems.Contains(target.HeldItem)) yield break;

            foreach (KeyValuePair<VolatileStatus, float> pair in
                     StatusChances.Where(pair => battleManager.RandomProvider.Value01() <= pair.Value))
            {
                ShowAbilityNotification(owner);

                (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
                (BattlerType targetType, int targetIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(target);

                object[] extraData =
                    PrepareExtraData(pair.Key,
                                     move,
                                     effectiveness,
                                     owner,
                                     user,
                                     target,
                                     damageDealt,
                                     previousHP,
                                     wasCritical,
                                     substituteTookHit,
                                     ignoresAbilities,
                                     hitNumber,
                                     expectedMoveHits,
                                     battleManager);

                yield return battleManager.Statuses.AddStatus(pair.Key,
                                                              CalculateCountdown(pair.Key,
                                                                  battleManager,
                                                                  userType,
                                                                  userIndex,
                                                                  targetType,
                                                                  targetIndex),
                                                              target,
                                                              userType,
                                                              userIndex,
                                                              ignoresAbilities,
                                                              extraData);

                yield break;
            }
        }

        /// <summary>
        /// Calculate the countdown 
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetIndex">Index of the target.</param>
        protected virtual int CalculateCountdown(VolatileStatus status,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattlerType targetType,
                                                 int targetIndex) =>
            InfiniteDuration
                ? -1
                : status.CalculateRandomCountdown(battleManager, userType, userIndex, targetType, targetIndex);

        /// <summary>
        /// Prepare the extra data for the volatile status.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="owner">Battler owner of the ability.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="target">Target of the status.</param>
        /// <param name="damageDealt"></param>
        /// <param name="previousHP"></param>
        /// <param name="wasCritical"></param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedMoveHits">Expected hits of this move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        protected virtual object[] PrepareExtraData(VolatileStatus status,
                                                    DamageMove move,
                                                    float effectiveness,
                                                    Battler owner,
                                                    Battler user,
                                                    Battler target,
                                                    int damageDealt,
                                                    uint previousHP,
                                                    bool wasCritical,
                                                    bool substituteTookHit,
                                                    bool ignoresAbilities,
                                                    int hitNumber,
                                                    int expectedMoveHits,
                                                    BattleManager battleManager) =>
            null;
    }
}