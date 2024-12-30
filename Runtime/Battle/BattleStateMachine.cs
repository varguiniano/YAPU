using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.PlayerControl;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using WhateverDevs.Core.Runtime.DataStructures;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Behaviour in charge of the battle state machine.
    /// </summary>
    public class BattleStateMachine : BattleManagerModule<BattleStateMachine>
    {
        /// <summary>
        /// Reference to the PlayerControlManager.
        /// </summary>
        [Inject]
        private IPlayerControlManager playerControlManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings settings;

        /// <summary>
        /// Starts the battle loop.
        /// </summary>
        public void StartBattleLoop() => StartCoroutine(BattleLoop());

        /// <summary>
        /// Battle loop routine.
        /// </summary>
        private IEnumerator BattleLoop()
        {
            Logger.Info("Battle loop started.");

            while (!BattleManager.IsBattleOver)
            {
                Logger.Info("Updating fought battlers.");
                Battlers.UpdateBattlersFought();

                Logger.Info("Requesting actions.");

                SerializableDictionary<Battler, BattleAction> actions = null;

                yield return RequestActions(result => actions = result);

                Logger.Info("All action requests received.");

                Logger.Info("Determining order.");

                Queue<Battler> order = null;
                SerializableDictionary<Battler, int> priorityDictionary = null;

                yield return DetermineActionOrder(actions,
                                                  (newOrder, newPriorityDictionary) =>
                                                  {
                                                      order = newOrder;
                                                      priorityDictionary = newPriorityDictionary;
                                                  });

                // Let the player think the AI is thinking very hard.
                yield return new WaitForSeconds(1f / BattleManager.BattleSpeed);

                yield return new WaitUntil(() => order != null);

                Logger.Info("Performing actions.");

                BattleManager.CurrentTurnActionOrder = order;
                BattleManager.CurrentTurnActions = actions;
                BattleManager.CurrentTurnPriorityBrackets = priorityDictionary;

                yield return PerformActions();

                Logger.Info("All actions processed.");

                if (BattleManager.IsBattleOver) break;

                Logger.Info("Post turn callbacks.");

                yield return AfterTurnPreStatusCallbacks(BattlerType.Ally);
                yield return AfterTurnPreStatusCallbacks(BattlerType.Enemy);

                yield return Statuses.TriggerStatusesCountdown();

                yield return AfterTurnPostStatusCallbacks(BattlerType.Ally);
                yield return AfterTurnPostStatusCallbacks(BattlerType.Enemy);

                yield return ResetBattlersTurnFlags(BattlerType.Ally);
                yield return ResetBattlersTurnFlags(BattlerType.Enemy);

                yield return BattlerHealth.CheckFaintedBattlers();
                yield return BattlerHealth.ProcessFaintedBattlers();

                BattleManager.Rosters.AfterTurnFaintedChecks();

                BattleManager.TickTurn();

                Logger.Info("End of the turn.");
            }

            Logger.Info("Battle has ended.");

            yield return Animation.BattleEndAnimation();

            yield return Characters.GivePlayerPriceMoney();

            yield return BattleEndCallbacks();

            BattleManager.FinishBattle();
        }

        /// <summary>
        /// Request actions to be taken this turn to the player and AIs.
        /// </summary>
        /// <returns>A dictionary of battlers and the actions they want to take.</returns>
        private IEnumerator RequestActions(Action<SerializableDictionary<Battler, BattleAction>> actionsChosenCallback)
        {
            SerializableDictionary<Battler, BattleAction> actions = new();

            for (int i = 0; i < Battlers.GetNumberOfBattlersUnderPlayersControl(); ++i)
            {
                if (!Battlers.IsBattlerFighting(BattlerType.Ally, i)) continue;

                Battler battler = Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, i);

                if (battler.RequestForcedAction(BattleManager, out BattleAction forcedAction))
                {
                    actions[battler] = forcedAction;
                    continue;
                }

                bool playerChose = false;
                bool goBack = false;

                int iCopy = i;

                yield return playerControlManager.RequestAction(BattleManager,
                                                                i,
                                                                action =>
                                                                {
                                                                    actions[battler] = action;

                                                                    playerChose = true;
                                                                },
                                                                iCopy == 1
                                                             && Battlers
                                                               .GetBattlersFighting(BattlerType.Ally)
                                                               .Count
                                                              > 1,
                                                                () =>
                                                                {
                                                                    actions.Remove(Battlers
                                                                       .GetBattlerFromBattleIndex(BattlerType
                                                                               .Ally,
                                                                            iCopy));

                                                                    goBack = true;

                                                                    playerChose = true;
                                                                });

                yield return new WaitUntil(() => playerChose);

                playerControlManager.ReleaseInput();

                if (goBack) i -= 2;
            }

            for (int i = 0; i < Battlers.GetNumberOfBattlers(); ++i)
            {
                if (i < Battlers.GetNumberOfBattlersUnderPlayersControl()) continue;

                if (!Battlers.IsBattlerFighting(BattlerType.Ally, i)) continue;

                Battler battler = Battlers.GetBattlerFromBattleIndex(BattlerType.Ally, i);

                if (battler.RequestForcedAction(BattleManager, out BattleAction forcedAction))
                {
                    actions[battler] = forcedAction;
                    continue;
                }

                yield return BattleManager.AI
                                          .AllyAIs[Mathf.Clamp(i - Battlers.GetNumberOfBattlersUnderPlayersControl(),
                                                               0,
                                                               Rosters.AllyRosters.Count - 1)]
                                          .RequestPerformAction(settings,
                                                                BattleManager,
                                                                BattlerType.Ally,
                                                                i,
                                                                newAction => actions.TryAdd(battler, newAction));
            }

            for (int i = 0; i < Battlers.GetNumberOfBattlers(); ++i)
            {
                if (!Battlers.IsBattlerFighting(BattlerType.Enemy, i)) continue;

                Battler battler = Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, i);

                if (battler.RequestForcedAction(BattleManager, out BattleAction forcedAction))
                {
                    actions[battler] = forcedAction;
                    continue;
                }

                yield return BattleManager
                            .AI.EnemyAIs[i]
                            .RequestPerformAction(settings,
                                                  BattleManager,
                                                  BattlerType.Enemy,
                                                  i,
                                                  newAction => actions.TryAdd(battler, newAction));
            }

            actionsChosenCallback?.Invoke(actions);
        }

        /// <summary>
        /// Determine the order of the actions to be taken in a turn.
        /// First we order by the priority of the action.
        /// Inside that, we order by the speed of each monster.
        /// If draw, we take a random order.
        /// </summary>
        /// <param name="actions">Actions that are going to be taken.</param>
        /// <param name="finished">Callback with list of battlers in the order they must act and the priority that was assigned to each battler.</param>
        private IEnumerator DetermineActionOrder(SerializableDictionary<Battler, BattleAction> actions,
                                                 Action<Queue<Battler>, SerializableDictionary<Battler, int>> finished)
        {
            SortedDictionary<int, List<Battler>> actionPriority = new(new DescendingComparer<int>());

            foreach ((Battler battler, BattleAction action) in actions)
            {
                int priority = BattleUtils.CalculatePriority(settings, BattleManager, battler, action);

                if (!actionPriority.ContainsKey(priority)) actionPriority[priority] = new List<Battler>();

                actionPriority[priority].Add(battler);
            }

            Queue<Battler> order = new();
            SerializableDictionary<Battler, int> priorityDictionary = new();

            foreach ((int priority, List<Battler> battlers) in actionPriority)
            {
                if (battlers.Count == 1)
                {
                    AddToOrder(battlers[0],
                               priority,
                               ref order,
                               ref priorityDictionary,
                               battlers,
                               actionPriority,
                               actions);

                    continue;
                }

                Dictionary<Battler, int> priorityModifiers = new();

                // If any monster has priority modifications given from callbacks, add them first.
                foreach (Battler battler in battlers)
                    yield return battler.OnDeterminePriority(BattleManager,
                                                             newPriority =>
                                                             {
                                                                 priorityModifiers[battler] = newPriority;
                                                             });

                AddBattlersInModifiedPriority(1,
                                              battlers,
                                              priorityModifiers,
                                              actionPriority,
                                              actions,
                                              ref order,
                                              ref priorityDictionary);

                AddBattlersInModifiedPriority(0,
                                              battlers,
                                              priorityModifiers,
                                              actionPriority,
                                              actions,
                                              ref order,
                                              ref priorityDictionary);

                AddBattlersInModifiedPriority(-1,
                                              battlers,
                                              priorityModifiers,
                                              actionPriority,
                                              actions,
                                              ref order,
                                              ref priorityDictionary);
            }

            finished.Invoke(order, priorityDictionary);
        }

        /// <summary>
        /// Add the battlers for a certain priority modifier.
        /// </summary>
        /// <param name="priority">The priority modifier to check.</param>
        /// <param name="battlers">The battlers in this battle.</param>
        /// <param name="priorityModifiers">The modifiers for each battler.</param>
        /// <param name="actionPriority">Established action priority.</param>
        /// <param name="actions">All the actions that are going to be taken.</param>
        /// <param name="order">The final order.</param>
        /// <param name="priorityDictionary">Used to store the final priorities for this turn.</param>
        private void AddBattlersInModifiedPriority(int priority,
                                                   List<Battler> battlers,
                                                   IReadOnlyDictionary<Battler, int> priorityModifiers,
                                                   SortedDictionary<int, List<Battler>> actionPriority,
                                                   SerializableDictionary<Battler, BattleAction> actions,
                                                   ref Queue<Battler> order,
                                                   ref SerializableDictionary<Battler, int> priorityDictionary)
        {
            bool orderInverted = false;

            foreach (GlobalStatus status in BattleManager.Statuses.GetGlobalStatuses().Select(slot => slot.Key))
                orderInverted |= status.DoesInvertPriorityBracketOrder(BattleManager);

            // Use the descending comparer unless the order is inverted.
            SortedDictionary<uint, List<Battler>> speeds =
                orderInverted
                    ? new SortedDictionary<uint, List<Battler>>()
                    : new SortedDictionary<uint, List<Battler>>(new DescendingComparer<uint>());

            foreach (Battler battler in battlers)
            {
                if (priority != priorityModifiers[battler]) continue;

                uint speed = (actions[battler] is {TriggerMegaForm: true, ActionType: BattleAction.Type.Move}
                           && Megas.CanMegaevolve(battler)
                                  ? battler.GetStatsForSpecificForm(BattleManager,
                                                                    battler.GetAvailableMegaForm(BattleManager))
                                  : battler.GetStats(BattleManager))[Stat.Speed];

                if (!speeds.ContainsKey(speed)) speeds[speed] = new List<Battler>();

                speeds[speed].Add(battler);
            }

            foreach (List<Battler> drawBattlers in speeds.Select(keyValuePair => keyValuePair.Value))
                while (drawBattlers.Count > 0)
                {
                    Battler drawnBattler = BattleManager.RandomProvider.RandomElement(drawBattlers);

                    AddToOrder(drawnBattler,
                               priority,
                               ref order,
                               ref priorityDictionary,
                               battlers,
                               actionPriority,
                               actions);

                    drawBattlers.Remove(drawnBattler);
                }
        }

        /// <summary>
        /// Add a battler to the order and perform callbacks.
        /// </summary>
        /// <param name="battler">Battler to add.</param>
        /// <param name="currentPriority">The priority this battler has.</param>
        /// <param name="order">Order of this turn.</param>
        /// <param name="priorityDictionary">Used to store the final priorities for this turn.</param>
        /// <param name="battlers">All battlers.</param>
        /// <param name="actionPriority">Established action priority.</param>
        /// <param name="actions">All actions.</param>
        private void AddToOrder(Battler battler,
                                int currentPriority,
                                ref Queue<Battler> order,
                                ref SerializableDictionary<Battler, int> priorityDictionary,
                                List<Battler> battlers,
                                SortedDictionary<int, List<Battler>> actionPriority,
                                SerializableDictionary<Battler, BattleAction> actions)
        {
            if (order.Contains(battler)) return;

            order.Enqueue(battler);
            priorityDictionary[battler] = currentPriority;

            foreach (KeyValuePair<int, List<Battler>> keyValuePair in actionPriority)
            {
                foreach (Battler otherBattler in keyValuePair.Value)
                {
                    BattleAction action = actions[otherBattler];

                    if (order.Contains(otherBattler) || action.ActionType != BattleAction.Type.Move) continue;

                    int moveIndex = action.Parameters[0];

                    if (moveIndex is < 0 or > 3) continue;

                    Move move = otherBattler.CurrentMoves[moveIndex].Move;

                    if (move == null) continue;

                    switch (move.OnActionAddedToOrder(otherBattler,
                                                      battler,
                                                      ref order,
                                                      battlers,
                                                      actions,
                                                      BattleManager))
                    {
                        case -1:

                            Logger.Info("Order overriden to add "
                                      + otherBattler.GetNameOrNickName(BattleManager.Localizer)
                                      + " using "
                                      + BattleManager.Localizer[move.LocalizableName]
                                      + " just before "
                                      + battler.GetNameOrNickName(BattleManager.Localizer)
                                      + ".");

                            Queue<Battler> newOrder = new();

                            // Remove the battler.
                            while (order.TryDequeue(out Battler dequeuedBattler))
                                if (dequeuedBattler != battler)
                                    newOrder.Enqueue(dequeuedBattler);

                            order = newOrder;

                            AddToOrder(otherBattler,
                                       currentPriority,
                                       ref order,
                                       ref priorityDictionary,
                                       battlers,
                                       actionPriority,
                                       actions);

                            AddToOrder(battler,
                                       currentPriority,
                                       ref order,
                                       ref priorityDictionary,
                                       battlers,
                                       actionPriority,
                                       actions);

                            break;

                        case 1:

                            Logger.Info("Order overriden to add "
                                      + otherBattler.GetNameOrNickName(BattleManager.Localizer)
                                      + " using "
                                      + BattleManager.Localizer[move.LocalizableName]
                                      + " just after "
                                      + battler.GetNameOrNickName(BattleManager.Localizer)
                                      + ".");

                            AddToOrder(otherBattler,
                                       currentPriority,
                                       ref order,
                                       ref priorityDictionary,
                                       battlers,
                                       actionPriority,
                                       actions);

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Perform the actions for the turn.
        /// </summary>
        private IEnumerator PerformActions()
        {
            foreach (KeyValuePair<Battler, BattleAction> pair in BattleManager.CurrentTurnActions)
                if (pair.Value is {TriggerMegaForm: true, ActionType: BattleAction.Type.Move}
                 && Megas.CanMegaevolve(pair.Key))
                    yield return Megas.TriggerMegaevolution(pair.Key);

            while (BattleManager.CurrentTurnActionOrder.TryDequeue(out Battler battler))
            {
                if (BattleManager.IsBattleOver) break;

                BattleAction action = BattleManager.CurrentTurnActions[battler];

                // The battler was changed, so ignore the action.
                if (Battlers.GetBattlerFromBattleIndex(action.BattlerType, action.Index) != battler)
                {
                    Logger.Info(action.BattlerType + " with index " + action.Index + " changed, ignoring action.");
                    continue;
                }

                // Fainted, ignore.
                if (!Battlers.IsBattlerFighting(action.BattlerType, action.Index))
                {
                    Logger.Info(action.BattlerType
                              + " with index "
                              + action.Index
                              + " is no longer battling, ignoring action.");

                    continue;
                }

                switch (action.ActionType)
                {
                    case BattleAction.Type.Move:
                        List<(BattlerType Type, int Index)> targets = new();

                        if (action.Parameters[0] < 5)
                        {
                            for (int i = 1; i < action.Parameters.Length; i += 2)
                                targets.Add(((BattlerType) action.Parameters[i], action.Parameters[i + 1]));

                            yield return BattleManager.Moves.PerformMove(action.BattlerType,
                                                                         action.Index,
                                                                         targets,
                                                                         action.Parameters[0]);
                        }
                        else // Actions that use other moves that are not in their current moves.
                        {
                            for (int i = 1; i < action.Parameters.Length; i += 2)
                                targets.Add(((BattlerType) action.Parameters[i], action.Parameters[i + 1]));

                            yield return BattleManager.Moves.ForcePerformMove(action.BattlerType,
                                                                              action.Index,
                                                                              targets,
                                                                              action.AdditionalParameters[0] as Move);
                        }

                        break;
                    case BattleAction.Type.Switch:

                        int index = action.Parameters[0];

                        List<Battler> roster = action.BattlerType == BattlerType.Ally
                                                   ? BattleManager.Rosters.AllyRosters[action.Index]
                                                   : BattleManager.Rosters.EnemyRosters[action.Index];

                        index = Mathf.Clamp(index, 0, roster.Count - 1);

                        yield return BattleManagerBattlerSwitch.SwitchBattler(action.BattlerType,
                                                                              action.Index,
                                                                              index);

                        break;
                    case BattleAction.Type.Item: yield return PerformUseItemAction(action); break;
                    case BattleAction.Type.Run:
                        yield return Battlers.RunAway(action.BattlerType, action.Index, true);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                battler.LastPerformedAction.LastAction = action.ActionType;

                Animation.UpdatePanels();
                yield return BattlerHealth.CheckFaintedBattlers();

                if (BattleManager.IsBattleOver) break;

                yield return Statuses.TriggerStatusRemoval();

                yield return PostActionCallbacks(BattlerType.Ally, action, battler);
                yield return PostActionCallbacks(BattlerType.Enemy, action, battler);
            }
        }

        /// <summary>
        /// Perform a battle action when it's using an item.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        private IEnumerator PerformUseItemAction(BattleAction action)
        {
            switch (action.Parameters[0])
            {
                case 0:
                    Logger.Info(action.BattlerType
                              + " with index "
                              + action.Index
                              + " wants to use item with index "
                              + action.Parameters[1]
                              + ".");

                    yield return BattleManager.Items.UseItem(action.BattlerType,
                                                             action.Index,
                                                             action.Parameters[1]);

                    break;

                case 1:
                    Logger.Info(action.BattlerType
                              + " with index "
                              + action.Index
                              + " wants to use item with index "
                              + action.Parameters[1]
                              + " on monster of type "
                              + (BattlerType) action.Parameters[2]
                              + ", roster "
                              + action.Parameters[3]
                              + " and index "
                              + action.Parameters[4]
                              + ".");

                    yield return BattleManager.Items.UseItemOnTarget(action.BattlerType,
                                                                     action.Index,
                                                                     action.Parameters[1],
                                                                     (BattlerType) action.Parameters[2],
                                                                     action.Parameters[3],
                                                                     action.Parameters[4]);

                    break;
                case 2:
                    Logger.Info(action.BattlerType
                              + " with index "
                              + action.Index
                              + " wants to use item with index "
                              + action.Parameters[1]
                              + " on move with index "
                              + action.Parameters[5]
                              + " of monster of type "
                              + (BattlerType) action.Parameters[2]
                              + ", roster "
                              + action.Parameters[3]
                              + " and index "
                              + action.Parameters[4]
                              + ".");

                    yield return BattleManager.Items.UseItemOnTargetMove(action.BattlerType,
                                                                         action.Index,
                                                                         action.Parameters[5],
                                                                         action.Parameters[1],
                                                                         (BattlerType) action.Parameters[2],
                                                                         action.Parameters[3],
                                                                         action.Parameters[4]);

                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Callbacks after an action has been performed.
        /// </summary>
        /// <param name="type">Type of battle to check.</param>
        /// <param name="action">Action that was performed.</param>
        /// <param name="user">User of the action.</param>
        private IEnumerator PostActionCallbacks(BattlerType type, BattleAction action, Battler user) =>
            Battlers.GetBattlersFighting(type)
                    .Select(battler => battler.AfterAction(action, user, BattleManager))
                     // ReSharper disable once NotDisposedResourceIsReturned
                    .GetEnumerator();

        /// <summary>
        /// Callbacks for the held items once after each turn before statuses have ticked.
        /// </summary>
        /// <param name="type">Type of battler to check.</param>
        private IEnumerator AfterTurnPreStatusCallbacks(BattlerType type)
        {
            foreach (Battler battler in Battlers.GetBattlersFighting(type))
            {
                yield return battler.AfterTurnPreStatus(BattleManager, BattleManager.Localizer);

                yield return BattlerHealth.CheckFaintedBattlers();
            }
        }

        /// <summary>
        /// Callbacks for the held items once after each turn after statuses have ticked.
        /// </summary>
        /// <param name="type">Type of battler to check.</param>
        private IEnumerator AfterTurnPostStatusCallbacks(BattlerType type)
        {
            foreach (Battler battler in Battlers.GetBattlersFighting(type))
            {
                yield return battler.AfterTurnPostStatus(BattleManager, BattleManager.Localizer);

                yield return BattlerHealth.CheckFaintedBattlers();
            }
        }

        /// <summary>
        /// Tick the turn for battlers so they reset flags.
        /// </summary>
        /// <param name="type">Type of battler to check.</param>
        private IEnumerator ResetBattlersTurnFlags(BattlerType type) =>
            // ReSharper disable once NotDisposedResourceIsReturned
            Battlers.GetBattlersFighting(type).Select(battler => battler.ResetTurnFlags(BattleManager)).GetEnumerator();

        /// <summary>
        /// Callbacks made when the battle has ended.
        /// </summary>
        private IEnumerator BattleEndCallbacks()
        {
            yield return Statuses.OnBattleEnded();

            yield return BattleEndCallbacks(BattlerType.Ally);
            yield return BattleEndCallbacks(BattlerType.Enemy);

            if (Capture.CapturedMonster != null) yield return Capture.CapturedMonster.OnBattleEnded(BattleManager);
        }

        /// <summary>
        /// Callbacks made for a side when the battle has ended.
        /// </summary>
        private IEnumerator BattleEndCallbacks(BattlerType side)
        {
            foreach (Battler battler in Battlers.GetBattlersFighting(side))
            {
                yield return battler.OnBattleEnded(BattleManager);

                (BattlerType type, int index) = Battlers.GetTypeAndIndexOfBattler(battler);

                if (type == BattlerType.Enemy
                 || index > Battlers.GetNumberOfBattlersUnderPlayersControl() - 1)
                    AI.GetAI(type, index)
                      .OnBattleEnded(type,
                                     index,
                                     BattleManager.PlayerLost ? BattlerType.Enemy : BattlerType.Ally,
                                     BattleManager);
            }
        }
    }
}