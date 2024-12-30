using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing an item effect that changes friendship.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/FriendshipChangeEffect",
                     fileName = "FriendshipChangeEffect")]
    public class FriendshipChangeEffect : UseOnTargetItemEffect
    {
        /// <summary>
        /// The change in friendship to apply based on max threshold.
        /// </summary>
        [SerializeField]
        [InfoBox("Keys are the 'up to' threshold (non inclusive), values are the change to apply below that threshold. No valid threshold will default to 0.")]
        private SerializableDictionary<byte, int> Change;

        /// <summary>
        /// Should this effect affect compatibility?
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
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="item"></param>
        /// <param name="playerCharacter"></param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(YAPUSettings settings,
                                          TimeManager timeManager,
                                          MonsterInstance monsterInstance,
                                          Item item,
                                          PlayerCharacter playerCharacter) =>
            AffectCompatibility && monsterInstance.Friendship < 255;

        /// <summary>
        /// Apply the friendship changing effect.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="item"></param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="evolutionManager"></param>
        /// <param name="inputManager"></param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        public override IEnumerator UseOnMonsterInstance(MonsterInstance monsterInstance,
                                                         Item item,
                                                         YAPUSettings settings,
                                                         ExperienceLookupTable experienceLookupTable,
                                                         PlayerCharacter playerCharacter,
                                                         TimeManager timeManager,
                                                         EvolutionManager evolutionManager,
                                                         IInputManager inputManager,
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
                                                                        },
                                                             switchToNextAfterSeconds: 1.5f);

            finished?.Invoke(true);
        }

        /// <summary>
        /// Is this effect compatible with the given monster?
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Monster to check.</param>
        /// <param name="item"></param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler, Item item) =>
            IsCompatible(battleManager.YAPUSettings,
                         battleManager.TimeManager,
                         battler,
                         item,
                         battleManager.PlayerCharacter);

        /// <summary>
        /// Apply the friendship changing effect.
        /// </summary>
        /// <param name="item">Reference to the used item.</param>
        /// <param name="battler">Reference to that battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="localizer">Localizer reference.</param>
        /// <param name="finished">Finished callback stating if the item should be consumed.</param>
        /// <param name="wasFlung">Was the item flung to this battler?</param>
        public override IEnumerator UseOnBattler(Item item,
                                                 Battler battler,
                                                 BattleManager battleManager,
                                                 YAPUSettings settings,
                                                 ExperienceLookupTable experienceLookupTable,
                                                 ILocalizer localizer,
                                                 Action<bool> finished,
                                                 bool wasFlung = false)
        {
            yield return UseOnMonsterInstance(battler,
                                              item,
                                              settings,
                                              experienceLookupTable,
                                              battleManager.PlayerCharacter,
                                              battleManager.TimeManager,
                                              null,
                                              null,
                                              localizer,
                                              finished);
        }
    }
}