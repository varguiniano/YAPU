using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.AfterUsingMove
{
    /// <summary>
    /// Data class for a held item effect that changes a stat stage after using a sound move
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/AfterUsingMove/DamageUserOfMove",
                     fileName = "DamageUserOfMove")]
    public class DamageUserOfMove : AfterUsingMoveItemEffect
    {
        /// <summary>
        /// Percentage of HP to change.
        /// </summary>
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPChange;

        /// <summary>
        /// Abilities immune to this item.
        /// </summary>
        [SerializeField]
        private List<Ability> ImmuneAbilities;

        /// <summary>
        /// Called after the holder uses a move.
        /// </summary>
        /// <param name="item">Item held.</param>
        /// <param name="move">The hitting move.</param>
        /// <param name="user">Move user.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback stating if it should be consumed.</param>
        public override IEnumerator AfterHittingWithMove(Item item,
                                                         Move move,
                                                         Battler user,
                                                         List<(BattlerType Type, int Index)> targets,
                                                         BattleManager battleManager,
                                                         ILocalizer localizer,
                                                         Action<bool> finished)
        {
            item.ShowItemNotification(user, battleManager.Localizer);

            if (user.CanUseAbility(battleManager, false) && ImmuneAbilities.Contains(user.GetAbility()))
            {
                user.GetAbility().ShowAbilityNotification(user);
                finished.Invoke(false);
                yield break;
            }

            int hpToChange =
                Mathf.Max((int) (MonsterMathHelper.CalculateStat(user, Stat.Hp, battleManager) * HPChange), 1);

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            int amount = 0;

            yield return battleManager.BattlerHealth.ChangeLife(user,
                                                                user,
                                                                -hpToChange,
                                                                isSecondaryDamage: true,
                                                                finished: (finalAmount, _) =>
                                                                          {
                                                                              amount = finalAmount;
                                                                          });

            if (amount == 0)
            {
                finished.Invoke(false);
                yield break;
            }

            battleManager.Battlers.GetPanel(type, index).UpdatePanel(battleManager.BattleSpeed, tween: true);

            finished.Invoke(false);
        }
    }
}