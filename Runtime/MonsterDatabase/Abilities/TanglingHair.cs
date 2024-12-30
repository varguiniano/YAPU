using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// TanglingHair ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/TanglingHair", fileName = "TanglingHair")]
    public class TanglingHair : Ability
    {
        /// <summary>
        /// Stat to change on contact.
        /// </summary>
        [SerializeField]
        private Stat StatToChange;

        /// <summary>
        /// Stages to change.
        /// </summary>
        [SerializeField]
        private short Stages = -1;

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
            if (!move.DoesMoveMakeContact(user, owner, battleManager, IgnoresOtherAbilities(battleManager, owner, move))
             || !AffectsUserOfEffect(user, owner, IgnoresOtherAbilities(battleManager, owner, move), battleManager))
                yield break;

            ShowAbilityNotification(owner, battleManager);

            yield return battleManager.BattlerStats.ChangeStatStage(user, StatToChange, Stages, owner, this);
        }
    }
}