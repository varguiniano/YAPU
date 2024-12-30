using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Pickup ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Pickup", fileName = "Pickup")]
    public class Pickup : Ability
    {
        /// <summary>
        /// Chance to pick up an item after battle.
        /// </summary>
        [FoldoutGroup("Effect")]
        [PropertyRange(0, 1)]
        [SerializeField]
        private float AfterBattlePickupChance = 0.1f;

        /// <summary>
        /// Items that can be picked up after battle.
        /// </summary>
        [FoldoutGroup("Effect")]
        [Tooltip("Key is the max level for each table. Value is the table with the max roll for each item.")]
        [SerializeField]
        private SerializedDictionary<int, SerializedDictionary<float, Item>> AfterBattleLootTable;

        /// <summary>
        /// Pickup a recyclable item.
        /// </summary>
        public override IEnumerator AfterTurnPreStatus(Battler battler,
                                                       BattleManager battleManager,
                                                       ILocalizer localizer)
        {
            yield return base.AfterTurnPreStatus(battler, battleManager, localizer);

            if (battler.HeldItem != null) yield break;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Battler candidate in battleManager.Battlers.GetBattlersFighting())
            {
                if (candidate == battler && battleManager.EnemyType == EnemyType.Trainer) continue;

                if (!candidate.ConsumedItemData.HasConsumedItem || !candidate.ConsumedItemData.CanBeRecycled) continue;

                ShowAbilityNotification(battler);

                battler.HeldItem = candidate.ConsumedItemData.ConsumedItem;

                candidate.ConsumedItemData.CanBeRecycled = false;

                yield return DialogManager.ShowDialogAndWait("Abilities/Pickup/Effect",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetLocalizedName(localizer),
                                                                            candidate.GetLocalizedName(localizer),
                                                                            battler.HeldItem.GetLocalizedName(localizer)
                                                                        });

                yield return battler.HeldItem.OnItemReceivedInBattle(battler, battleManager);

                yield break;
            }
        }

        /// <summary>
        /// Chance to pick up an item.
        /// </summary>
        public override IEnumerator AfterBattle(MonsterInstance monster, ILocalizer localizer)
        {
            yield return base.AfterBattle(monster, localizer);

            if (monster.HeldItem != null || Random.value > AfterBattlePickupChance) yield break;

            float roll = Random.value;

            Logger.Info("After battle pickup roll: " + roll + ".");

            SerializedDictionary<float, Item> table = new();

            // ReSharper disable twice ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (int maxLevel in AfterBattleLootTable.Keys)
                if (monster.StatData.Level <= maxLevel)
                {
                    table = AfterBattleLootTable[maxLevel];
                    break;
                }

            ShowAbilityNotification(monster);

            foreach (float chance in table.Keys)
            {
                monster.HeldItem = table[chance];
                if (roll < chance) yield break;
            }
        }
    }
}