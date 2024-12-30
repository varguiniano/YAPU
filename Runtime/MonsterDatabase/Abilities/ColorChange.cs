using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// ColorChange ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/ColorChange", fileName = "ColorChange")]
    public class ColorChange : SetVolatileStatusOnContact
    {
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
        protected override object[] PrepareExtraData(VolatileStatus status,
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
            new object[] {move.GetMoveTypeInBattle(user, battleManager), null};
    }
}