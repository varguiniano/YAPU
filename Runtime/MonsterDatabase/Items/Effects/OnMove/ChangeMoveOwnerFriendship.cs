using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnMove
{
    /// <summary>
    /// Use on move item effect that changes the friendship of the move owner.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTargetMove/ChangeMoveOwnerFriendship",
                     fileName = "ChangeMoveOwnerFriendship")]
    public class ChangeMoveOwnerFriendship : UseOnTargetMoveItemEffect
    {
        /// <summary>
        /// The change in friendship to apply based on max threshold.
        /// </summary>
        [SerializeField]
        [InfoBox("Keys are the 'up to' threshold (non inclusive), values are the change to apply below that threshold. No valid threshold will default to 0.")]
        private SerializableDictionary<byte, int> Change;

        /// <summary>
        /// Does this effect affect compatibility.
        /// </summary>
        [SerializeField]
        private bool AffectCompatibility;

        /// <summary>
        /// Show a dialog when executing this effect.
        /// </summary>
        [SerializeField]
        private bool ShowDialog;

        /// <summary>
        /// Is this effect compatible with the given monster?
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(MonsterInstance monsterInstance) =>
            AffectCompatibility && monsterInstance.Friendship < 255;

        /// <summary>
        /// Check if the effect can be used on a monster in battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler) => IsCompatible(battler);

        /// <summary>
        /// Check if the effect can be used on a move slot.
        /// </summary>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="index">Move slot index.</param>
        /// <returns>True if it can be used.</returns>
        public override bool IsMoveCompatible(MonsterInstance monsterInstance, int index) =>
            IsCompatible(monsterInstance);

        /// <summary>
        /// Check if the effect can be used on a move slot.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="index">Move slot index.</param>
        /// <returns>True if it can be used.</returns>
        public override bool IsMoveCompatible(BattleManager battleManager, Battler battler, int index) =>
            IsCompatible(battler);

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="index">Move slot index.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator UseOnMonsterInstance(MonsterInstance monsterInstance,
                                                         int index,
                                                         PlayerCharacter playerCharacter,
                                                         ILocalizer localizer,
                                                         Action<bool> finished)
        {
            int amount = 0;

            foreach (KeyValuePair<byte, int> threshold in Change)
            {
                if (monsterInstance.Friendship >= threshold.Key) continue;
                amount = threshold.Value;
                break;
            }

            monsterInstance.ChangeFriendship(amount, playerCharacter.Scene.Asset, localizer);

            if (ShowDialog)
                yield return DialogManager.ShowDialogAndWait(amount > 0
                                                                 ? "Monsters/FriendshipRaised"
                                                                 : "Monsters/FriendshipDecreased",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            monsterInstance
                                                                               .GetNameOrNickName(localizer),
                                                                            amount.ToString()
                                                                        });

            finished?.Invoke(true);
        }

        /// <summary>
        /// Use on a battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="index">Move slot index.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator UseOnBattler(BattleManager battleManager,
                                                 Battler battler,
                                                 int index,
                                                 ILocalizer localizer,
                                                 Action<bool> finished) =>
            UseOnMonsterInstance(battler, index, battleManager.PlayerCharacter, localizer, finished);
    }
}