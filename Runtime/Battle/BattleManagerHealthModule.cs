using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Module class for the battle manager that handles stuff related to battler's health.
    /// </summary>
    public class BattleManagerHealthModule : BattleManagerModule<BattleManagerHealthModule>
    {
        /// <summary>
        /// List of the monsters that have fainted.
        /// </summary>
        private readonly List<Battler> faintedBattlers = new();

        /// <summary>
        /// Reference to the audio when the monster gets damaged.
        /// </summary>
        [SerializeField]
        private AudioReference DamageAudio;

        /// <summary>
        /// Check the fainted battlers that are fighting and withdraw them.
        /// </summary>
        /// <returns></returns>
        public IEnumerator CheckFaintedBattlers()
        {
            foreach (BattlerType battlerType in Utils.GetAllItems<BattlerType>())
                yield return CheckFaintedBattlers(battlerType,
                                                  fainted =>
                                                      faintedBattlers
                                                         .Add(Battlers.GetBattlerFromBattleIndex(battlerType,
                                                                  fainted)));
        }

        /// <summary>
        /// Check the fainted battlers of a type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="fainted">Callback when a fainted battler is found.</param>
        private IEnumerator CheckFaintedBattlers(BattlerType type, Action<int> fainted)
        {
            for (int i = 0; i < Battlers.GetNumberOfBattlers(); i++)
            {
                int iCopy = i;

                yield return CheckFaintedBattler(type,
                                                 i,
                                                 isFainted =>
                                                 {
                                                     if (isFainted) fainted?.Invoke(iCopy);
                                                 });
            }
        }

        /// <summary>
        /// Check if a battler is fainted and withdraw.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="inBattleIndex">In battle index.</param>
        /// <param name="isFaintedCallback">Returns true if it is fainted.</param>
        /// <returns></returns>
        private IEnumerator CheckFaintedBattler(BattlerType type,
                                                int inBattleIndex,
                                                Action<bool> isFaintedCallback)
        {
            Battler battler = Battlers.GetBattlerFromBattleIndex(type, inBattleIndex);

            // Already accounted for.
            if (faintedBattlers.Contains(battler) || Rosters.DefeatedTeams.Contains((type, inBattleIndex))) yield break;

            foreach (SideStatus status in Statuses.GetSideStatuses(type).Select(slot => slot.Key))
                yield return status.OnCheckFaintedBattler(type, inBattleIndex, BattleManager);

            yield return Statuses.TriggerStatusRemoval();

            if (battler.CanBattle)
            {
                isFaintedCallback?.Invoke(false);
                yield break;
            }

            isFaintedCallback?.Invoke(true);

            yield return BattleManagerBattlerSwitch.WithdrawBattler(type, inBattleIndex, true);

            foreach (Battler other in BattleManager.Battlers.GetBattlersFighting())
            {
                yield return other.OnOtherBattlerFainted(type, inBattleIndex, BattleManager);

                (BattlerType otherType, int otherIndex) = Battlers.GetTypeAndIndexOfBattler(other);

                if (otherType == BattlerType.Enemy
                 || otherIndex > Battlers.GetNumberOfBattlersUnderPlayersControl() - 1)
                    AI.GetAI(otherType, otherIndex)
                      .OnBattlerFainted(BattleManager, otherType, otherIndex, type, inBattleIndex);
            }

            yield return battler.OnFainted(BattleManager, type, inBattleIndex);

            Rosters.HadABattlerFaintThisTurn.Add((type, Battlers.GetTypeAndRosterIndexOfBattler(battler).RosterIndex));

            if (AI.PlayerControlsFirstRoster && type == BattlerType.Enemy) yield return Battlers.YieldXPAndEV(battler);

            if (Rosters.AreAllBattlersOfTypeFainted(type)) BattleManager.IsBattleOver = true;
        }

        /// <summary>
        /// For each fainted battler, switch them if necessary.
        /// </summary>
        public IEnumerator ProcessFaintedBattlers()
        {
            foreach (Battler faintedBattler in faintedBattlers)
            {
                (BattlerType type, int inBattleIndex) = Battlers.GetTypeAndIndexOfBattler(faintedBattler);

                if (inBattleIndex == -1) continue;

                int newBattler = -2;

                if (AI.PlayerControlsFirstRoster
                 && type == BattlerType.Ally
                 && inBattleIndex < Battlers.GetNumberOfBattlersUnderPlayersControl())
                {
                    // If battling a wild monster, ask if want to run.

                    bool playerWantsToRun = false;
                    bool chosen = false;

                    if (BattleManager.CanPlayerRun)
                    {
                        DialogManager.ShowChoiceMenu(new List<string>
                                                     {
                                                         "Common/True",
                                                         "Common/False"
                                                     },
                                                     index =>
                                                     {
                                                         playerWantsToRun = index == 1;
                                                         chosen = true;
                                                     },
                                                     onBackCallback: () =>
                                                                     {
                                                                         playerWantsToRun = true;
                                                                         chosen = true;
                                                                     },
                                                     character: null,
                                                     showDialog: true,
                                                     localizationKey: "Battle/OptionalRun");

                        yield return new WaitUntil(() => chosen);

                        if (playerWantsToRun) yield return Battlers.RunAway(BattlerType.Ally, 0, false);

                        if (BattleManager.IsBattleOver) yield break;
                    }

                    //If controlled by player, request the player to send a new one.

                    BattleManager.PlayerControlManager.RequestNewMonster(BattleManager, index => newBattler = index);

                    yield return new WaitUntil(() => newBattler != -2);
                }
                else
                {
                    newBattler = AI.GetAI(type, inBattleIndex)
                                   .RequestNewMonster(BattleManager.YAPUSettings,
                                                      BattleManager,
                                                      type,
                                                      inBattleIndex,
                                                      faintedBattlers);

                    if (newBattler != -1
                     && BattleManager.BattleType == BattleType.SingleBattle
                     && BattleManager.Configuration.BattleStyle == BattleStyle.Switch
                     && Rosters.CanPlayerChangeBattlers())
                    {
                        bool chosen = false;
                        bool playerWantsToSwitch = false;
                        int newPlayerIndex = -1;

                        DialogManager.ShowChoiceMenu(new List<string>
                                                     {
                                                         "Common/True",
                                                         "Common/False"
                                                     },
                                                     index =>
                                                     {
                                                         if (index == 0)
                                                         {
                                                             playerWantsToSwitch = true;

                                                             BattleManager.PlayerControlManager
                                                                          .RequestNewMonster(BattleManager,
                                                                               playerChoice =>
                                                                               {
                                                                                   newPlayerIndex = playerChoice;
                                                                                   chosen = true;
                                                                               },
                                                                               true);
                                                         }
                                                         else
                                                             chosen = true;
                                                     },
                                                     onBackCallback: () => chosen = true,
                                                     character: null,
                                                     showDialog: true,
                                                     localizationKey: "Battle/SwapMon/Enemy/AboutToChange",
                                                     localizableModifiers: true,
                                                     modifiers: new[]
                                                                {
                                                                    Battlers
                                                                       .GetBattlerFromRosterAndIndex(BattlerType
                                                                               .Enemy,
                                                                            inBattleIndex,
                                                                            newBattler)
                                                                       .Species.LocalizableName,
                                                                    Characters.EnemyCharacters[inBattleIndex]
                                                                              .LocalizableName
                                                                });

                        yield return new WaitUntil(() => chosen);

                        if (playerWantsToSwitch && newPlayerIndex != -1) // -1 = Canceled.
                            yield return BattleManagerBattlerSwitch.SwitchBattler(BattlerType.Ally,
                                0,
                                newPlayerIndex);
                    }
                }

                if (newBattler == -1
                 || !Battlers.GetBattlerFromRosterAndIndex(type, inBattleIndex, newBattler).CanBattle)
                {
                    Logger.Info("No new battler to be sent.");
                    Rosters.DefeatedTeams.Add((type, inBattleIndex));
                    continue;
                }

                yield return BattleManagerBattlerSwitch.SwitchBattler(type, inBattleIndex, newBattler, false);
            }

            faintedBattlers.Clear();

            // Check if the 1 in the 1v2 has revived a monster.
            if (BattleManager.BattleType != BattleType.DoubleBattle || Rosters.AllyRosters.Count != 1) yield break;

            for (int i = 0; i < 2; ++i)
                if (Rosters.DefeatedTeams.Contains((BattlerType.Ally, i)))
                {
                    List<Battler> list = Rosters.GetRoster(BattlerType.Ally, i);

                    for (int j = 0; j < list.Count; j++)
                    {
                        Battler battler = list[j];

                        if (!battler.CanBattle || (Battlers.GetBattlersFighting(BattlerType.Ally).Contains(battler)))
                            continue;

                        Rosters.DefeatedTeams.Remove((BattlerType.Ally, i));
                        yield return BattleManagerBattlerSwitch.SwitchBattler(BattlerType.Ally, i, j, false, true);
                    }
                }
        }

        /// <summary>
        /// Change the life of a battler.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">Index of the battler.</param>
        /// <param name="amount">Amount to change.</param>
        [HideInEditorMode]
        [FoldoutGroup("Debug")]
        [Button("Change life")]
        private void TestChangeLife(BattlerType type, int index, int amount) =>
            StartCoroutine(ChangeLife(type, index, type, index, amount));

        /// <summary>
        /// Change the life of a battler.
        /// </summary>
        /// <param name="battler">Target.</param>
        /// <param name="user">Battler forcing the change.</param>
        /// <param name="amount">Amount to change.</param>
        /// <param name="userMove">Move that caused the damage.</param>
        /// <param name="playAudio">Play an audio?</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <param name="isSecondaryDamage">Is this damage caused by a secondary effect?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        /// <param name="finished">Event raised when finished telling the amount changed and if the substitute took the hit.</param>
        public IEnumerator ChangeLife(Battler battler,
                                      Battler user,
                                      int amount,
                                      Move userMove = null,
                                      bool playAudio = true,
                                      bool forceSurvive = false,
                                      bool isSecondaryDamage = false,
                                      bool ignoreAbilities = false,
                                      Action<int, bool> finished = null)
        {
            (BattlerType targetType, int targetIndex) = Battlers.GetTypeAndIndexOfBattler(battler);
            (BattlerType userType, int userIndex) = Battlers.GetTypeAndIndexOfBattler(user);

            yield return ChangeLife(targetType,
                                    targetIndex,
                                    userType,
                                    userIndex,
                                    amount,
                                    userMove,
                                    playAudio,
                                    forceSurvive,
                                    isSecondaryDamage,
                                    ignoreAbilities,
                                    finished);
        }

        /// <summary>
        /// Change the life of a battler.
        /// </summary>
        /// <param name="battler">Reference to the battler.</param>
        /// <param name="userType">Type of the origin of the life change.</param>
        /// <param name="userIndex">Index of the origin of the life change.</param>
        /// <param name="amount">Amount to change.</param>
        /// <param name="userMove">Move that inflicted the damage.</param>
        /// <param name="playAudio">Play an animation audio?</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <param name="isSecondaryDamage">Is this damage caused by a secondary effect?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        /// <param name="finished">Event raised when finished telling the amount changed and if the substitute took the hit.</param>
        public IEnumerator ChangeLife(Battler battler,
                                      BattlerType userType,
                                      int userIndex,
                                      int amount,
                                      Move userMove = null,
                                      bool playAudio = true,
                                      bool forceSurvive = false,
                                      bool isSecondaryDamage = false,
                                      bool ignoreAbilities = false,
                                      Action<int, bool> finished = null)
        {
            (BattlerType type, int rosterIndex, int battlerIndex) =
                Battlers.GetTypeAndRosterIndexOfBattler(battler);

            yield return ChangeLife(type,
                                    rosterIndex,
                                    battlerIndex,
                                    userType,
                                    userIndex,
                                    userMove,
                                    amount,
                                    playAudio,
                                    forceSurvive,
                                    isSecondaryDamage,
                                    ignoreAbilities,
                                    finished);
        }

        /// <summary>
        /// Change the life of a battler.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="rosterIndex">Index of the roster.</param>
        /// <param name="battlerIndex">Index of the battler inside the roster.</param>
        /// <param name="userType">Type of the origin of the life change.</param>
        /// <param name="userIndex">Index of the origin of the life change.</param>
        /// <param name="userMove">Move that inflicted the damage.</param>
        /// <param name="amount">Amount to change.</param>
        /// <param name="playAudio">Play an animation audio?</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <param name="isSecondaryDamage">Is this damage caused by a secondary effect?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        /// <param name="finished">Event raised when finished telling the amount changed and if the substitute took the hit.</param>
        private IEnumerator ChangeLife(BattlerType type,
                                       int rosterIndex,
                                       int battlerIndex,
                                       BattlerType userType,
                                       int userIndex,
                                       Move userMove,
                                       int amount,
                                       bool playAudio,
                                       bool forceSurvive,
                                       bool isSecondaryDamage,
                                       bool ignoreAbilities,
                                       Action<int, bool> finished)
        {
            int index = Battlers.RosterAndIndexToInBattleIndex(type, rosterIndex, battlerIndex);

            if (index == -1 || !Battlers.IsBattlerFighting(type, index))
            {
                Battler battler = Battlers.GetBattlerFromRosterAndIndex(type, rosterIndex, battlerIndex);

                int currentHP = 0;
                int previousHP = 0;
                bool substituteTookHit = false;

                yield return battler.ChangeHPInBattle(amount,
                                                      BattleManager,
                                                      type,
                                                      index,
                                                      userType,
                                                      userIndex,
                                                      isSecondaryDamage,
                                                      ignoreAbilities,
                                                      userMove,
                                                      (newHP, prevHP, substituteWasEnabled) =>
                                                      {
                                                          currentHP = newHP;
                                                          previousHP = prevHP;
                                                          substituteTookHit = substituteWasEnabled;
                                                      },
                                                      forceSurvive);

                finished?.Invoke(currentHP - previousHP, substituteTookHit);
            }
            else
                yield return ChangeLife(type,
                                        index,
                                        userType,
                                        userIndex,
                                        amount,
                                        userMove,
                                        playAudio,
                                        forceSurvive,
                                        isSecondaryDamage,
                                        ignoreAbilities,
                                        finished);
        }

        /// <summary>
        /// Change the life of a battler.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">Index of the battler.</param>
        /// <param name="userType">Type of the origin of the life change.</param>
        /// <param name="userIndex">Index of the origin of the life change.</param>
        /// <param name="amount">Amount to change.</param>
        /// <param name="userMove">Move that caused the damage.</param>
        /// <param name="playAudio">Play an audio?</param>
        /// <param name="forceSurvive">Force the monster to survive to the hit?</param>
        /// <param name="isSecondaryDamage">Is this damage caused by a secondary effect?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        /// <param name="finished">Event raised when finished telling the amount changed and if the substitute took the hit.</param>
        public IEnumerator ChangeLife(BattlerType type,
                                      int index,
                                      BattlerType userType,
                                      int userIndex,
                                      int amount,
                                      Move userMove = null,
                                      bool playAudio = true,
                                      bool forceSurvive = false,
                                      bool isSecondaryDamage = false,
                                      bool ignoreAbilities = false,
                                      Action<int, bool> finished = null)
        {
            bool substituteTookHit = false;

            yield return new WaitForSeconds(.5f);

            // TODO: Move this to the monsters animator and improve it.
            if (playAudio && amount < 0)
            {
                BattleManager.AudioManager.PlayAudio(DamageAudio, pitch: BattleManager.BattleSpeed);
                yield return new WaitForSeconds(.5f);
            }

            Battler battler = Battlers.GetBattlerFromBattleIndex(type, index);

            MonsterPanel panel = type switch
            {
                BattlerType.Ally => BattleManager.AllyPanels[index],
                BattlerType.Enemy => BattleManager.EnemyPanels[index],
                _ => null
            };

            if (panel == null) yield break;

            int currentHP = 0;
            int previousHP = 0;

            yield return battler.ChangeHPInBattle(amount,
                                                  BattleManager,
                                                  type,
                                                  index,
                                                  userType,
                                                  userIndex,
                                                  isSecondaryDamage,
                                                  ignoreAbilities,
                                                  userMove,
                                                  (newHP, prevHP, substituteWasEnabled) =>
                                                  {
                                                      currentHP = newHP;
                                                      previousHP = prevHP;
                                                      substituteTookHit = substituteWasEnabled;
                                                  },
                                                  forceSurvive);

            int finalAmount = currentHP - previousHP;

            bool animationFinished = false;

            panel.UpdatePanel(BattleManager.BattleSpeed,
                              playLowHealthSound: type == BattlerType.Ally,
                              tween: finalAmount != 0,
                              finished: () => animationFinished = true);

            yield return new WaitUntil(() => animationFinished);

            finished?.Invoke(finalAmount, substituteTookHit);
        }
    }
}