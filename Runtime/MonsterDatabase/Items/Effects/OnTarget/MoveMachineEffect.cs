using System;
using System.Collections;
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
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.MoveMachines;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Item effect that represents the effect of a move machine.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/MoveMachineEffect", fileName = "MoveMachineEffect")]
    [InfoBox("The move and whether the machine is spent or not is defined in the item itself.")]
    public class MoveMachineEffect : UseOnTargetItemEffect
    {
        /// <summary>
        /// Check if the effect can be used on a monster.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="item">Item owner of this effect.</param>
        /// <param name="playerCharacter"></param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(YAPUSettings settings,
                                          TimeManager timeManager,
                                          MonsterInstance monsterInstance,
                                          Item item,
                                          PlayerCharacter playerCharacter) =>
            !monsterInstance.EggData.IsEgg
         && item is MoveMachine moveMachine
         && monsterInstance.CanLearnMove(moveMachine.Move);

        /// <summary>
        /// Not usable in battle.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler to check.</param>
        /// <param name="item"></param>
        /// <returns>True if compatible.</returns>
        public override bool IsCompatible(BattleManager battleManager, Battler battler, Item item) => false;

        /// <summary>
        /// Make the monster learn the move.
        /// </summary>
        /// <param name="monsterInstance">Monster to learn.</param>
        /// <param name="item">Item holding this effect.</param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience lookup table.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="evolutionManager"></param>
        /// <param name="inputManager"></param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="finished">Callback telling if the item should be consumed.</param>
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
            MoveMachine moveMachine = (MoveMachine)item;

            bool moveLearnt = false;

            yield return DialogManager.ShowMoveLearnPanel(monsterInstance,
                                                          moveMachine.Move,
                                                          localizer,
                                                          learnt => moveLearnt = learnt);

            finished.Invoke(moveLearnt && moveMachine.IsSpentOnUse);
        }
    }
}