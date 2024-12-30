using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Module class for the battle manager that handles stuff related to battler switching.
    /// </summary>
    public class BattleManagerBattlerSwitchModule : BattleManagerModule<BattleManagerBattlerSwitchModule>
    {
        /// <summary>
        /// Flag structure that stores if a specific Side and Index have their battler currently in the field.
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        private SerializableDictionary<(BattlerType, int), bool> battlerInFieldFlags = new();

        /// <summary>
        /// Queue of battlers waiting to trigger their Enter The Battlefield trigger.
        /// This is needed because these only trigger when all battlers are in so that effects like Intimidate make sense.
        /// </summary>
        private readonly Queue<Battler> battlersWaitingForETBTrigger = new();

        /// <summary>
        /// Check if there are battlers waiting to enter the battle field.
        /// </summary>
        [ShowInInspector]
        [HideInEditorMode]
        public bool AreThereBattlersWaitingToEnter
        {
            get
            {
                if (!Application.isPlaying) return false;

                for (int i = 0; i < Battlers.GetNumberOfBattlers(); ++i)
                {
                    if (!DoesRosterHaveBattlerInField(BattlerType.Ally, i)
                     && !Rosters.AreAllBattlersInRosterFainted(BattlerType.Ally, i)
                     && (i >= Battlers.GetNumberOfBattlersUnderPlayersControl()
                      || Rosters.GetNumberNotFainted(BattlerType.Ally, i)
                      >= Battlers.GetNumberOfBattlersUnderPlayersControl()))
                        return true;

                    if (!DoesRosterHaveBattlerInField(BattlerType.Enemy, i)
                     && !Rosters.AreAllBattlersInRosterFainted(BattlerType.Enemy, i))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Does the given roster have a battler in field.
        /// </summary>
        /// <param name="type">Type of the roster to check.</param>
        /// <param name="index">Index of that roster.</param>
        /// <returns>True if it has a battler in the field.</returns>
        public bool DoesRosterHaveBattlerInField(BattlerType type, int index) =>
            battlerInFieldFlags.ContainsKey((type, index)) && battlerInFieldFlags[(type, index)];

        /// <summary>
        /// Register the wild battlers as in the field.
        /// </summary>
        public IEnumerator RegisterWildsAsInField()
        {
            for (int i = 0; i < Battlers.GetNumberOfBattlers(); ++i)
            {
                battlerInFieldFlags[(BattlerType.Enemy, i)] = true;

                yield return
                    ProcessOrEnqueueETB(Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, i));
            }
        }

        /// <summary>
        /// Function for testing swapping out the ally battler.
        /// </summary>
        /// <param name="type">Type of battler to swap.</param>
        /// <param name="index">Index of the roster to swap.</param>
        /// <param name="newBattler">New battler to enter.</param>
        [Button("Switch battler")]
        [HideInEditorMode]
        [FoldoutGroup("Debug")]
        private void TestSwitchBattler(BattlerType type, int index = 0, int newBattler = 1) =>
            StartCoroutine(SwitchBattler(type, index, newBattler));

        /// <summary>
        /// Switch a battler for a new one.
        /// </summary>
        /// <param name="type">Type of battler to swap.</param>
        /// <param name="index">In battle index of the battler to swap.</param>
        /// <param name="newBattler">Index of the new battler.</param>
        /// <param name="withdrawAnimation">Play animation when withdrawing?</param>
        /// <param name="forceSameBattler">Force to send the same battler again.</param>
        public IEnumerator SwitchBattler(BattlerType type,
                                         int index,
                                         int newBattler,
                                         bool withdrawAnimation = true,
                                         bool forceSameBattler = false)
        {
            (int rosterIndex, int battlerIndex) = Rosters.InBattleIndexToRosterIndex(type, index);

            if (newBattler == battlerIndex && !forceSameBattler)
            {
                Logger.Error("That's the same battler, you idiot.");
                yield break;
            }

            List<Battler> roster = Rosters.GetRoster(type, rosterIndex);

            if (roster.Count <= newBattler)
            {
                Logger.Error("The index for the battler is way too high.");
                yield break;
            }

            if (withdrawAnimation) yield return WithdrawBattler(type, index);

            Battler battler = Battlers.GetBattlerFromRosterAndIndex(type, rosterIndex, battlerIndex);

            yield return battler.OnMonsterLeavingBattle(BattleManager);

            foreach (Battler other in Battlers.GetBattlersFighting())
                yield return other.OnOtherMonsterLeavingBattle(battler, BattleManager);

            (roster[newBattler], roster[battlerIndex]) = (roster[battlerIndex], roster[newBattler]);

            // Keep track of the first roster if it belongs to the player.
            if (type == BattlerType.Ally
             && rosterIndex == 0
             && index < Battlers.GetNumberOfBattlersUnderPlayersControl())
                (Rosters.FirstRosterIndexes[newBattler], Rosters.FirstRosterIndexes[battlerIndex]) =
                    (Rosters.FirstRosterIndexes[battlerIndex], Rosters.FirstRosterIndexes[newBattler]);

            yield return SendBattlerIn(type, index);

            Rosters.UpdateRosterIndicators();
        }

        /// <summary>
        /// Force a battler to be switched out.
        /// </summary>
        /// <param name="type">Battler type to switch out.</param>
        /// <param name="index">In battle index of the battler.</param>
        /// <param name="userType">Type of the battle forcing the switch.</param>
        /// <param name="userIndex">Type of the battler forcing the switch.</param>
        /// <param name="userMove">Move used to force the switch, if there is any.</param>
        /// <param name="item">Item used to force the switch, if there is any.</param>
        /// <param name="itemBelongsToUser">Does the item used to force the switch belong to the user?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        /// <param name="showMessages">Show can't switch messages?</param>
        /// <param name="onIndexChosen">Action called when an index has been chosen but before actually switching.</param>
        public IEnumerator ForceSwitchBattler(BattlerType type,
                                              int index,
                                              BattlerType userType,
                                              int userIndex,
                                              Move userMove,
                                              Item item,
                                              bool itemBelongsToUser,
                                              bool ignoreAbilities,
                                              bool showMessages,
                                              Action<int> onIndexChosen = null)
        {
            Battler battler = Battlers.GetBattlerFromBattleIndex(type, index);

            if (!battler.CanSwitch(BattleManager,
                                   userType,
                                   userIndex,
                                   userMove,
                                   ignoreAbilities,
                                   item,
                                   itemBelongsToUser,
                                   showMessages))
            {
                yield return DialogManager.WaitForDialog;
                yield break;
            }

            int newBattler = -2;

            if (type == BattlerType.Ally
             && index <= Battlers.GetNumberOfBattlersUnderPlayersControl() - 1)
            {
                BattleManager.PlayerControlManager.RequestNewMonster(BattleManager, newIndex => newBattler = newIndex);
                yield return new WaitUntil(() => newBattler != -2);
            }
            else
                newBattler = AI.GetAI(type, index)
                               .RequestNewMonster(BattleManager.YAPUSettings,
                                                  BattleManager,
                                                  type,
                                                  index,
                                                  new List<Battler> {battler});

            if (newBattler == -1)
            {
                Logger.Error("No new battler to be sent. Whoever called this method must make sure first that a new monster can be sent in!");
                yield break;
            }

            onIndexChosen?.Invoke(newBattler);

            yield return BattleManagerBattlerSwitch.SwitchBattler(type, index, newBattler);
        }

        /// <summary>
        /// Send in the battlers of a type.
        /// </summary>
        /// <param name="type">Type of battler to send.</param>
        internal IEnumerator SendBattlersIn(BattlerType type)
        {
            switch (BattleManager.BattleType)
            {
                case BattleType.SingleBattle: yield return SendBattlerIn(type, 0); break;
                case BattleType.DoubleBattle:
                    StartCoroutine(SendBattlerIn(type, 0));
                    yield return SendBattlerIn(type, 1);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Send a new battler in.
        /// </summary>
        private IEnumerator SendBattlerIn(BattlerType type, int index)
        {
            MonsterPanel panel = Battlers.GetPanel(type, index);
            BattleMonsterSprite sprite = BattleManager.GetMonsterSprite(type, index);

            panel.SlideOut(true);

            string dialogKey = type switch
            {
                BattlerType.Ally => "Battle/SwapMon/Ally/Enter",
                BattlerType.Enemy => "Battle/SwapMon/Enemy/Enter",
                _ => throw new BattleManager.UnsupportedBattlerException(type)
            };

            Battler battler = Battlers.GetBattlerFromBattleIndex(type, index);

            panel.SetMonsterInBattle(battler, BattleManager, type, type == BattlerType.Ally);
            sprite.SetMonster(battler, type == BattlerType.Enemy, BattleManager);

            DialogManager.ShowDialog(dialogKey,
                                     acceptInput: false,
                                     localizableModifiers: false,
                                     modifiers: new[]
                                                {
                                                    battler.GetNameOrNickName(BattleManager.Localizer),
                                                    Characters.GetCharacter(type, index)
                                                              .GetLocalizedFullName(BattleManager.Localizer)
                                                },
                                     switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            switch (type)
            {
                case BattlerType.Ally: AllyBattlerInAnimation(index); break;

                case BattlerType.Enemy: EnemyBattlerInAnimation(index); break;

                default: throw new BattleManager.UnsupportedBattlerException(type);
            }

            yield return DialogManager.WaitForDialog;

            panel.SlideIn();

            battlerInFieldFlags[(type, index)] = true;

            Battlers.UpdateBattlersFought();

            bool belongsToPlayer = type == BattlerType.Ally
                                && AI.PlayerControlsFirstRoster
                                && index < Battlers.GetNumberOfBattlersUnderPlayersControl();

            if (belongsToPlayer && !Rosters.PlayerBattlersThatHaveFought.Contains(battler))
                Rosters.PlayerBattlersThatHaveFought.Add(battler);

            BattleManager.Dex.RegisterAsSeen(battler, true, belongsToPlayer);

            yield return ProcessOrEnqueueETB(battler);
        }

        /// <summary>
        /// Enqueue an ETB trigger if not all battlers are in and process all when all battlers are in.
        /// </summary>
        /// <param name="battler">Battler to enqueue or process.</param>
        private IEnumerator ProcessOrEnqueueETB(Battler battler)
        {
            battlersWaitingForETBTrigger.Enqueue(battler);

            if (AreThereBattlersWaitingToEnter) yield break;

            // TODO: Should this queue be reordered in speed order?

            while (battlersWaitingForETBTrigger.TryDequeue(out Battler waitingBattler))
            {
                Logger.Info("Processing "
                          + waitingBattler.GetNameOrNickName(BattleManager.Localizer)
                          + "'s ETB triggers.");

                yield return waitingBattler.OnMonsterEnteredBattle(BattleManager);

                (BattlerType battlerType, int battlerIndex) = Battlers.GetTypeAndIndexOfBattler(waitingBattler);

                yield return Statuses.OnBattlerEnteredSide(battlerType, battlerIndex);

                // Clean up statuses.
                yield return Statuses.TriggerStatusRemoval();
            }
        }

        /// <summary>
        /// Play the animation of an ally battle entering the battle.
        /// </summary>
        /// <param name="index">Index of the roster to use.</param>
        private void AllyBattlerInAnimation(int index) =>
            BattleManager.AllyTrainerSprites[index]
                         .ThrowAndSlide(BattleManager.BattleSpeed,
                                        () => BattleManager.AllyBattlerSprites[index]
                                                           .DropBallAndEnlarge(BattleManager.BattleSpeed,
                                                                               () =>
                                                                                   // ReSharper disable once WrongIndentSize
                                                                               {
                                                                                   Audio.PlayCry(BattlerType.Ally,
                                                                                       index);
                                                                               }));

        /// <summary>
        /// Play the animation of an enemy battle entering the battle.
        /// </summary>
        /// <param name="index">Index of the roster to use.</param>
        private void EnemyBattlerInAnimation(int index)
        {
            BattleManager.EnemyTrainerSprites[index].Slide(BattleManager.BattleSpeed);

            BattleManager.EnemyBattlerSprites[index]
                         .DropBallAndEnlarge(BattleManager.BattleSpeed,
                                             () =>
                                             {
                                                 Audio.PlayCry(BattlerType.Enemy, index);
                                                 BattleManager.EnemyPanels[index].SlideIn();
                                             });
        }

        /// <summary>
        /// Withdraw a battler.
        /// </summary>
        internal IEnumerator WithdrawBattler(BattlerType type, int index, bool fainted = false)
        {
            battlerInFieldFlags[(type, index)] = false;

            Battler battler = Battlers.GetBattlerFromBattleIndex(type, index);
            MonsterPanel panel = Battlers.GetPanel(type, index);
            BattleMonsterSprite battlerSprite = BattleManager.GetMonsterSprite(type, index);

            string dialogKey = type switch
            {
                BattlerType.Ally => "Battle/SwapMon/Ally/Exit",
                BattlerType.Enemy => "Battle/SwapMon/Enemy/Exit",
                _ => throw new BattleManager.UnsupportedBattlerException(type)
            };

            panel.SlideOut();

            yield return new WaitForSeconds(.5f / BattleManager.BattleSpeed);

            if (fainted)
                DialogManager.ShowDialog("Battle/Fainted",
                                         acceptInput: false,
                                         localizableModifiers: false,
                                         modifiers: battler.GetNameOrNickName(BattleManager.Localizer),
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);
            else
                DialogManager.ShowDialog(dialogKey,
                                         acceptInput: false,
                                         localizableModifiers: false,
                                         modifiers: new[]
                                                    {
                                                        battler.GetNameOrNickName(BattleManager.Localizer),
                                                        Characters.GetCharacter(type, index)
                                                                  .GetLocalizedFullName(BattleManager.Localizer)
                                                    },
                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            Audio.PlayCry(type, index);

            yield return new WaitForSeconds(2f / BattleManager.BattleSpeed);

            bool finished = false;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            if (BattleManager.EnemyType == EnemyType.Wild && type == BattlerType.Enemy)
                battlerSprite.Shrink(BattleManager.BattleSpeed,
                                     false,
                                     () =>
                                     {
                                         finished = true;
                                     });
            else
                battlerSprite.ShrinkAndCloseBall(BattleManager.BattleSpeed,
                                                 () =>
                                                 {
                                                     finished = true;
                                                 });

            yield return new WaitUntil(() => finished);

            battlerSprite.ResetSpritePosition();
            Rosters.UpdateRosterIndicators();

            yield return DialogManager.WaitForDialog;
        }
    }
}