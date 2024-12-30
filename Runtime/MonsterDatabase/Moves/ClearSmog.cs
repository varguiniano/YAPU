using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move ClearSmog.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/ClearSmog", fileName = "ClearSmog")]
    public class ClearSmog : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// It does have a secondary effect.
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Reset all stat stages of the targets.
        /// </summary>
        public override IEnumerator ExecuteSecondaryEffect(BattleManager battleManager,
                                                           ILocalizer localizer,
                                                           BattlerType userType,
                                                           int userIndex,
                                                           Battler user,
                                                           List<(BattlerType Type, int Index)> targets,
                                                           int hitNumber,
                                                           int expectedHits,
                                                           float externalPowerMultiplier,
                                                           bool ignoresAbilities)
        {
            yield return base.ExecuteSecondaryEffect(battleManager,
                                                     localizer,
                                                     userType,
                                                     userIndex,
                                                     user,
                                                     targets,
                                                     hitNumber,
                                                     expectedHits,
                                                     externalPowerMultiplier,
                                                     ignoresAbilities);

            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);
                target.ResetStatStages();

                yield return DialogManager.ShowDialogAndWait("Moves/ClearSmog/Effect",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: target.GetNameOrNickName(localizer));
            }
        }
    }
}