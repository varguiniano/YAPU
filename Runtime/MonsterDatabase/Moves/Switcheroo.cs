using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Switcheroo.
    /// Mostly copy pasted from StealingDamageMove.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Dark/Switcheroo", fileName = "Switcheroo")]
    public class Switcheroo : Move
    {
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
        /// Reference to the move sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the item prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SpriteRenderer ItemPrefab;

        /// <summary>
        /// Execute the effect of the move.
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
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
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

                if (!BasicChecks(user, target, bypassSubstitute)
                 || !CheckMoveImmunities(user, target, ignoresAbilities, battleManager))
                {
                    yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);

                    yield break;
                }

                if (user.HeldItem != null) yield return user.HeldItem.OnItemStolen(target, battleManager);
                if (target.HeldItem != null) yield return target.HeldItem.OnItemStolen(target, battleManager);

                user.ConsumedItemData.ConsumedItem = user.HeldItem;
                user.ConsumedItemData.CanBeRecycled = false;
                user.ConsumedItemData.CanBeRecoveredAfterBattle = true;

                target.ConsumedItemData.ConsumedItem = target.HeldItem;
                target.ConsumedItemData.CanBeRecycled = false;
                target.ConsumedItemData.CanBeRecoveredAfterBattle = true;

                (user.HeldItem, target.HeldItem) = (target.HeldItem, user.HeldItem);

                if (user.HeldItem != null) yield return user.HeldItem.OnItemReceivedInBattle(user, battleManager);
                if (target.HeldItem != null) yield return target.HeldItem.OnItemReceivedInBattle(user, battleManager);

                if (user.HeldItem != null && target.HeldItem != null)
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Switcheroo/Effect/Swap",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed,
                                                                 localizableModifiers: false,
                                                                 modifiers: new[]
                                                                            {
                                                                                user.GetNameOrNickName(localizer),
                                                                                target.GetNameOrNickName(localizer),
                                                                                user.HeldItem.GetName(localizer),
                                                                                target.HeldItem.GetName(localizer)
                                                                            });

                else if (user.HeldItem != null)
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Switcheroo/Effect/Give",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed,
                                                                 localizableModifiers: false,
                                                                 modifiers: new[]
                                                                            {
                                                                                target.GetNameOrNickName(localizer),
                                                                                user.GetNameOrNickName(localizer),
                                                                                user.HeldItem.GetName(localizer),
                                                                            });
                else // Targets is not null.
                    yield return DialogManager.ShowDialogAndWait("Dialogs/Moves/Switcheroo/Effect/Give",
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed,
                                                                 localizableModifiers: false,
                                                                 modifiers: new[]
                                                                            {
                                                                                user.GetNameOrNickName(localizer),
                                                                                target.GetNameOrNickName(localizer),
                                                                                target.HeldItem.GetName(localizer),
                                                                            });
            }
        }

        /// <summary>
        /// Do basic checks like substitute and item presence.
        /// </summary>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="bypassSubstitute">Does the move bypass substitute?</param>
        private static bool BasicChecks(MonsterInstance user, Battler target, bool bypassSubstitute)
        {
            // Can't work if the none have an item.
            if (target.HeldItem == null && user.HeldItem == null) return false;

            // Can't take mega stones from mons that can use them.
            if (user.FormData.MegaEvolutions.ContainsKey(user.HeldItem)) return false;
            if (target.FormData.MegaEvolutions.ContainsKey(target.HeldItem)) return false;

            // Doesn't affect substitutes.
            return bypassSubstitute || !target.Substitute.SubstituteEnabled;
        }

        /// <summary>
        /// Check the configured move immunities.
        /// </summary>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private bool CheckMoveImmunities(Battler user,
                                         Battler target,
                                         bool ignoresAbilities,
                                         BattleManager battleManager)
        {
            // Check user held item.
            if (StealImmuneItems.Contains(user.HeldItem)) return false;

            // Check target held item and ability.
            if (StealImmuneItems.Contains(target.HeldItem)) return false;

            if (target.CanUseAbility(battleManager, ignoresAbilities)
             && StealImmuneAbilities.Contains(target.GetAbility()))
            {
                target.GetAbility().ShowAbilityNotification(target);

                return false;
            }

            // Check immune item-monster combinations for user.
            foreach (KeyValuePair<Item, List<MonsterEntry>> _ in ImmuneItemMonsterCombinations
                                                                .Where(itemMonsterPair =>
                                                                           user.HeldItem
                                                                        == itemMonsterPair.Key)
                                                                .Where(itemMonsterPair =>
                                                                           itemMonsterPair.Value
                                                                              .Contains(target
                                                                                  .Species)
                                                                        || itemMonsterPair.Value
                                                                              .Contains(user
                                                                                  .Species)))
                return false;

            // Check immune item-monster combinations for target.
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
                return false;

            // Check immune ability-item combinations for user.
            foreach (KeyValuePair<Ability, List<Item>> _ in ImmuneItemAbilityCombinations
                                                           .Where(itemMonsterPair =>
                                                                      user.CanUseAbility(battleManager, false)
                                                                   && user.GetAbility()
                                                                   == itemMonsterPair.Key)
                                                           .Where(itemMonsterPair =>
                                                                      itemMonsterPair.Value
                                                                         .Contains(user
                                                                             .HeldItem)))
                return false;

            // Check immune ability-item combinations for target.
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
                return false;

            return true;
        }

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            for (int i = 0; i < targetPositions.Count; i++)
            {
                Transform targetPosition = targetPositions[i];

                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targets[i]);

                SpriteRenderer userItem = null;
                SpriteRenderer targetItem = null;

                if (user.HeldItem != null) userItem = Instantiate(ItemPrefab, userPosition);
                if (target.HeldItem != null) targetItem = Instantiate(ItemPrefab, targetPosition);

                yield return WaitAFrame;

                battleManager.AudioManager.PlayAudio(Sound, pitch: speed);

                if (userItem != null) userItem.sprite = user.HeldItem.Icon;
                if (targetItem != null) targetItem.sprite = target.HeldItem.Icon;

                if (userItem != null) userItem.DOFade(1, .1f / speed);
                if (targetItem != null) targetItem.DOFade(1, .1f / speed);

                yield return new WaitForSeconds(.1f / speed);

                if (userItem != null) userItem.transform.DOMove(targetPosition.position, .5f / speed);
                if (targetItem != null) targetItem.transform.DOMove(userPosition.position, .5f / speed);

                yield return new WaitForSeconds(.5f / speed);

                if (userItem != null) userItem.DOFade(0, .1f / speed);
                if (targetItem != null) targetItem.DOFade(0, .1f / speed);

                yield return new WaitForSeconds(.1f / speed);

                DOVirtual.DelayedCall(3,
                                      () =>
                                      {
                                          if (userItem != null) Destroy(userItem.gameObject);
                                          if (targetItem != null) Destroy(targetItem.gameObject);
                                      });
            }
        }
    }
}