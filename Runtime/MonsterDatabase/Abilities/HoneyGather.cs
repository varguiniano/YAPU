using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// HoneyGather ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/HoneyGather", fileName = "HoneyGather")]
    public class HoneyGather : Ability
    {
        /// <summary>
        /// Reference tot he honey item.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllItems))]
        #endif
        private Item HoneyItem;

        /// <summary>
        /// Chance to pick up an item.
        /// </summary>
        public override IEnumerator AfterBattle(MonsterInstance monster, ILocalizer localizer)
        {
            yield return base.AfterBattle(monster, localizer);

            if (monster.HeldItem != null) yield break;

            float roll = Random.value;

            Logger.Info("After battle pickup roll: " + roll + ".");

            float chance = monster.StatData.Level * .005f; // Increases by 0.5% every 10th level.

            if (roll > chance) yield break;

            ShowAbilityNotification(monster);

            monster.HeldItem = HoneyItem;
        }
    }
}