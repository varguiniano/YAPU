using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Battle AI that uses battle items in certain conditions, falls back if conditions are not met.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Battle/AI/UseBattleItems", fileName = "UseBattleItems")]
    public class UseBattleItems : BattleAI
    {
        /// <summary>
        /// Fallback to go to.
        /// </summary>
        [InfoBox("This AI uses battle items in certain conditions, falls back if conditions are not met.")]
        [SerializeField]
        private BattleAI Fallback;

        /// <summary>
        /// Items that will be used if they are available.
        /// </summary>
        [FoldoutGroup("Always Use")]
        [SerializeField]
        private List<Item> UseIfAvailable;

        /// <summary>
        /// Items that will be used if they are available.
        /// </summary>
        [FoldoutGroup("Always Use")]
        [SerializeField]
        private List<Item> UseOnOwnIfAvailable;

        /// <summary>
        /// HP heal threshold.
        /// </summary>
        [FoldoutGroup("HP Heal")]
        [SerializeField]
        [PropertyRange(0, 1)]
        private float HPThreshold;

        /// <summary>
        /// Items to be used when the AI wants to heal HP.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllUsableInBattleOnTargetItems))]
        #endif
        [FoldoutGroup("HP Heal")]
        [SerializeField]
        private List<Item> HPHealItems;

        /// <summary>
        /// Dictionary of statuses and items that can heal them.
        /// </summary>
        [FoldoutGroup("Status Heal")]
        [SerializeField]
        private SerializableDictionary<Status, List<Item>> StatusHeals;

        /// <summary>
        /// Reference to the confusion status.
        /// </summary>
        [FoldoutGroup("Status Heal")]
        [SerializeField]
        private VolatileStatus Confusion;

        /// <summary>
        /// Items to be used to heal confusion.
        /// </summary>
        [FoldoutGroup("Status Heal")]
        [SerializeField]
        private List<Item> ConfusionHeal;

        /// <summary>
        /// Item to be used to revive.
        /// </summary>
        [FoldoutGroup("Revives")]
        [SerializeField]
        private List<Item> Revives;

        /// <summary>
        /// Request to choose to perform an action.
        /// </summary>
        /// <param name="settings">Reference to the yapu settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="callback">Callback stating the action to take along with its parameters.</param>
        /// <returns>The action taken along with its parameters.</returns>
        public override IEnumerator RequestPerformAction(YAPUSettings settings,
                                                         BattleManager battleManager,
                                                         BattlerType type,
                                                         int inBattleIndex,
                                                         Action<BattleAction> callback)
        {
            if (battleManager.EnemyType != EnemyType.Trainer)
            {
                yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
                yield break;
            }

            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            (BattlerType _, int rosterIndex, int battlerIndexInsideRoster) =
                battleManager.Battlers.GetTypeAndRosterIndexOfBattler(battler);

            List<Battler> ownRoster = battleManager.Rosters.GetRoster(type, inBattleIndex);

            Bag bag = battleManager.Items.GetBag(type, inBattleIndex);
            List<Item> availableInBag = bag.GetAllAvailableItemTypes();

            List<Item> availableToUse =
                GetCompatibleAvailableItemsForAction(UseIfAvailable, availableInBag, battleManager, battler);

            if (availableToUse.Count > 0)
            {
                callback.Invoke(UseItem(battleManager, type, inBattleIndex, bag, availableToUse));
                yield break;
            }

            List<Item> availableToUseOnOwn =
                GetCompatibleAvailableItemsForAction(UseOnOwnIfAvailable,
                                                     availableInBag,
                                                     battleManager,
                                                     battler,
                                                     battler);

            if (availableToUseOnOwn.Count > 0)
            {
                callback.Invoke(UseItemOnOwn(battleManager,
                                             type,
                                             inBattleIndex,
                                             bag,
                                             availableToUseOnOwn,
                                             rosterIndex,
                                             battlerIndexInsideRoster));

                yield break;
            }

            List<Item> availableItemsForHealingHP =
                GetCompatibleAvailableItemsForAction(HPHealItems, availableInBag, battleManager, battler, battler);

            if ((float) battler.CurrentHP / MonsterMathHelper.CalculateStat(battler, Stat.Hp, battleManager)
             <= HPThreshold
             && battler.CanBattle
             && availableItemsForHealingHP.Count > 0)
            {
                callback.Invoke(UseItemOnOwn(battleManager,
                                             type,
                                             inBattleIndex,
                                             bag,
                                             availableItemsForHealingHP,
                                             rosterIndex,
                                             battlerIndexInsideRoster));

                yield break;
            }

            foreach (KeyValuePair<Status, List<Item>> heal in StatusHeals)
            {
                List<Item> availableItemsForHealingStatus =
                    GetCompatibleAvailableItemsForAction(heal.Value, availableInBag, battleManager, battler, battler);

                if (battler.GetStatus() != heal.Key || availableItemsForHealingStatus.Count <= 0) continue;

                callback.Invoke(UseItemOnOwn(battleManager,
                                             type,
                                             inBattleIndex,
                                             bag,
                                             availableItemsForHealingStatus,
                                             rosterIndex,
                                             battlerIndexInsideRoster));

                yield break;
            }

            List<Item> confusionHealersAvailable =
                GetCompatibleAvailableItemsForAction(ConfusionHeal, availableInBag, battleManager, battler, battler);

            if (battler.HasVolatileStatus(Confusion) && confusionHealersAvailable.Count > 0)
            {
                callback.Invoke(UseItemOnOwn(battleManager,
                                             type,
                                             inBattleIndex,
                                             bag,
                                             confusionHealersAvailable,
                                             rosterIndex,
                                             battlerIndexInsideRoster));

                yield break;
            }

            foreach (Battler target in ownRoster)
            {
                List<Item> revivesAvailable =
                    GetCompatibleAvailableItemsForAction(Revives, availableInBag, battleManager, battler, target);

                if (revivesAvailable.Count <= 0) continue;

                (BattlerType targetType, int targetRosterIndex, int targetIndex) =
                    battleManager.Battlers.GetTypeAndRosterIndexOfBattler(target);

                callback.Invoke(UseItemOnTarget(battleManager,
                                                type,
                                                inBattleIndex,
                                                bag,
                                                revivesAvailable,
                                                targetType,
                                                targetRosterIndex,
                                                targetIndex));

                yield break;
            }

            yield return Fallback.RequestPerformAction(settings, battleManager, type, inBattleIndex, callback);
        }

        /// <summary>
        /// Request the AI to send a new monster after a monster has fainted.
        /// </summary>
        /// <param name="settings">Reference to the yapu settings.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index of the monster that just fainted..</param>
        /// <param name="forbiddenBattlers"></param>
        /// <returns>The index of the monster to send from the AI's roster.</returns>
        public override int
            RequestNewMonster(YAPUSettings settings,
                              BattleManager battleManager,
                              BattlerType type,
                              int inBattleIndex,
                              List<Battler> forbiddenBattlers) =>
            Fallback.RequestNewMonster(settings, battleManager, type, inBattleIndex, forbiddenBattlers);

        /// <summary>
        /// Create an action for using a random item from a list.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="bag">Reference to the bag.</param>
        /// <param name="availableItems">Available items for this action.</param>
        /// <returns>The action taken along with its parameters.</returns>
        private static BattleAction UseItem(BattleManager battleManager,
                                            BattlerType type,
                                            int inBattleIndex,
                                            Bag bag,
                                            List<Item> availableItems) =>
            new()
            {
                BattlerType = type,
                Index = inBattleIndex,
                ActionType = BattleAction.Type.Item,
                Parameters = new[] {0, bag.GetIndexOfItem(battleManager.RandomProvider.RandomElement(availableItems))}
            };

        /// <summary>
        /// Create an action for using a random item from a list on itself.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="bag">Reference to the bag.</param>
        /// <param name="availableItems">Available items for this action.</param>
        /// <param name="rosterIndex">Roster index of the battler.</param>
        /// <param name="battlerIndexInsideRoster">Index of the battler inside that roster.</param>
        /// <returns>The action taken along with its parameters.</returns>
        private static BattleAction UseItemOnOwn(BattleManager battleManager,
                                                 BattlerType type,
                                                 int inBattleIndex,
                                                 Bag bag,
                                                 List<Item> availableItems,
                                                 int rosterIndex,
                                                 int battlerIndexInsideRoster) =>
            UseItemOnTarget(battleManager,
                            type,
                            inBattleIndex,
                            bag,
                            availableItems,
                            type,
                            rosterIndex,
                            battlerIndexInsideRoster);

        /// <summary>
        /// Create an action for using a random item from a list on a target.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="type">Type of this AI's battler.</param>
        /// <param name="inBattleIndex">Reference to the AI's in battle index.</param>
        /// <param name="bag">Reference to the bag.</param>
        /// <param name="availableItems">Available items for this action.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetRosterIndex">Roster index of the battler.</param>
        /// <param name="targetIndexInsideRoster">Index of the battler inside that roster.</param>
        /// <returns>The action taken along with its parameters.</returns>
        private static BattleAction UseItemOnTarget(BattleManager battleManager,
                                                    BattlerType type,
                                                    int inBattleIndex,
                                                    Bag bag,
                                                    List<Item> availableItems,
                                                    BattlerType targetType,
                                                    int targetRosterIndex,
                                                    int targetIndexInsideRoster) =>
            new()
            {
                BattlerType = type,
                Index = inBattleIndex,
                ActionType = BattleAction.Type.Item,
                Parameters = new[]
                             {
                                 1,
                                 bag.GetIndexOfItem(battleManager.RandomProvider.RandomElement(availableItems)),
                                 (int) targetType,
                                 targetRosterIndex,
                                 targetIndexInsideRoster
                             }
            };

        /// <summary>
        /// Get a list of items available for a possible action.
        /// </summary>
        /// <param name="possibleItems">The items possible for the action.</param>
        /// <param name="availableItemsInBag">The items available in the bag.</param>
        /// <param name="battleManager">Battle manager reference.</param>
        /// <param name="battler">Battler to use the item.</param>
        /// <param name="target">Target of the item.</param>
        /// <returns>A filtered list that excludes those not compatible.</returns>
        private static List<Item> GetCompatibleAvailableItemsForAction(ICollection<Item> possibleItems,
                                                                       IEnumerable<Item> availableItemsInBag,
                                                                       BattleManager battleManager,
                                                                       Battler battler,
                                                                       Battler target) =>
            GetAvailableItemsForAction(possibleItems, availableItemsInBag)
               .Where(item => item.IsCompatible(battleManager, target))
               .ToList();

        /// <summary>
        /// Get a list of items available for a possible action without a target.
        /// </summary>
        /// <param name="possibleItems">The items possible for the action.</param>
        /// <param name="availableItemsInBag">The items available in the bag.</param>
        /// <param name="battleManager">Battle manager reference.</param>
        /// <param name="battler">Battler to use the item.</param>
        /// <returns>A filtered list that excludes those not compatible.</returns>
        private static List<Item> GetCompatibleAvailableItemsForAction(ICollection<Item> possibleItems,
                                                                       IEnumerable<Item> availableItemsInBag,
                                                                       BattleManager battleManager,
                                                                       Battler battler) =>
            GetAvailableItemsForAction(possibleItems, availableItemsInBag)
               .Where(item => item.CanBeUsedInBattleRightNow(battleManager, battler))
               .ToList();

        /// <summary>
        /// Get a list of items available for a possible action.
        /// </summary>
        /// <param name="possibleItems">The items possible for the action.</param>
        /// <param name="availableItemsInBag">The items available in the bag.</param>
        /// <returns>An inner join of both lists.</returns>
        private static List<Item> GetAvailableItemsForAction(ICollection<Item> possibleItems,
                                                             IEnumerable<Item> availableItemsInBag) =>
            availableItemsInBag.Where(possibleItems.Contains).ToList();
    }
}