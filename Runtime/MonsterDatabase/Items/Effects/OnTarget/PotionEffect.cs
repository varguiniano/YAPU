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
    /// Data class representing a potion effect.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/Potion", fileName = "PotionEffect")]
    public class PotionEffect : UseOnTargetItemEffect
    {
        /// <summary>
        /// Amount of HP to change.
        /// </summary>
        [SerializeField]
        private int HPChange;

        /// <summary>
        /// Check if the effect can be used on a monster.
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
            monsterInstance.CanBattle
         && monsterInstance.CurrentHP < MonsterMathHelper.CalculateStat(monsterInstance, Stat.Hp, null);

        /// <summary>
        /// Apply the life changing effect.
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
            (int currentHP, int previousHP, bool _) = monsterInstance.ChangeHP(HPChange);

            int amount = currentHP - previousHP;

            Logger.Info("Used potion effect "
                      + name
                      + " on "
                      + monsterInstance.Species.name
                      + ", HP changed by "
                      + amount
                      + ".");

            DialogManager.ShowDialog("Battle/RecoverHP",
                                     acceptInput: false,
                                     localizableModifiers: false,
                                     modifiers: new[]
                                                {
                                                    monsterInstance.GetNameOrNickName(localizer), amount.ToString()
                                                });

            finished?.Invoke(true);

            yield break;
        }

        /// <summary>
        /// Same case as outside of battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler to check.</param>
        /// <param name="item">Item that has this effect.</param>
        /// <returns>True if compatible.</returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler, Item item) =>
            IsCompatible(battleManager.YAPUSettings,
                         battleManager.TimeManager,
                         battler,
                         item,
                         battleManager.PlayerCharacter);

        /// <summary>
        /// Apply the life changing effect.
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
            if (wasFlung && !battler.CanHeal(battleManager))
            {
                finished.Invoke(false);
                yield break;
            }

            int amount = 0;

            (BattlerType type, int index) = battleManager.Battlers.GetTypeAndIndexOfBattler(battler);

            yield return battleManager.BattlerHealth.ChangeLife(battler,
                                                                type,
                                                                index,
                                                                HPChange,
                                                                finished: (delta, _) => amount = delta);

            Logger.Info("Used potion effect "
                      + name
                      + " on "
                      + battler.Species.name
                      + ", HP changed by "
                      + amount
                      + ".");

            yield return DialogManager.ShowDialogAndWait("Battle/RecoverHP",
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        battler.GetNameOrNickName(localizer),
                                                                        amount.ToString()
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

            finished?.Invoke(true);
        }
    }
}