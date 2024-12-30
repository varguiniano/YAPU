using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnHitByMove
{
    /// <summary>
    /// Data class for a held item effect that reduces the damage made by a super effective move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/HitByMove/ReduceSuperEffective",
                     fileName = "ReduceSuperEffective")]
    public class ReduceSuperEffective : HitByMoveItemEffect
    {
        /// <summary>
        /// Threshold from which the effect should trigger.
        /// </summary>
        [SerializeField]
        private float EffectivenessThreshold = 1f;

        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        private float Multiplier = .75f;
        
        /// <summary>
        /// Check the type and if it is super effective, eat the berry and reduce the effectiveness.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">Performed move.</param>
        /// <param name="effectiveness">Current effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="moveUser">User of the move.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if the item should be consumed and the new effectiveness.</param>
        public override IEnumerator OnHitByMove(Item item,
                                                DamageMove move,
                                                float effectiveness,
                                                Battler battler,
                                                BattleManager battleManager,
                                                Battler moveUser,
                                                ILocalizer localizer,
                                                Action<bool, float> finished)
        {
            if (effectiveness <= EffectivenessThreshold)
            {
                finished.Invoke(false, effectiveness);
                yield break;
            }

            item.ShowItemNotification(battler, localizer);

            yield return DialogManager.ShowDialogAndWait("Battle/MoveEffectivenessReduced",
                                                         modifiers: new[] {move.LocalizableName},
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            effectiveness *= Multiplier;

            yield return DialogManager.WaitForDialog;

            finished.Invoke(false, effectiveness);
        }
    }
}