using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Base class for abilities that nullifies the modifier applied to a stat when changing.
    /// They can also ignore certain abilities of targets when using certain calculations.
    /// </summary>
    public abstract class PreventStatChangeAbility : Ability
    {
        /// <summary>
        /// Stats that are protected by this ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<Stat> StatsToProtect;

        /// <summary>
        /// Stats that are protected by this ability.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private List<BattleStat> BattleStatsToProtect;

        /// <summary>
        /// Dialog to use when the monster is protected.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private string ProtectionDialog;

        /// <summary>
        /// Ignore the targets evasion when calculating move accuracy?
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private bool IgnoreOpponentEvasion;

        /// <summary>
        /// Ignore the targets evasion when calculating move accuracy?
        /// </summary>
        public override bool IgnoreEvasionWhenCalculatingMoveAccuracyWhenUsing() => IgnoreOpponentEvasion;

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the ability to change the modifier.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnStatChange(Battler owner,
                                                 Stat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Ability changingAbility,
                                                 Action<short> callback)
        {
            (BattlerType ownerType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (modifier >= 0
             || ownerType == userType
             || !StatsToProtect.Contains(stat)
             || !AffectsUserOfEffect(user, owner, IgnoresOtherAbilities(battleManager, owner, null), battleManager))
            {
                callback.Invoke(modifier);

                yield break;
            }

            ShowAbilityNotification(user);

            if (!ProtectionDialog.IsNullEmptyOrWhiteSpace())
                yield return DialogManager.ShowDialogAndWait(ProtectionDialog,
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            owner.GetNameOrNickName(battleManager
                                                                               .Localizer),
                                                                            battleManager.Localizer[LocalizableName]
                                                                        });

            callback.Invoke(0);
        }

        /// <summary>
        /// Callback for when a stat stage is about to be changed. Allows the ability to change the modifier.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback with the new modifier to use.</param>
        public override IEnumerator OnStatChange(Battler owner,
                                                 BattleStat stat,
                                                 short modifier,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 BattleManager battleManager,
                                                 Action<short> callback)
        {
            (BattlerType ownerType, int _) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            Battler user = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (modifier >= 0
             || ownerType == userType
             || !BattleStatsToProtect.Contains(stat)
             || !AffectsUserOfEffect(user, owner, IgnoresOtherAbilities(battleManager, owner, null), battleManager))
            {
                callback.Invoke(modifier);

                yield break;
            }

            ShowAbilityNotification(user);

            yield return DialogManager.ShowDialogAndWait(ProtectionDialog,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        owner.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        battleManager.Localizer[LocalizableName]
                                                                    });

            callback.Invoke(0);
        }
    }
}