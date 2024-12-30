using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move HighJumpKick.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/HighJumpKick", fileName = "HighJumpKick")]
    public class HighJumpKick : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Lose half HP when failing.
        /// </summary>
        public override IEnumerator OnMoveFailed(BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 List<(BattlerType Type, int Index)> targets,
                                                 float externalPowerMultiplier)
        {
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            int recoilDamage = Mathf.FloorToInt(user.GetStats(battleManager)[Stat.Hp] * .5f);

            yield return DialogManager.ShowDialogAndWait("Moves/HighJumpKick/Recoil",
                                                         localizableModifiers: false,
                                                         modifiers: user.GetNameOrNickName(battleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battleManager.BattlerHealth.ChangeLife(userType,
                                                                userIndex,
                                                                userType,
                                                                userIndex,
                                                                -recoilDamage,
                                                                isSecondaryDamage: true);
        }
    }
}