using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Controller for evolving monsters.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Evolution/Manager", fileName = "EvolutionManager")]
    public class EvolutionManager : WhateverScriptable<EvolutionManager>
    {
        /// <summary>
        /// Monster that it's about to evolve.
        /// </summary>
        [ReadOnly]
        public MonsterInstance MonsterToEvolve;

        /// <summary>
        /// New species of the monster.
        /// </summary>
        [ReadOnly]
        public MonsterEntry NewSpecies;

        /// <summary>
        /// New form of the monster.
        /// </summary>
        [ReadOnly]
        public Form NewForm;

        /// <summary>
        /// Reference to the evolution scene.
        /// </summary>
        [SerializeField]
        private SceneReference EvolutionScene;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject]
        private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the global grid manager.
        /// </summary>
        [Inject]
        private GlobalGridManager globalGridManager;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the YAPU database.
        /// </summary>
        [Inject]
        private MonsterDatabaseInstance database;

        /// <summary>
        /// Reference to the evolution animation in the scene.
        /// </summary>
        private EvolutionAnimation evolutionAnimation;

        /// <summary>
        /// Allow the evolution animation to register itself to the manager.
        /// </summary>
        /// <param name="evolutionAnimationReference">Reference to the animation.</param>
        public void RegisterAnimation(EvolutionAnimation evolutionAnimationReference) =>
            evolutionAnimation = evolutionAnimationReference;

        /// <summary>
        /// Trigger evolutions after the monsters have leveled up.
        /// </summary>
        /// <param name="monsters">Monster list.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="removeBlackScreenOnFinish">Remove the black screen when finished?</param>
        public IEnumerator TriggerEvolutionsAfterLevelUp(IEnumerable<MonsterInstance> monsters,
                                                         PlayerCharacter playerCharacter,
                                                         bool removeBlackScreenOnFinish)
        {
            bool playerCancelled = true;

            // ReSharper disable once WrongIndentSize
            List<MonsterInstance> monstersToCheck = monsters.Where(monster => monster is
                                                             {
                                                                 IsNullEntry: false,
                                                                 ExtraData:
                                                                 {
                                                                     NeedsLevelUpEvolutionCheck: true
                                                                 }
                                                             })
                                                            .ToList();

            foreach (MonsterInstance monster in monstersToCheck)
            {
                Logger.Info("Checking if " + monster.GetNameOrNickName(localizer) + " can evolve through level.");

                bool consumeHeldItem = false;

                foreach (EvolutionData evolutionData in monster.FormData.Evolutions.Where(evolutionData =>
                             evolutionData.CanEvolveAfterLevelUp(monster,
                                                                 timeManager.DayMoment,
                                                                 playerCharacter,
                                                                 out consumeHeldItem)))
                {
                    yield return TriggerEvolution(monster,
                                                  evolutionData,
                                                  consumeHeldItem,
                                                  playerCharacter,
                                                  didCancel => playerCancelled = didCancel);

                    break;
                }

                monster.ExtraData.NeedsLevelUpEvolutionCheck = false;
            }

            if (removeBlackScreenOnFinish && monstersToCheck.Count > 0)
            {
                TransitionManager.BlackScreenFadeOut();
                if (!playerCancelled) globalGridManager.PlayCurrentSceneMusic();
            }

            inputManager.BlockInput(false);
        }

        /// <summary>
        /// Trigger evolutions after battle by checking the extra data.
        /// </summary>
        public IEnumerator TriggerEvolutionAfterBattleThroughExtraData(IEnumerable<MonsterInstance> monsters,
                                                                       bool removeBlackScreenOnFinish,
                                                                       PlayerCharacter playerCharacter)
        {
            bool playerCancelled = true;

            // ReSharper disable once WrongIndentSize
            List<MonsterInstance> monstersToCheck = monsters.Where(monster => monster is
                                                             {
                                                                 IsNullEntry: false
                                                             })
                                                            .ToList();

            foreach (MonsterInstance monster in monstersToCheck)
            {
                Logger.Info("Checking if " + monster.GetNameOrNickName(localizer) + " can evolve through extra data.");

                bool consumeHeldItem = false;

                foreach (EvolutionData evolutionData in monster.FormData.Evolutions.Where(evolutionData =>
                             evolutionData.CanEvolveAfterBattleThroughExtraData(monster,
                                                                                    timeManager.DayMoment,
                                                                                    out consumeHeldItem)))
                {
                    yield return TriggerEvolution(monster,
                                                  evolutionData,
                                                  consumeHeldItem,
                                                  playerCharacter,
                                                  didCancel => playerCancelled = didCancel);

                    monster.ExtraData.ResetEvolutionData();

                    break;
                }

                monster.ExtraData.ResetBattleData();
            }

            if (removeBlackScreenOnFinish && monstersToCheck.Count > 0)
            {
                TransitionManager.BlackScreenFadeOut();
                if (!playerCancelled) globalGridManager.PlayCurrentSceneMusic();
            }

            inputManager.BlockInput(false);
        }

        /// <summary>
        /// Trigger the evolution of a monster.
        /// </summary>
        /// <param name="monster">Monster to evolve.</param>
        /// <param name="evolutionData">Evolution data to use.</param>
        /// <param name="consumeHeldItem">Should the held item be consumed?</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="finished">Callback stating if the player cancelled.</param>
        public IEnumerator TriggerEvolution(MonsterInstance monster,
                                            EvolutionData evolutionData,
                                            bool consumeHeldItem,
                                            PlayerCharacter playerCharacter,
                                            Action<bool> finished)
        {
            Logger.Info("Triggering " + monster.GetNameOrNickName(localizer) + " evolution.");

            inputManager.BlockInput();

            evolutionAnimation = null;

            MonsterToEvolve = monster;
            (NewSpecies, NewForm) = evolutionData.GetTargetEvolution(monster, timeManager.DayMoment);

            yield return TransitionManager.BlackScreenFadeInRoutine();

            bool sceneLoaded = false;

            sceneManager.LoadScene(EvolutionScene, null, _ => sceneLoaded = true);

            yield return new WaitUntil(() => sceneLoaded);

            bool playerCancelled = false;

            // By this point the animation reference has been assigned.
            yield return
                // ReSharper disable once PossibleNullReferenceException
                evolutionAnimation.PlayEvolutionAnimation(cancelled => playerCancelled = cancelled);
            
            if (!playerCancelled && consumeHeldItem) monster.HeldItem = null;

            bool sceneUnloaded = false;

            sceneManager.UnloadScene(EvolutionScene, null, _ => sceneUnloaded = true);

            yield return new WaitUntil(() => sceneUnloaded);

            if (!playerCancelled)
                evolutionData.AfterEvolutionCallback(MonsterToEvolve, playerCharacter, settings, database, localizer);

            MonsterToEvolve = null;
            NewSpecies = null;
            NewForm = null;

            finished.Invoke(playerCancelled);
        }
    }
}