using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.Player;
using Varguiniano.YAPU.Runtime.UI;
using Varguiniano.YAPU.Runtime.UI.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Behaviours;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle.PlayerControl
{
    /// <summary>
    /// Battle behaviour that requests actions to the player.
    /// </summary>
    public class PlayerControlManager : WhateverBehaviour<PlayerControlManager>,
                                        IPlayerControlManager,
                                        IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the main menu selector.
        /// </summary>
        [FoldoutGroup("Scene References")]
        [SerializeField]
        private MenuSelector MainMenuSelector;

        /// <summary>
        /// Reference to the move menu.
        /// </summary>
        [FoldoutGroup("Scene References")]
        [SerializeField]
        private BattleMoveMenuSelector BattleMoveMenuSelector;

        /// <summary>
        /// Reference to the monsters menu.
        /// </summary>
        [FoldoutGroup("Scene References")]
        [SerializeField]
        private BattleMonstersMenu BattleMonstersMenu;

        /// <summary>
        /// Reference to the targets selector for double battles.
        /// </summary>
        [FoldoutGroup("Scene References")]
        [SerializeField]
        private TargetMonstersMenuSelector TargetsSelector;

        /// <summary>
        /// Reference to the last ball menu.
        /// </summary>
        [FoldoutGroup("Scene References")]
        [SerializeField]
        private LastBallMenu LastBallMenu;

        /// <summary>
        /// Reference to the first ally panel.
        /// </summary>
        [FoldoutGroup("Scene References")]
        [SerializeField]
        public MonsterPanel[] AllyPanels;

        /// <summary>
        /// In battle index of the current battler.
        /// </summary>
        public int CurrentBattlerInBattleIndex { get; private set; }

        /// <summary>
        /// Callback for when the action is chosen.
        /// </summary>
        private Action<BattleAction> callback;

        /// <summary>
        /// Flag to know if an action has been decided.
        /// </summary>
        private bool actionDecided;

        /// <summary>
        /// Callback when a replacement monster has been chosen.
        /// </summary>
        private Action<int> replacementMonsterChosen;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        private BattleManager battleManager;

        /// <summary>
        /// Current battler requesting input.
        /// </summary>
        private Battler currentBatter;

        /// <summary>
        /// Flag to know when to listen to the move menu or when to sent back an action when the moves button in the main is selected.
        /// Useful por repeat moves or struggle.
        /// </summary>
        private bool listeningToMoveMenu;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Subscribe the events that can always be on.
        /// </summary>
        private void OnEnable() => LastBallMenu.BallSelected += OnBallSelected;

        /// <summary>
        /// Unsubscribe the events that can always be on.
        /// </summary>
        private void OnDisable() => LastBallMenu.BallSelected -= OnBallSelected;

        /// <summary>
        /// Request the player to choose their next action.
        /// </summary>
        /// <param name="battleManagerReference">Reference to the battle manager.</param>
        /// <param name="index">Index of the monster that is requesting the action.</param>
        /// <param name="finished">Event with the chosen battle action.</param>
        /// <param name="allowGoBack">Allow to go back to the previous monster. Useful for double battles.</param>
        /// <param name="goBackCallback">Callback when going back.</param>
        public IEnumerator RequestAction(BattleManager battleManagerReference,
                                         int index,
                                         Action<BattleAction> finished,
                                         bool allowGoBack,
                                         Action goBackCallback)
        {
            actionDecided = false;
            callback = finished;

            battleManager = battleManagerReference;

            if (battleManager.BattleType == BattleType.DoubleBattle) AllyPanels[index].StartBouncing();

            currentBatter = battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, index);
            CurrentBattlerInBattleIndex = index;

            MainMenuSelector.UpdateLayout(new List<bool>
                                          {
                                              battleManager.IsFightAvailable,
                                              battleManager.IsMonstersMenuAvailable,
                                              battleManager.IsBagAvailable,
                                              battleManager.CanPlayerRun
                                          });

            // Make sure no shenanigans happen.
            MainMenuSelector.OnButtonSelected -= OnMainMenuSelected;
            MainMenuSelector.OnButtonSelected += OnMainMenuSelected;

            if (allowGoBack)
            {
                MainMenuSelector.PlayAudioOnBack = true;

                MainMenuSelector.OnBackSelected += () =>
                                                   {
                                                       MainMenuSelector.Show(false);

                                                       goBackCallback?.Invoke();

                                                       actionDecided = true;
                                                   };
            }
            else
            {
                MainMenuSelector.PlayAudioOnBack = false;
                MainMenuSelector.OnBackSelected = null;
            }

            UpdateMoveMenu();

            (int rosterIndex, int _) =
                battleManager.Rosters.InBattleIndexToRosterIndex(BattlerType.Ally,
                                                                 CurrentBattlerInBattleIndex);

            BattleMonstersMenu.SetMonsters(battleManager.Rosters.GetRoster(BattlerType.Ally,
                                                                           rosterIndex));

            BattleMonstersMenu.MonsterToSwitchChosen = OnSwitchMonsterChosen;

            BattleMonstersMenu.MenuClosed +=
                UpdateMoveMenu; // Call this because the player may have changed the move order.

            MainMenuSelector.Show();

            yield return new WaitUntil(() => actionDecided);

            AllyPanels[index].StopBouncing();
        }

        /// <summary>
        /// Request the player to choose a new monster to send.
        /// </summary>
        /// <param name="battleManagerReference">Reference to the battle manager.</param>
        /// <param name="finished">Callback when finished.</param>
        /// <param name="menuCanBeClosed">Whether the change is obligatory, useful for changing after fainting.</param>
        public void RequestNewMonster(BattleManager battleManagerReference,
                                      Action<int> finished,
                                      bool menuCanBeClosed = false)
        {
            replacementMonsterChosen = finished;
            battleManager = battleManagerReference;

            List<Battler> roster = battleManager.Rosters.GetRoster(BattlerType.Ally, 0);

            if (!battleManager.Rosters.CanPlayerChangeBattlers())
            {
                for (int i = 0; i < roster.Count; i++)
                    if (!battleManager.Battlers.IsBattlerFighting(BattlerType.Ally, i)
                     && roster[i].CanBattle)
                    {
                        finished?.Invoke(i);
                        return;
                    }

                finished?.Invoke(-1);
                return;
            }

            BattleMonstersMenu.SetMonsters(roster);

            BattleMonstersMenu.MonsterToSwitchChosen = OnReplacementChosen;

            if (menuCanBeClosed)
                BattleMonstersMenu.MenuClosed += () =>
                                                 {
                                                     UpdateMoveMenu(); // Call this because the player may have changed the move order.
                                                     BattleMonstersMenu.MenuClosed = null;
                                                     finished?.Invoke(-1);
                                                 };

            BattleMonstersMenu.OpenMenu(canMenuBeClosed: menuCanBeClosed, openMainMenuOnClose: false);
        }

        /// <summary>
        /// Release the input of the menus.
        /// </summary>
        public void ReleaseInput() => MainMenuSelector.ReleaseInput();

        /// <summary>
        /// Called when the main menu is selected.
        /// </summary>
        /// <param name="index">The index of the chose option.</param>
        private void OnMainMenuSelected(int index)
        {
            switch (index)
            {
                case 0:

                    if (listeningToMoveMenu) break;

                    List<MoveSlot> usableMoves = currentBatter.GetUsableMoves(battleManager);

                    switch (usableMoves.Count)
                    {
                        case 1:
                            OnMoveChosen(currentBatter.GetMoveIndex(usableMoves[0].Move));
                            break;
                        case 0:
                            OnMoveChosen(4);
                            break;
                    }

                    MainMenuSelector.OnBackSelected = null;

                    break;

                case 1:
                    BattleMonstersMenu.OpenMenu(currentBatter);

                    break;

                case 2:

                    DialogManager.ShowBag((_, _) => MainMenuSelector.Show(),
                                          battleManager: battleManager,
                                          currentBattlerIndex: CurrentBattlerInBattleIndex,
                                          overrideDisplayed: true,
                                          displayOverride: new[]
                                                           {
                                                               // Only the first 4 are displayed in battle.
                                                               true, true, true, true, false, false, false, false, false
                                                           },
                                          onBattleItemSelectedCallback: action =>
                                                                        {
                                                                            callback?.Invoke(action);

                                                                            actionDecided = true;
                                                                            currentBatter = null;
                                                                        });

                    break;

                case 3:

                    if (!battleManager.Battlers
                                      .GetBattlerFromBattleIndex(BattlerType.Ally,
                                                                 CurrentBattlerInBattleIndex)
                                      .CanRunAway(battleManager, false, true))
                    {
                        StartCoroutine(ShowMainMenuWhenDialogFinishes());

                        break;
                    }

                    callback?.Invoke(new BattleAction
                                     {
                                         BattlerType = BattlerType.Ally,
                                         Index = CurrentBattlerInBattleIndex,
                                         ActionType = BattleAction.Type.Run
                                     });

                    actionDecided = true;
                    currentBatter = null;

                    break;
            }
        }

        /// <summary>
        /// Called when a move is chosen.
        /// </summary>
        /// <param name="index">Index of the chosen move.</param>
        private void OnMoveChosen(int index)
        {
            BattleMoveMenuSelector.OnButtonSelected -= OnMoveChosen;

            StartCoroutine(OnMoveChosenRoutine(index));
        }

        /// <summary>
        /// Routine to run when a move has been chosen.
        /// </summary>
        /// <param name="index">Index of the chosen move.</param>
        private IEnumerator OnMoveChosenRoutine(int index)
        {
            yield return WaitAFrame;

            Move move;

            if (index == 4 || currentBatter.CurrentMoves[index].CurrentPP == 0)
            {
                index = 4;
                move = settings.NoPPMove;
            }
            else
                move = currentBatter.CurrentMoves[index].Move;

            List<int> parameters = new() {index};

            if (BattleUtils.TryAutoGenerateMoveTargets(battleManager,
                                                       move,
                                                       BattlerType.Ally,
                                                       CurrentBattlerInBattleIndex,
                                                       out List<int> autoParameters))
                parameters.AddRange(autoParameters);
            else
            {
                Logger.Info("Couldn't auto generate target for move "
                          + move.LocalizableName
                          + ". Calculating possible targets.");

                Battler[] targets = // I don't like this at all.
                {
                    battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, 0),
                    battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, 1),
                    battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, 0),
                    battleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, 1)
                };

                bool[] valid =
                    MoveUtils.GenerateValidTargetsArray(battleManager.BattleType,
                                                        move,
                                                        targets,
                                                        CurrentBattlerInBattleIndex);

                int validCount = valid.Count(isValid => isValid);

                switch (validCount)
                {
                    case 0:
                        Logger.Warn("No possible valid targets for "
                                  + move.LocalizableName
                                  + ". We will assign -1.");

                        parameters.Add(1);
                        parameters.Add(-1);
                        break;
                    case 1:

                        Logger.Info("There is only one possible target for "
                                  + move.LocalizableName
                                  + ". We will automatically assign it.");

                        int validTarget = valid.IndexOf(true);

                        parameters.Add(validTarget < 2 ? 1 : 0);
                        parameters.Add(validTarget % 2);

                        break;
                    default:

                        List<float> effectiveness = new();

                        for (int i = 0; i < valid.Length; i++)
                        {
                            Battler target = targets[i];

                            if (valid[i])
                            {
                                target.GetEffectivenessOfMove(currentBatter,
                                                              move,
                                                              false,
                                                              battleManager,
                                                              false,
                                                              out float effectivenessValue);

                                effectiveness.Add(effectivenessValue);
                            }
                            else
                                effectiveness.Add(0);
                        }

                        TargetsSelector.SetMonsters(targets, battleManager, valid, effectiveness.ToArray());

                        int targetIndex = -2;

                        bool goBack = false;

                        TargetsSelector.OnButtonSelected += chosenTarget =>
                                                            {
                                                                TargetsSelector.OnButtonSelected = null;
                                                                TargetsSelector.OnBackSelected = null;

                                                                targetIndex = chosenTarget;
                                                            };

                        TargetsSelector.OnBackSelected += () =>
                                                          {
                                                              TargetsSelector.OnButtonSelected = null;
                                                              TargetsSelector.OnBackSelected = null;

                                                              targetIndex = -1;
                                                              goBack = true;
                                                          };

                        AllyPanels[CurrentBattlerInBattleIndex].StopBouncing();

                        Logger.Info("Requesting the player to choose a target for move "
                                  + move.LocalizableName
                                  + ".");

                        TargetsSelector.Show();

                        yield return new WaitUntil(() => targetIndex != -2);

                        if (goBack)
                        {
                            BattleMoveMenuSelector.OnButtonSelected += OnMoveChosen;

                            AllyPanels[CurrentBattlerInBattleIndex].StartBouncing();

                            TargetsSelector.Show(false);
                            BattleMoveMenuSelector.Show();
                            yield break;
                        }

                        parameters.Add(targetIndex < 2 ? 1 : 0);
                        parameters.Add(targetIndex % 2);
                        break;
                }
            }

            callback?.Invoke(new BattleAction
                             {
                                 BattlerType = BattlerType.Ally,
                                 Index = CurrentBattlerInBattleIndex,
                                 ActionType = BattleAction.Type.Move,
                                 TriggerMegaForm = BattleMoveMenuSelector.ShouldMegaevolve,
                                 Parameters = parameters.ToArray()
                             });

            actionDecided = true;
            currentBatter = null;
        }

        /// <summary>
        /// Called when the player chooses to switch to another monster.
        /// </summary>
        /// <param name="index">Index of the monster to switch to.</param>
        private void OnSwitchMonsterChosen(int index)
        {
            MainMenuSelector.Show(false);

            BattleMonstersMenu.MonsterToSwitchChosen -= OnSwitchMonsterChosen;

            callback?.Invoke(new BattleAction
                             {
                                 BattlerType = BattlerType.Ally,
                                 Index = CurrentBattlerInBattleIndex,
                                 ActionType = BattleAction.Type.Switch,
                                 Parameters =
                                     new[] {index}
                             });

            actionDecided = true;
            currentBatter = null;
        }

        /// <summary>
        /// Called when a replacement monster has been chosen after a fainting.
        /// </summary>
        /// <param name="index">Index of the chosen monster.</param>
        private void OnReplacementChosen(int index)
        {
            replacementMonsterChosen?.Invoke(index);
            replacementMonsterChosen = null;
        }

        /// <summary>
        /// Called when a ball is selected through the last ball menu.
        /// </summary>
        /// <param name="ball"></param>
        private void OnBallSelected(Ball ball)
        {
            callback?.Invoke(new BattleAction
                             {
                                 BattlerType = BattlerType.Ally,
                                 Index = CurrentBattlerInBattleIndex,
                                 ActionType = BattleAction.Type.Item,
                                 Parameters =
                                     new[] {0, playerBag.GetIndexOfItem(ball)}
                             });

            actionDecided = true;
            currentBatter = null;
        }

        /// <summary>
        /// Update the moves menu.
        /// </summary>
        private void UpdateMoveMenu()
        {
            if (currentBatter == null) return;

            BattleMoveMenuSelector.CanMegaevolve = battleManager.Megas.CanMegaevolve(currentBatter);

            Battler firstEnemy =
                battleManager.Battlers.GetBattlerFromRosterAndIndex(BattlerType.Enemy, 0, 0);

            List<bool> useEffectiveness = battleManager.BattleType == BattleType.SingleBattle
                                              ? new List<bool>()
                                              : new List<bool>
                                                {
                                                    false,
                                                    false,
                                                    false,
                                                    false
                                                };

            List<float> effectiveness = new();

            bool canUseAtLeastOneMove = false;
            List<MoveSlot> usableMoves = currentBatter.GetUsableMoves(battleManager);

            foreach (MoveSlot move in currentBatter.CurrentMoves)
            {
                if (battleManager.BattleType == BattleType.SingleBattle)
                {
                    useEffectiveness.Add(firstEnemy.GetEffectivenessOfMove(currentBatter,
                                                                           move.Move,
                                                                           false,
                                                                           battleManager,
                                                                           false,
                                                                           out float effectivenessValue));

                    effectiveness.Add(effectivenessValue);
                }

                if (usableMoves.Contains(move)) canUseAtLeastOneMove = true;
            }

            if (canUseAtLeastOneMove)
            {
                BattleMoveMenuSelector.SetMoves(currentBatter.CurrentMoves,
                                                currentBatter,
                                                useEffectiveness,
                                                effectiveness,
                                                battleManager.BattleType == BattleType.SingleBattle
                                                    ? firstEnemy
                                                    : null, // Show targeting in single battles.
                                                battleManager);

                BattleMoveMenuSelector.OnButtonSelected -= OnMoveChosen;
                BattleMoveMenuSelector.OnButtonSelected += OnMoveChosen;

                BattleMoveMenuSelector.ListenToMainMenu = true;
                listeningToMoveMenu = true;
            }
            else
            {
                listeningToMoveMenu = false;
                BattleMoveMenuSelector.ListenToMainMenu = false;
            }
        }

        /// <summary>
        /// Routine to show the main menu when the dialog finishes.
        /// </summary>
        private IEnumerator ShowMainMenuWhenDialogFinishes()
        {
            yield return DialogManager.WaitForDialog;

            MainMenuSelector.Show();
        }
    }
}