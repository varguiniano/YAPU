using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Base class for moves that damage and then steal the held item.
    /// </summary>
    public abstract class StealingDamageMove : DamageMove
    {
        /// <summary>
        /// Steal the item or just remove it?
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        private bool StealItem = true;

        /// <summary>
        /// Items that can't be stolen.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllHoldableItems))]
        #endif
        private List<Item> StealImmuneItems;

        /// <summary>
        /// Abilities that will prevent stealing.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllAbilities))]
        #endif
        private List<Ability> StealImmuneAbilities;

        /// <summary>
        /// Items that can't be stolen if either the user or the target are the specified monster.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        private SerializedDictionary<Item, List<MonsterEntry>> ImmuneItemMonsterCombinations;

        /// <summary>
        /// Abilities that prevent stealing depending on the stolen item.
        /// </summary>
        [FoldoutGroup("Steal Effect")]
        [SerializeField]
        private SerializedDictionary<Ability, List<Item>> ImmuneItemAbilityCombinations;

        /// <summary>
        /// Does this move have a secondary effect?
        /// </summary>
        public override bool HasSecondaryEffect() => true;

        /// <summary>
        /// Execute the secondary effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
        /// <param name="externalPowerMultiplier">External multiplier applied to the power.</param>
        /// <param name="ignoresAbilities"></param>
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

                bool bypassSubstitute = user.CanUseAbility(battleManager, false)
                                     && user.GetAbility()
                                            .ByPassesSubstitute(targetType,
                                                                targetIndex,
                                                                battleManager,
                                                                userType,
                                                                userIndex,
                                                                this);

                if (!target.IsAffectedBySecondaryEffectsOfDamageMove(user,
                                                                     this,
                                                                     LastDamageMade,
                                                                     ignoresAbilities,
                                                                     battleManager))
                    yield break;

                // Won't work if user fainted.
                if (!user.CanBattle) yield break;

                // Can't steal if the target doesn't have an item.
                if (target.HeldItem == null) yield break;

                // Can't steal if the user has an item.
                if (user.HeldItem != null && StealItem) yield break;

                // Doesn't affect substitutes.
                if (!bypassSubstitute && target.Substitute.SubstituteEnabled) yield break;

                if (StealImmuneItems.Contains(target.HeldItem)) yield break;

                if (target.CanUseAbility(battleManager, ignoresAbilities)
                 && StealImmuneAbilities.Contains(target.GetAbility()))
                {
                    target.GetAbility().ShowAbilityNotification(target);

                    yield break;
                }

                foreach (KeyValuePair<Item, List<MonsterEntry>> _ in ImmuneItemMonsterCombinations
                                                                    .Where(itemMonsterPair =>
                                                                               target.HeldItem
                                                                            == itemMonsterPair.Key)
                                                                    .Where(itemMonsterPair =>
                                                                               itemMonsterPair.Value
                                                                                  .Contains(target
                                                                                      .Species)
                                                                            || itemMonsterPair.Value
                                                                                  .Contains(user
                                                                                      .Species)))
                    yield break;

                foreach (KeyValuePair<Ability, List<Item>> _ in ImmuneItemAbilityCombinations
                                                               .Where(itemMonsterPair =>
                                                                          target.CanUseAbility(battleManager,
                                                                              ignoresAbilities)
                                                                       && target.GetAbility()
                                                                       == itemMonsterPair.Key)
                                                               .Where(itemMonsterPair =>
                                                                          itemMonsterPair.Value
                                                                             .Contains(target
                                                                                 .HeldItem)))
                    yield break;

                // Can't steal mega stones from mons that can use them.
                if (target.FormData.MegaEvolutions.ContainsKey(target.HeldItem)) yield break;

                yield return target.HeldItem.OnItemStolen(target, battleManager);

                Item item = target.HeldItem;

                if (StealItem) user.HeldItem = item;

                target.ConsumedItemData.ConsumedItem = item;
                target.ConsumedItemData.CanBeRecycled = false;
                target.ConsumedItemData.CanBeRecoveredAfterBattle = true;

                target.HeldItem = null;

                if (StealItem) yield return user.HeldItem.OnItemReceivedInBattle(user, battleManager);

                yield return DialogManager.ShowDialogAndWait(StealItem
                                                                 ? "Dialogs/Moves/Thief/ItemStolen"
                                                                 : "Dialogs/Moves/KnockOff/ItemStolen",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            user.GetNameOrNickName(localizer),
                                                                            item.GetName(localizer),
                                                                            target.GetNameOrNickName(localizer)
                                                                        });
            }
        }
    }
}