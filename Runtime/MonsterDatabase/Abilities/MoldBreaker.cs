using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability MoldBreaker.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/MoldBreaker", fileName = "MoldBreaker")]
    public class MoldBreaker : Ability
    {
        /// <summary>
        /// Show that this monster has the ability.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            ShowAbilityNotification(battler);
            yield break;
        }

        /// <summary>
        /// Ignore other abilities.
        /// </summary>
        public override bool IgnoresOtherAbilities(BattleManager battleManager, Battler owner, Move move) => true;
    }
}