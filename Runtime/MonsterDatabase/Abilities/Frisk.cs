using System.Collections;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability Frisk.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Frisk", fileName = "Frisk")]
    public class Frisk : Ability
    {
        /// <summary>
        /// Check all the items the opponents have.
        /// </summary>
        public override IEnumerator OnMonsterEnteredBattle(BattleManager battleManager, Battler battler)
        {
            yield return base.OnMonsterEnteredBattle(battleManager, battler);

            ShowAbilityNotification(battler);

            (BattlerType userType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            foreach (Battler opponent in battleManager.Battlers.GetBattlersFighting(userType == BattlerType.Ally
                                                           ? BattlerType.Enemy
                                                           : BattlerType.Ally)
                                                      .Where(opponent => opponent.HeldItem != null))
                yield return DialogManager.ShowDialogAndWait("Abilities/Frisk/Effect",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetLocalizedName(battleManager
                                                                               .Localizer),
                                                                            opponent.HeldItem
                                                                               .GetLocalizedName(battleManager
                                                                                   .Localizer),
                                                                            opponent.GetLocalizedName(battleManager
                                                                               .Localizer)
                                                                        });
        }
    }
}