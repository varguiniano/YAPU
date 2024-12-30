using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that changes a stat stage after hit by a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/ChangeStatAfterHitByMove",
                     fileName = "ChangeStatAfterHitByMove")]
    public class ChangeStatAfterHitByMove : AfterHitByMoveItemEffect
    {
        /// <summary>
        /// Stat to be changed.
        /// </summary>
        [SerializeField]
        private Stat StatToChange;

        /// <summary>
        /// Amount to change the stat.
        /// </summary>
        [SerializeField]
        private short Amount = 1;

        /// <summary>
        /// Mode for this effect to work on.
        /// </summary>
        [SerializeField]
        private HitMode Mode;

        /// <summary>
        /// Category to match.
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(Mode) + " == HitMode.MoveCategory")]
        private Move.Category MoveCategory;

        /// <summary>
        /// Type to match.
        /// </summary>
        [SerializeField]
        [ShowIf("@" + nameof(Mode) + " == HitMode.MoveType")]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType MoveType;

        /// <summary>
        /// Threshold from which the effect should trigger.
        /// </summary>
        [ShowIf("@" + nameof(Mode) + " == HitMode.SuperEffective")]
        [SerializeField]
        private float EffectivenessThreshold = 1f;

        /// <summary>
        /// Play an eat berry effect?
        /// </summary>
        [SerializeField]
        private bool PlayEatEffect = true;

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities">Does the move ignore abilities?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator AfterHitByMove(Item item,
                                                   DamageMove move,
                                                   float effectiveness,
                                                   Battler battler,
                                                   Battler user,
                                                   bool substituteTookHit,
                                                   bool ignoresAbilities,
                                                   BattleManager battleManager,
                                                   ILocalizer localizer,
                                                   Action<bool> finished)
        {
            if ((Mode == HitMode.MoveCategory
              && move.GetMoveCategory(user, battler, ignoresAbilities, battleManager) != MoveCategory)
             || (Mode == HitMode.MoveType && move.GetMoveTypeInBattle(user, battleManager) != MoveType)
             || (Mode == HitMode.SuperEffective && effectiveness <= EffectivenessThreshold)
             || battler.CurrentHP == 0)
            {
                finished.Invoke(false);
                yield break;
            }

            item.ShowItemNotification(battler, localizer);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            if (PlayEatEffect)
            {
                yield return battleManager.GetMonsterSprite(type, index).EatBerry(battleManager.BattleSpeed);

                DialogManager.ShowDialog("Battle/EatBerry",
                                         localizableModifiers: false,
                                         acceptInput: false,
                                         modifiers: new[]
                                                    {
                                                        battler.GetNameOrNickName(localizer), item.GetName(localizer)
                                                    },
                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
            }

            yield return battleManager.BattlerStats.ChangeStatStage(type, index, StatToChange, Amount, type, index);

            finished?.Invoke(true);
        }

        /// <summary>
        /// Enumeration of the modes for this effect.
        /// </summary>
        private enum HitMode
        {
            MoveCategory,
            MoveType,
            SuperEffective
        }
    }
}