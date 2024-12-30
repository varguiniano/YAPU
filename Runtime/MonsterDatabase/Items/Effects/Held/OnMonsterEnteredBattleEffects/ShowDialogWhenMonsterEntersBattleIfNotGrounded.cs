using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnMonsterEnteredBattleEffects
{
    /// <summary>
    /// Data class for a held item effect that shows a message when the monster enters the battlefield if it is not grounded.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/EntersBattle/ShowDialogWhenMonsterEntersBattleIfNotGrounded",
                     fileName = "ShowDialogWhenMonsterEntersBattleIfNotGrounded")]
    public class ShowDialogWhenMonsterEntersBattleIfNotGrounded : ShowDialogWhenMonsterEntersBattle
    {
        /// <summary>
        /// Condition to check if the message should be shown.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it should be shown.</returns>
        protected override bool ShowCondition(Item item, Battler battler, BattleManager battleManager) =>
            !battler.IsGrounded(battleManager, false);
    }
}