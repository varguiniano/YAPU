using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using WhateverDevs.Localization.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.OnTarget
{
    /// <summary>
    /// Data class representing an item effect that triggers a monster evolution as if it had been traded.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/OnTarget/TriggerTradeEvolution",
                     fileName = "TriggerTradeEvolutionOnUseEffect")]
    public class TriggerTradeEvolutionOnUseEffect : UseOnTargetItemEffect
    {
        /// <summary>
        /// Check if the effect can be used on a monster.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="monsterInstance">Monster to check.</param>
        /// <param name="item">Item being used.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>True if it can be used.</returns>
        public override bool IsCompatible(YAPUSettings settings,
                                          TimeManager timeManager,
                                          MonsterInstance monsterInstance,
                                          Item item,
                                          PlayerCharacter playerCharacter) =>
            monsterInstance.FormData.Evolutions.Any(data => data.CanEvolveWhenTrading(monsterInstance,
                                                        timeManager.DayMoment,
                                                        null,
                                                        playerCharacter,
                                                        out _));

        /// <summary>
        /// Use on a monster instance.
        /// </summary>
        /// <param name="monsterInstance">Reference to that monster instance.</param>
        /// <param name="item"></param>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="experienceLookupTable">Reference to the experience look up table.</param>
        /// <param name="playerCharacter"></param>
        /// <param name="timeManager">Reference to the time manager.</param>
        /// <param name="evolutionManager">Reference to the evolution manager.</param>
        /// <param name="inputManager">Reference to the input manager.</param>
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
            if (!IsCompatible(settings, timeManager, monsterInstance, item, playerCharacter))
            {
                finished.Invoke(false);
                yield break;
            }

            bool consumeHeldItem = false;

            EvolutionData evolutionData =
                monsterInstance.FormData.Evolutions.First(data => data.CanEvolveWhenTrading(monsterInstance,
                                                              timeManager.DayMoment,
                                                              null,
                                                              playerCharacter,
                                                              out consumeHeldItem));

            bool playerCancelled = false;

            yield return evolutionManager.TriggerEvolution(monsterInstance,
                                                           evolutionData,
                                                           consumeHeldItem,
                                                           playerCharacter,
                                                           didCancel => playerCancelled = didCancel);

            TransitionManager.BlackScreenFadeOut();
            if (!playerCancelled) playerCharacter.GlobalGridManager.PlayCurrentSceneMusic();

            inputManager.BlockInput(false);

            finished.Invoke(!playerCancelled);
        }
    }
}