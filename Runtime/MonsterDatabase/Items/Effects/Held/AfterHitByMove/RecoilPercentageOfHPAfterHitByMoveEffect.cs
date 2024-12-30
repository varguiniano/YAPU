using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterHitByMove
{
    /// <summary>
    /// Data class for a held item effect that recoils part of the physical move after the monster has been hit by a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterHit/RecoilPercentageOfHPAfterHitByMoveEffect",
                     fileName = "RecoilPercentageOfHPAfterHitByMoveEffect")]
    public class RecoilPercentageOfHPAfterHitByMoveEffect : AfterHitByMoveItemEffect
    {
        /// <summary>
        /// Percentage of HP to change.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPChange = .125f;

        /// <summary>
        /// Category to match.
        /// </summary>
        [SerializeField]
        private Move.Category MoveCategory;

        /// <summary>
        /// Called after the holder is hit by a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="effectiveness">Its effectiveness.</param>
        /// <param name="battler">Battler holding the item.</param>
        /// <param name="user">Reference to the move's user.</param>
        /// <param name="substituteTookHit">Did the substitute take the hit?</param>
        /// <param name="ignoresAbilities"></param>
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
            if (move.GetMoveCategory(user, battler, ignoresAbilities, battleManager) != MoveCategory || !battler.CanBattle)
            {
                finished.Invoke(false);
                yield break;
            }

            item.ShowItemNotification(battler, localizer);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            // TODO: Move this to a separate effect?
            yield return battleManager.GetMonsterSprite(type, index).EatBerry(battleManager.BattleSpeed);

            yield return DialogManager.ShowDialogAndWait("Battle/EatBerry",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        item.GetName(localizer)
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            yield return DialogManager.ShowDialogAndWait("Battle/MoveRecoiled",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            int hpToChange = (int) (MonsterMathHelper.CalculateStat(user, Stat.Hp, battleManager) * HPChange);

            yield return battleManager.BattlerHealth.ChangeLife(battler, battler, -hpToChange);

            battleManager.Battlers.GetPanel(userType, userIndex).UpdatePanel(battleManager.BattleSpeed);

            finished?.Invoke(true);
        }
    }
}