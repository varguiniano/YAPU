using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the AngerPoint ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/AngerPoint", fileName = "AngerPoint")]
    public class AngerPoint : Ability
    {
        /// <summary>
        /// Stat to max when hit by a critical.
        /// </summary>
        [SerializeField]
        private Stat StatToMax;

        /// <summary>
        /// Called after the monster is hit by a move.
        /// </summary>
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
            if (!wasCritical || substituteTookHit) yield break;

            ShowAbilityNotification(owner);
            yield return battleManager.BattlerStats.ChangeStatStage(owner, StatToMax, 12, owner, this);
        }
    }
}