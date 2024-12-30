using System.Collections;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Forewarn.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Forewarn", fileName = "Forewarn")]
    public class Forewarn : Ability
    {
        /// <summary>
        /// Shudder if the enemy has a super effective move.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            ShowAbilityNotification(battler);

            Battler maxOpponent = null;
            DamageMove maxMove = null;
            int maxPower = 0;

            foreach (Battler opponent in battleManager.Battlers
                                                      .GetBattlersFighting(battleManager.Battlers
                                                                              .GetTypeAndIndexOfBattler(battler)
                                                                              .Type
                                                                        == BattlerType.Ally
                                                                               ? BattlerType.Enemy
                                                                               : BattlerType.Ally))

            {
                foreach (Move move in opponent.CurrentMoves.Select(slot => slot.Move))
                {
                    if (move is not DamageMove damageMove) continue;

                    int power = damageMove.GetMovePowerInBattle(battleManager, opponent, battler, false);

                    if (power <= maxPower) continue;

                    maxPower = power;
                    maxMove = damageMove;
                    maxOpponent = opponent;
                }
            }

            if (maxOpponent == null)
                yield return DialogManager.ShowDialogAndWait("Abilities/Forewarn/NoMove",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(battleManager
                                                                .Localizer));
            else
                yield return DialogManager.ShowDialogAndWait("Abilities/Forewarn/FoundMove",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetNameOrNickName(battleManager
                                                                               .Localizer),
                                                                            maxOpponent.GetNameOrNickName(battleManager
                                                                               .Localizer),
                                                                            maxMove.GetLocalizedName(battleManager
                                                                               .Localizer)
                                                                        });
        }
    }
}