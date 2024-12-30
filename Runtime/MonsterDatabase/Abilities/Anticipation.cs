using System.Collections;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Anticipation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Anticipation", fileName = "Anticipation")]
    public class Anticipation : Ability
    {
        /// <summary>
        /// Shudder if the enemy has a super effective move.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            foreach (Move move in battleManager.Battlers
                                               .GetBattlersFighting(battleManager.Battlers
                                                                       .GetTypeAndIndexOfBattler(battler)
                                                                       .Type
                                                                 == BattlerType.Ally
                                                                        ? BattlerType.Enemy
                                                                        : BattlerType.Ally)
                                               .SelectMany(opponent => opponent.CurrentMoves.Select(slot => slot.Move)))
            {
                if (!battler.GetEffectivenessOfMove(battler,
                                                    move,
                                                    false,
                                                    battleManager,
                                                    false,
                                                    out float effectiveness))
                    continue;

                if (!(effectiveness > 1)) continue;

                yield return DialogManager.ShowDialogAndWait("Abilities/Anticipation/Shudder",
                                                             switchToNextAfterSeconds:
                                                             1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer));

                yield break;
            }
        }
    }
}