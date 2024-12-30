using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing an item effect that changes EVs.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/EVChangeEffect", fileName = "EVChangeEffect")]
    public class EVChangeEffect : UseOnTargetItemEffect
    {
        /// <summary>
        /// Stat to change.
        /// </summary>
        [SerializeField]
        private Stat Stat;

        /// <summary>
        /// Change applied to the EV.
        /// </summary>
        [SerializeField]
        private int EVChange;

        /// <summary>
        /// Should this effect affect compatibility?
        /// </summary>
        [SerializeField]
        private bool AffectCompatibility;

        /// <summary>
        /// Show a dialog?
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
            AffectCompatibility
         && EVChange > 0
         && !monsterInstance.EggData.IsEgg
         && byte.MaxValue - monsterInstance.StatData.EffortValues[Stat] > 0
         && monsterInstance.StatData.GetTotalEVs() < settings.MaxEVPointsPerMonster;

        /// <summary>
        /// Apply the friendship changing effect.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="item"></param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter"></param>
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
            monsterInstance.ChangeEV(settings, Stat, EVChange, localizer);

            if (ShowDialog)
                yield return DialogManager.ShowDialogAndWait("Dialogs/EVRaised",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            localizer[Stat.GetLocalizationString()],
                                                                            monsterInstance.GetNameOrNickName(localizer)
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