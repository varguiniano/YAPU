using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Pressure.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Pressure", fileName = "Pressure")]
    public class Pressure : Ability
    {
        /// <summary>
        /// Show notification.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            ShowAbilityNotification(battler);
            yield break;
        }

        /// <summary>
        /// Reduce one more PP.
        /// </summary>
        public override IEnumerator OnHitByMove(DamageMove move,
                                                float effectiveness,
                                                Battler owner,
                                                BattleManager battleManager,
                                                Battler moveUser,
                                                Action<float> finished)
        {
            yield return base.OnHitByMove(move, effectiveness, owner, battleManager, moveUser, finished);

            if (move == null
             || moveUser == null
             || battleManager.Battlers.GetTypeAndIndexOfBattler(owner).Type
             == battleManager.Battlers.GetTypeAndIndexOfBattler(moveUser).Type)
                yield break;

            int index = moveUser.GetMoveIndex(move);

            if (index < 0) yield break;

            moveUser.CurrentMoves[index].CurrentPP--;
        }

        /// <summary>
        /// Only use the upper half.
        /// </summary>
        /// <param name="monster">Owner of the ability.</param>
        /// <param name="encounter">Encounter type.</param>
        /// <param name="minimum">Minimum level.</param>
        /// <param name="maximum">Maximum level.</param>
        /// <returns>The new limits.</returns>
        public override (byte minimum, byte maximum)
            ModifyEncounterLevels(MonsterInstance monster, EncounterType encounter, byte minimum, byte maximum) =>
            ((byte) (minimum + (maximum - minimum) / 2f), maximum);
    }
}