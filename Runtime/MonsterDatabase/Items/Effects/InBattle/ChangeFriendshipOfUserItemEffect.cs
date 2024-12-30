using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.InBattle
{
    /// <summary>
    /// Item effect that changes the friendship of the user.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/InBattle/ChangeFriendshipOfUserItemEffect",
                     fileName = "ChangeFriendshipOfUserItemEffect")]
    public class ChangeFriendshipOfUserItemEffect : UseInBattleItemEffect
    {
        /// <summary>
        /// The change in friendship to apply based on max threshold.
        /// </summary>
        [SerializeField]
        [InfoBox("Keys are the 'up to' threshold (non inclusive), values are the change to apply below that threshold. No valid threshold will default to 0.")]
        private SerializableDictionary<byte, int> Change;

        /// <summary>
        /// This effect can't be used on its own.
        /// </summary>
        /// <param name="battleManager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public override bool CanBeUsed(BattleManager battleManager, Battler user) => false;

        /// <summary>
        /// Use in battle.
        /// </summary>
        /// <param name="item">Item being used.</param>
        /// <param name="userType">Battler type of the user.</param>
        /// <param name="userIndex">Index of the user.</param>
        /// <param name="battleManager"></param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator Use(Item item,
                                        BattlerType userType,
                                        int userIndex,
                                        BattleManager battleManager,
                                        Action<bool> finished)
        {
            int amount = 0;

            Battler monsterInstance = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            foreach (KeyValuePair<byte, int> threshold in Change)
            {
                if (monsterInstance.Friendship >= threshold.Key) continue;
                amount = threshold.Value;
                break;
            }

            monsterInstance.ChangeFriendship(amount,
                                             battleManager.PlayerCharacter.Scene.Asset,
                                             battleManager.Localizer);

            finished?.Invoke(true);

            yield break;
        }
    }
}