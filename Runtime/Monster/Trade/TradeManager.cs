using System.Collections;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster.Evolution;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Monster.Trade
{
    /// <summary>
    /// Controller for trading monsters.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Trade/Manager", fileName = "TradeManager")]
    public class TradeManager : WhateverScriptable<TradeManager>
    {
        /// <summary>
        /// Monster that it's about to be traded.
        /// </summary>
        [ReadOnly]
        public MonsterInstance MonsterToTrade;

        /// <summary>
        /// Monster to receive.
        /// </summary>
        [ReadOnly]
        public MonsterInstance NewMonster;

        /// <summary>
        /// Trainer the player is trading with.
        /// </summary>
        [ReadOnly]
        public CharacterData Recipient;

        /// <summary>
        /// Reference to the trade scene.
        /// </summary>
        [SerializeField]
        private SceneReference TradeScene;

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
        /// Reference to the evolution manager.
        /// </summary>
        [Inject]
        private EvolutionManager evolutionManager;

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
        /// Reference to the trade animation in the scene.
        /// </summary>
        private TradeAnimation tradeAnimation;

        /// <summary>
        /// Allow the trade animation to register itself to the manager.
        /// </summary>
        /// <param name="tradeAnimationReference">Reference to the animation.</param>
        public void RegisterAnimation(TradeAnimation tradeAnimationReference) =>
            tradeAnimation = tradeAnimationReference;

        /// <summary>
        /// Perform the trade of two monsters.
        /// </summary>
        /// <param name="monsterToSend">Monster to send.</param>
        /// <param name="monsterToReceive">Monster to receive.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="tradingPartner">Trainer to trade with.</param>
        /// <param name="isPartOfRoster">Is the monster part of the roster (or the storage)?</param>
        public IEnumerator TradeMonster(MonsterInstance monsterToSend,
                                        MonsterInstance monsterToReceive,
                                        PlayerCharacter playerCharacter,
                                        CharacterData tradingPartner,
                                        bool isPartOfRoster)
        {
            inputManager.BlockInput();

            tradeAnimation = null;

            MonsterToTrade = monsterToSend;
            NewMonster = monsterToReceive;
            Recipient = tradingPartner;

            yield return TransitionManager.BlackScreenFadeInRoutine();

            bool sceneLoaded = false;

            sceneManager.LoadScene(TradeScene, null, _ => sceneLoaded = true);

            yield return new WaitUntil(() => sceneLoaded);

            // By this point the animation reference has been assigned.
            // ReSharper disable once PossibleNullReferenceException
            yield return tradeAnimation.PlayTradeAnimation();

            if (isPartOfRoster)
                playerCharacter.PlayerRoster[playerCharacter.PlayerRoster.RosterData.IndexOf(MonsterToTrade)] =
                    NewMonster;
            else
            {
                playerCharacter.PlayerStorage.RemoveMonster(MonsterToTrade);
                playerCharacter.PlayerStorage.AddMonster(NewMonster);
            }

            bool sceneUnloaded = false;

            sceneManager.UnloadScene(TradeScene, null, _ => sceneUnloaded = true);

            yield return new WaitUntil(() => sceneUnloaded);

            if (NewMonster.FormData.Evolutions.Any(data => data.CanEvolveWhenTrading(NewMonster,
                                                       timeManager.DayMoment,
                                                       MonsterToTrade,
                                                       playerCharacter,
                                                       out bool consumeHeldItem)))
            {
                bool consumeHeldItem = false;

                EvolutionData evolutionData =
                    NewMonster.FormData.Evolutions.First(data => data.CanEvolveWhenTrading(NewMonster,
                                                             timeManager.DayMoment,
                                                             MonsterToTrade,
                                                             playerCharacter,
                                                             out consumeHeldItem));

                bool playerCancelled = false;

                yield return evolutionManager.TriggerEvolution(NewMonster,
                                                               evolutionData,
                                                               consumeHeldItem,
                                                               playerCharacter,
                                                               didCancel => playerCancelled = didCancel);

                TransitionManager.BlackScreenFadeOut();
                if (!playerCancelled) playerCharacter.GlobalGridManager.PlayCurrentSceneMusic();
                inputManager.BlockInput(false);
            }
            else
                playerCharacter.GlobalGridManager.PlayCurrentSceneMusic();

            MonsterToTrade = null;
            NewMonster = null;
            Recipient = null;
        }
    }
}