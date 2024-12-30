using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Volatile status for protecting the user.
    /// It also damages the attacker.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/SpikyShielding", fileName = "SpikyShielding")]
    public class SpikyShielding : Protection
    {
        /// <summary>
        /// Percentage to HP to recoil.
        /// </summary>
        [PropertyRange(0, 1)]
        [SerializeField]
        private float RecoilPercentageOfMaxHP = 1f / 16f;

        /// <summary>
        /// Localization key for the recoil message.
        /// </summary>
        [SerializeField]
        private string RecoilMessage = "Abilities/SpikyShield/RecoilMessage";

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowUsedMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public override IEnumerator OnAboutToBeHitByMove(Battler target,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Battler user,
                                                         bool didShowUsedMessageNormally,
                                                         Action<bool> callback)
        {
            if (!move.AffectedByProtect || !move.DoesMoveMakeContact(user, target, battleManager, false)) yield break;

            yield return DialogManager.ShowDialogAndWait(RecoilMessage,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        user.GetNameOrNickName(battleManager.Localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return battleManager.BattlerHealth.ChangeLife(user,
                                                                target,
                                                                -(int) (user.GetStats(battleManager)[Stat.Hp]
                                                                      * RecoilPercentageOfMaxHP),
                                                                isSecondaryDamage: true);
        }
    }
}