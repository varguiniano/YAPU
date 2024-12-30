using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnHitByMove
{
    /// <summary>
    /// Data class for a held item effect that reduces the damage made by a super effective move if it matches the set type.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/HitByMove/ReduceSuperEffectiveIfItMatchesType",
                     fileName = "ReduceSuperEffectiveIfItMatchesType")]
    public class ReduceSuperEffectiveIfItMatchesType : HitByMoveItemEffect
    {
        /// <summary>
        /// Type to match to reduce the effectiveness.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [SerializeField]
        private MonsterType Type;

        /// <summary>
        /// Threshold from which the effect should trigger.
        /// </summary>
        [SerializeField]
        private float EffectivenessThreshold = 1f;

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
            if (Type != move.GetMoveTypeInBattle(moveUser, battleManager) || effectiveness <= EffectivenessThreshold)
            {
                finished.Invoke(false, effectiveness);
                yield break;
            }

            item.ShowItemNotification(battler, localizer);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battleManager.GetMonsterSprite(type, index).EatBerry(battleManager.BattleSpeed);

            yield return DialogManager.ShowDialogAndWait("Battle/EatBerry",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        item.GetName(localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return DialogManager.ShowDialogAndWait("Battle/MoveEffectivenessReduced",
                                                         modifiers: new[] {move.LocalizableName},
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            effectiveness *= .5f;

            yield return DialogManager.WaitForDialog;

            finished.Invoke(true, effectiveness);
        }
    }
}