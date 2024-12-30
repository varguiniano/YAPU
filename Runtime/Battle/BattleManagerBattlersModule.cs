using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Module class for the battle manager that handles stuff related to battlers.
    /// </summary>
    public class BattleManagerBattlersModule : BattleManagerModule<BattleManagerBattlersModule>
    {
        /// <summary>
        /// Get a battler from their battle index.
        /// </summary>
        /// <param name="tuple">Type and index of the battler.</param>
        /// <returns></returns>
        public Battler GetBattlerFromBattleIndex((BattlerType Type, int Index) tuple) =>
            GetBattlerFromBattleIndex(tuple.Item1, tuple.Item2);

        /// <summary>
        /// Get a battler from their battle index.
        /// </summary>
        /// <param name="type">Type of battler to get.</param>
        /// <param name="battleIndex">Index in the battle.</param>
        /// <returns></returns>
        public Battler GetBattlerFromBattleIndex(BattlerType type, int battleIndex)
        {
            try
            {
                (int rosterIndex, int battlerIndex) = Rosters.InBattleIndexToRosterIndex(type, battleIndex);
                return GetBattlerFromRosterAndIndex(type, rosterIndex, battlerIndex);
            }
            catch
            {
                Logger.Error("No battler was found with type "
                           + type
                           + " and index "
                           + battleIndex
                           + ". Returning null.");

                return null;
            }
        }

        /// <summary>
        /// Reference to a battler.
        /// <param name="type">Type of battler to get.</param>
        /// <param name="index">Index of the roster to get.</param>
        /// <param name="battlerIndex">Index of the battler inside the roster.</param>
        /// </summary>
        public Battler GetBattlerFromRosterAndIndex(BattlerType type, int index, int battlerIndex) =>
            type switch
            {
                BattlerType.Ally => Rosters.AllyRosters[Mathf.Clamp(index, 0, Rosters.AllyRosters.Count - 1)]
                    [battlerIndex],
                BattlerType.Enemy => Rosters.EnemyRosters[index][battlerIndex],
                _ => throw new BattleManager.UnsupportedBattlerException(type)
            };

        /// <summary>
        /// Get the type and index of a battler.
        /// Only works with battlers that are fighting.
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <returns>The type and its in battle index.</returns>
        public (BattlerType Type, int Index) GetTypeAndIndexOfBattler(Battler battler)
        {
            for (int i = 0; i < GetNumberOfBattlers(); ++i)
                foreach (BattlerType battlerType in Utils.GetAllItems<BattlerType>())
                    if (GetBattlerFromBattleIndex(battlerType, i) == battler)
                        return (battlerType, i);

            Logger.Warn("This battler was not found fighting!");
            return (BattlerType.Ally, -1);
        }

        /// <summary>
        /// Get the type and roster index of a battler.
        /// Only works with battlers that are fighting.
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <returns>The type and its in battle index.</returns>
        public (BattlerType Type, int RosterIndex, int BattlerIndex) GetTypeAndRosterIndexOfBattler(Battler battler)
        {
            for (int i = 0; i < Rosters.AllyRosters.Count; i++)
            {
                for (int j = 0; j < Rosters.AllyRosters[i].Count; j++)
                    if (battler == Rosters.AllyRosters[i][j])
                        return (BattlerType.Ally, i, j);
            }

            for (int i = 0; i < Rosters.EnemyRosters.Count; i++)
            {
                for (int j = 0; j < Rosters.EnemyRosters[i].Count; j++)
                    if (battler == Rosters.EnemyRosters[i][j])
                        return (BattlerType.Enemy, i, j);
            }

            Logger.Error("Battler was not found!");

            return (BattlerType.Ally, -1, -1);
        }

        /// <summary>
        /// Translate roster index and battler index to in battle index.
        /// </summary>
        /// <param name="type">Type of battler to get.</param>
        /// <param name="index">Index of the roster to get.</param>
        /// <param name="battlerIndex">Index of the battler inside the roster.</param>
        /// <returns>The in battle index of that battler.</returns>
        public int RosterAndIndexToInBattleIndex(BattlerType type, int index, int battlerIndex)
        {
            (BattlerType _, int battleIndex) =
                GetTypeAndIndexOfBattler(GetBattlerFromRosterAndIndex(type, index, battlerIndex));

            return battleIndex;
        }

        /// <summary>
        /// Get the number of battlers on a front.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfBattlers() =>
            BattleManager.BattleType switch
            {
                BattleType.SingleBattle => 1,
                BattleType.DoubleBattle => 2,
                _ => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// Get the number of battlers that are under the control of the player.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfBattlersUnderPlayersControl()
        {
            if (!BattleManager.AI.PlayerControlsFirstRoster) return 0;

            return BattleManager.BattleType switch
            {
                BattleType.SingleBattle => 1,
                BattleType.DoubleBattle when Rosters.AllyRosters.Count == 1 => 2,
                BattleType.DoubleBattle when Rosters.AllyRosters.Count == 2 => 1,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Reference to a panel.
        /// </summary>
        public MonsterPanel GetPanel(BattlerType type, int index) =>
            type switch
            {
                BattlerType.Ally => BattleManager.AllyPanels?[index],
                BattlerType.Enemy => BattleManager.EnemyPanels?[index],
                _ => throw new BattleManager.UnsupportedBattlerException(type)
            };

        /// <summary>
        /// Check if a battler is currently fighting.
        /// </summary>
        /// <param name="tuple">Type and index of the battler.</param>
        /// <returns>True if it is currently fighting.</returns>
        public bool IsBattlerFighting((BattlerType, int) tuple) => IsBattlerFighting(tuple.Item1, tuple.Item2);

        /// <summary>
        /// Check if a battler is currently fighting.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="battlerIndex">Index of the battler.</param>
        /// <returns>True if it is currently fighting.</returns>
        public bool IsBattlerFighting(BattlerType type, int battlerIndex) =>
            BattleManager.BattleType switch
            {
                BattleType.SingleBattle => battlerIndex == 0
                                        && GetBattlerFromBattleIndex(type, battlerIndex).CanBattle
                                        && !Rosters.DefeatedTeams.Contains((type, battlerIndex)),
                BattleType.DoubleBattle => battlerIndex is 0 or 1
                                        && GetBattlerFromBattleIndex(type, battlerIndex).CanBattle
                                        && !Rosters.DefeatedTeams.Contains((type, battlerIndex)),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        /// <summary>
        /// Get the battlers of all rosters that are currently currently fighting.
        /// </summary>
        /// <returns>A list of battlers currently fighting.</returns>
        public List<Battler> GetBattlersFighting()
        {
            List<Battler> battlers = GetBattlersFighting(BattlerType.Ally);
            battlers.AddRange(GetBattlersFighting(BattlerType.Enemy));
            return battlers;
        }

        /// <summary>
        /// Get the battlers of all rosters of a type that are currently currently fighting.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <returns>A list of battlers currently fighting.</returns>
        public List<Battler> GetBattlersFighting(BattlerType type)
        {
            List<Battler> battlers = new();

            if (IsBattlerFighting(type, 0))
            {
                Battler battler = GetBattlerFromBattleIndex(type, 0);
                if (IsBattlerFighting(type, 0)) battlers.Add(battler);
            }

            if (BattleManager.BattleType == BattleType.DoubleBattle && IsBattlerFighting(type, 1))
                battlers.Add(GetBattlerFromBattleIndex(type, 1));

            return battlers;
        }

        /// <summary>
        /// Get the battlers of a roster that are currently not fighting.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">Index of the roster.</param>
        /// <returns>A list of battlers currently not fighting.</returns>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public List<Battler> GetBattlersNotFighting(BattlerType type, int index)
        {
            List<Battler> battlers = new();

            List<Battler> list = Rosters.GetRoster(type, index);

            for (int i = 0; i < list.Count; i++)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement

                // First index with more than 0 HP is fighting.
                if (i == 0 && list[i].CanBattle) continue;

                // If ally is controlling two battlers, second index with more than 0 HP is fighting.
                if (BattleManager.BattleType == BattleType.DoubleBattle
                 && i == 1
                 && list[i].CanBattle
                 && type == BattlerType.Ally
                 && Rosters.AllyRosters.Count == 1)
                    continue;

                battlers.Add(list[i]);
            }

            return battlers;
        }

        /// <summary>
        /// Update the battler fought by the enemies.
        /// </summary>
        public void UpdateBattlersFought()
        {
            foreach (Battler battler in GetBattlersFighting(BattlerType.Ally))
            {
                foreach (Battler enemy in GetBattlersFighting(BattlerType.Enemy)
                            .Where(enemy => !enemy.BattlersFought.Contains(battler)))
                    enemy.BattlersFought.Add(battler);
            }
        }

        /// <summary>
        /// Shrink and hide all the battlers.
        /// </summary>
        /// <param name="immediately">Immediate or tween?</param>
        internal void ShrinkAndHideBattlers(bool immediately = true)
        {
            for (int i = 0; i < BattleManager.EnemyBattlerSprites.Length; i++) ShrinkAndHideBattlers(i, immediately);
        }

        /// <summary>
        /// Shrink and hide all the battlers of given roster index.
        /// </summary>
        /// <param name="index">Index of the rosters to hide.</param>
        /// <param name="immediately">Immediate or tween?</param>
        private void ShrinkAndHideBattlers(int index, bool immediately = true)
        {
            BattleManager.AllyBattlerSprites[index].Shrink(BattleManager.BattleSpeed, immediately);
            BattleManager.EnemyBattlerSprites[index].Shrink(BattleManager.BattleSpeed, immediately);

            BattleManager.EnemyPanels[index].SlideOut(immediately);
            BattleManager.AllyPanels[index].SlideOut(immediately);
        }

        /// <summary>
        /// Set all monster data to sprites.
        /// </summary>
        internal void SetAllMonsterSprites()
        {
            SetBothMonsterSprites(0);

            if (BattleManager.BattleType == BattleType.DoubleBattle) SetBothMonsterSprites(1);
        }

        /// <summary>
        /// Set both monster data to sprites.
        /// <param name="index">Index of the rosters to set.</param>
        /// </summary>
        private void SetBothMonsterSprites(int index)
        {
            UpdateAllyMonsterSprite(index);

            UpdateEnemyMonsterSprite(index);
        }

        /// <summary>
        /// Change a battler to a new form.
        /// </summary>
        /// <param name="battler">Battler to change.</param>
        /// <param name="newForm">New form to adopt.</param>
        /// <param name="showAnimation">Show an animation when changing form?</param>
        /// <param name="dialogLocalizationKey">Dialog to use. Not needed if no animation.</param>
        public IEnumerator ChangeForm(Battler battler,
                                      Form newForm,
                                      bool showAnimation = true,
                                      string dialogLocalizationKey = "")
        {
            (BattlerType battlerType, int battlerIndex) = GetTypeAndIndexOfBattler(battler);
            yield return ChangeForm(battlerType, battlerIndex, newForm, showAnimation, dialogLocalizationKey);
        }

        /// <summary>
        /// Change a battler to a new form.
        /// </summary>
        /// <param name="battlerType">Battler type to change.</param>
        /// <param name="index">Battle index of the battler to change.</param>
        /// <param name="newForm">New form to adopt.</param>
        /// <param name="showAnimation">Show an animation when changing form?</param>
        /// <param name="dialogLocalizationKey">Dialog to use. Not needed if no animation.</param>
        public IEnumerator ChangeForm(BattlerType battlerType,
                                      int index,
                                      Form newForm,
                                      bool showAnimation = true,
                                      string dialogLocalizationKey = "")
        {
            yield return ChangeToSpeciesAndForm(battlerType,
                                                index,
                                                GetBattlerFromBattleIndex(battlerType, index).Species,
                                                newForm,
                                                showAnimation,
                                                dialogLocalizationKey);
        }

        /// <summary>
        /// Change a battler to a species and new form.
        /// </summary>
        /// <param name="battlerType">Battler type to change.</param>
        /// <param name="index">Battle index of the battler to change.</param>
        /// <param name="newSpecies">New species to change to.</param>
        /// <param name="newForm">New form to adopt.</param>
        /// <param name="showAnimation">Show an animation when changing form?</param>
        /// <param name="dialogLocalizationKey">Dialog to use. Not needed if no animation.</param>
        /// <param name="keepShinyIfItIs">Keep the monster shiny if it already is.</param>
        /// <param name="replaceHp">Replace the Hp using a percentage?</param>
        /// <param name="registerOnDex">Register the new form on the dex?</param>
        /// <param name="overrideCanChange">Override the can change check?</param>
        public IEnumerator ChangeToSpeciesAndForm(BattlerType battlerType,
                                                  int index,
                                                  MonsterEntry newSpecies,
                                                  Form newForm,
                                                  bool showAnimation = true,
                                                  string dialogLocalizationKey = "",
                                                  bool keepShinyIfItIs = true,
                                                  bool replaceHp = true,
                                                  bool registerOnDex = true,
                                                  bool overrideCanChange = false)
        {
            Battler battler = GetBattlerFromBattleIndex(battlerType, index);

            if (keepShinyIfItIs && battler.Form.IsShiny && !newForm.IsShiny && newForm.HasShinyVersion)
                newForm = newForm.ShinyVersion;

            if (battler.Species == newSpecies && battler.Form == newForm)
            {
                Logger.Error("Battler is already of that form.");
                yield break;
            }

            if (!overrideCanChange && !battler.CanChangeForm(BattleManager))
            {
                Logger.Error("Battler can't change forms.");
                yield break;
            }

            if (!newSpecies.IsFormAvailable(newForm))
            {
                Logger.Error("This battler cannot adopt this form.");
                yield break;
            }

            foreach (VolatileStatus status in battler.VolatileStatuses.Select(slot => slot.Key))
                if (status.RemovedOnFormChange)
                    Statuses.ScheduleRemoveStatus(status, battler);

            yield return Statuses.TriggerStatusRemoval();

            battler.EvolveToSpeciesAndForm(newSpecies, newForm, false, replaceHp);

            if (showAnimation) yield return Animation.PlayFormChangeAnimation(battlerType, index);

            if (registerOnDex)
            {
                if (battlerType == BattlerType.Ally && index < GetNumberOfBattlersUnderPlayersControl())
                    BattleManager.Dex.RegisterAsCaught(battler, true, true, false);
                else
                    BattleManager.Dex.RegisterAsSeen(battler, true, false);
            }

            if (showAnimation && !dialogLocalizationKey.IsNullEmptyOrWhiteSpace())
                yield return DialogManager.ShowDialogAndWait(dialogLocalizationKey,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(BattleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);
        }

        /// <summary>
        /// Transform into the given target.
        /// </summary>
        /// <param name="battlerType">Battler type to change.</param>
        /// <param name="index">Battle index of the battler to change.</param>
        /// <param name="targetType">Type of the battler to transform into.</param>
        /// <param name="targetIndex">Index of the battler to transform into.</param>
        /// <param name="transformedStatus">Reference to the transformed status.</param>
        /// <param name="showAnimation">Show an animation when changing form?</param>
        /// <returns></returns>
        public IEnumerator TransformIntoTarget(BattlerType battlerType,
                                               int index,
                                               BattlerType targetType,
                                               int targetIndex,
                                               Transformed transformedStatus,
                                               bool showAnimation = true)
        {
            Battler battler = GetBattlerFromBattleIndex(battlerType, index);
            Battler target = GetBattlerFromBattleIndex(targetType, targetIndex);

            if (!battler.CanTransform(BattleManager))
            {
                Logger.Error("Can't transform.");
                yield break;
            }

            TransformationData transformationData = new()
                                                    {
                                                        Name = battler.GetNameOrNickName(BattleManager.Localizer),
                                                        Stats = target.GetStats(BattleManager),
                                                        Types = target.GetTypes(BattleManager.YAPUSettings),
                                                        CatchRate = battler.GetCatchRateInBattle(BattleManager),
                                                        Weight = target.GetWeight(BattleManager, false),
                                                        Height = target.GetHeight(BattleManager, false),
                                                    };

            // Keep the original max HP.
            transformationData.Stats[Stat.Hp] = battler.GetStats(BattleManager)[Stat.Hp];

            yield return ChangeToSpeciesAndForm(battlerType,
                                                index,
                                                target.Species,
                                                target.Form,
                                                showAnimation,
                                                keepShinyIfItIs: false,
                                                replaceHp: false,
                                                registerOnDex: false);

            battler.SetAbility(target.GetAbility());
            battler.StatStage = target.StatStage.ShallowClone();
            battler.BattleStatStage = target.BattleStatStage.ShallowClone();
            battler.CriticalStage = target.CriticalStage;
            battler.PhysicalData.Gender = target.PhysicalData.Gender;

            for (int i = 0; i < target.CurrentMoves.Length; i++)
            {
                MoveSlot slot = target.CurrentMoves[i];

                battler.TemporaryReplaceMove(i, slot.Move, true, 5, 5);
            }

            yield return BattleManager.Statuses.AddStatus(transformedStatus,
                                                          -1,
                                                          battlerType,
                                                          index,
                                                          battlerType,
                                                          index,
                                                          false,
                                                          transformationData);
        }

        /// <summary>
        /// Update a monster sprite.
        /// <param name="battlerType">Type of battler.</param>
        /// <param name="index">Index of the rosters to set.</param>
        /// </summary>
        public void UpdateMonsterSprite(BattlerType battlerType, int index)
        {
            switch (battlerType)
            {
                case BattlerType.Ally: UpdateAllyMonsterSprite(index); break;
                case BattlerType.Enemy: UpdateEnemyMonsterSprite(index); break;
                default: throw new BattleManager.UnsupportedBattlerException(battlerType);
            }
        }

        /// <summary>
        /// Update a monster sprite of an ally.
        /// <param name="index">Index of the rosters to set.</param>
        /// </summary>
        private void UpdateAllyMonsterSprite(int index) =>
            BattleManager.AllyBattlerSprites[index]
                         .SetMonster(GetBattlerFromBattleIndex(BattlerType.Ally, index), false, BattleManager);

        /// <summary>
        /// Update a monster sprite of an enemy.
        /// <param name="index">Index of the rosters to set.</param>
        /// </summary>
        private void UpdateEnemyMonsterSprite(int index) =>
            BattleManager.EnemyBattlerSprites[index]
                         .SetMonster(GetBattlerFromBattleIndex(BattlerType.Enemy, index), true, BattleManager);

        /// <summary>
        /// Set all enemy panels and sprites.
        /// </summary>
        internal void SetEnemyPanelsAndSprites()
        {
            SetEnemyPanelsAndSprites(0);

            if (BattleManager.BattleType == BattleType.DoubleBattle)
                SetEnemyPanelsAndSprites(1);
            else
                BattleManager.EnemyTrainerSprites[1].Hide();
        }

        /// <summary>
        /// Set the enemy panels and sprites.
        /// It will directly show the monster if it is wild or the trainer if not.
        /// <param name="index">Index of the rosters to set.</param>
        /// </summary>
        private void SetEnemyPanelsAndSprites(int index)
        {
            BattleManager.EnemyPanels[index]
                         .SetMonsterInBattle(GetBattlerFromBattleIndex(BattlerType.Enemy, index),
                                             BattleManager,
                                             BattlerType.Enemy,
                                             false);

            if (BattleManager.EnemyType != EnemyType.Wild) return;

            BattleManager.EnemyTrainerSprites[index].Hide();
            BattleManager.EnemyBattlerSprites[index].Enlarge(BattleManager.BattleSpeed, true);
            BattleManager.EnemyPanels[index].SlideIn(true);

            DOVirtual.DelayedCall(1f, () => BattleManager.EnemyBattlerSprites[index].ShinnyEffects());
        }

        /// <summary>
        /// Hide the allied trainer if this is not a double battle with an ally.
        /// </summary>
        internal void HideAllyIfNotAlliedDoubleBattle()
        {
            if (!(BattleManager.BattleType == BattleType.DoubleBattle && Rosters.AllyRosters.Count == 2))
                BattleManager.AllyTrainerSprites[1].Hide();
        }

        /// <summary>
        /// Yield XP and EV after a battler has fainted.
        /// This should only be called if the fainted battler is an enemy.
        /// </summary>
        /// <param name="yieldingBattler">The battler that is yielding the xp and ev.</param>
        /// <param name="yieldEv">Should this battler yield evs?</param>
        internal IEnumerator YieldXPAndEV(Battler yieldingBattler, bool yieldEv = true)
        {
            List<uint> amounts = new();
            List<List<MonsterInstance.LevelUpData>> levelUps = new();

            foreach (Battler battler in Rosters.AllyRosters[0])
            {
                uint yield =
                    (uint) MonsterMathHelper.CalculateXPYield(BattleManager.YAPUSettings,
                                                              BattleManager.PlayerSettings,
                                                              BattleManager.EnemyType,
                                                              yieldingBattler,
                                                              battler,
                                                              BattleManager);

                amounts.Add(yield);

                List<MonsterInstance.LevelUpData> monsterLevelUps = new();

                battler.RaiseExperience(BattleManager.ExperienceLookupTable,
                                        yield,
                                        data => monsterLevelUps.Add(data));

                levelUps.Add(monsterLevelUps);

                if (yield == 0) continue;

                // Monsters gain friendship when level up in battle.
                // Based on: https://bulbapedia.bulbagarden.net/wiki/Friendship#Generation_VII
                foreach (MonsterInstance.LevelUpData _ in monsterLevelUps)
                    switch (battler.Friendship)
                    {
                        case < 100:
                            battler.ChangeFriendship(5,
                                                     BattleManager.PlayerCharacter.Scene.Asset,
                                                     BattleManager.Localizer);

                            break;
                        case < 199:
                            battler.ChangeFriendship(4,
                                                     BattleManager.PlayerCharacter.Scene.Asset,
                                                     BattleManager.Localizer);

                            break;
                        default:
                            battler.ChangeFriendship(3,
                                                     BattleManager.PlayerCharacter.Scene.Asset,
                                                     BattleManager.Localizer);

                            break;
                    }

                if (!battler.CanUseHeldItemInBattle(BattleManager)) continue;

                bool consume = false;

                yield return battler.HeldItem.OnYield(battler,
                                                      BattleManager.YAPUSettings,
                                                      BattleManager,
                                                      BattleManager.Localizer,
                                                      shouldConsume => consume = shouldConsume);

                if (consume) yield return battler.ConsumeItemInBattle(BattleManager);
            }

            if (yieldEv)
                foreach (Battler battler in yieldingBattler.BattlersFought)
                {
                    foreach (StatByteValuePair pair in yieldingBattler.FormData.EVYield)
                        battler.ChangeEV(BattleManager.YAPUSettings, pair.Stat, pair.Value, BattleManager.Localizer);
                }

            yield return DialogManager.ShowXPGainDialogAndLearnMoveInBattle(Rosters.AllyRosters[0]
                                                                               .ToMonsterInstances(),
                                                                            amounts,
                                                                            levelUps,
                                                                            BattleManager.Localizer);

            Animation.UpdatePanels();

            yield return DialogManager.WaitForDialog;

            yield return new WaitUntil(() => !DialogManager.XPPanelShown);
        }

        /// <summary>
        /// Have a battler run away.
        /// It can be the allies or the enemies, but the battle will end anyway.
        /// </summary>
        /// <param name="type">The type of battler that will run away.</param>
        /// <param name="index">Index of the battler running away.</param>
        /// <param name="playAnimation">Play the slide out animation?</param>
        /// <param name="bypassAllyChances">Bypass the chances for allies and have them run always?</param>
        public IEnumerator RunAway(BattlerType type, int index, bool playAnimation, bool bypassAllyChances = false)
        {
            Battler battler = BattleManager.Battlers.GetBattlerFromBattleIndex(type, index);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (type)
            {
                case BattlerType.Enemy:

                    yield return battler.OnRunAway(BattleManager);

                    for (int i = 0; i < BattleManager.EnemyBattlerSprites.Length; ++i)
                    {
                        if (!Battlers.IsBattlerFighting(BattlerType.Enemy, i)) continue;

                        DialogManager.ShowDialog("Battle/Run/Enemy",
                                                 acceptInput: false,
                                                 localizableModifiers: false,
                                                 modifiers: Battlers
                                                           .GetBattlerFromRosterAndIndex(BattlerType
                                                                   .Enemy,
                                                                i,
                                                                0)
                                                           .GetNameOrNickName(BattleManager.Localizer),
                                                 switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

                        Audio.PlayRunAway();

                        if (playAnimation) BattleManager.EnemyPanels[i].SlideOut();
                        yield return BattleManager.EnemyBattlerSprites[i].SlideOut(BattleManager.BattleSpeed);

                        yield return DialogManager.WaitForDialog;
                    }

                    BattleManager.DidRunAway = true;
                    BattleManager.IsBattleOver = true;

                    break;

                case BattlerType.Ally:

                    BattleManager.RunAwayAttempts++;

                    float probability = BattleUtils.CalculateRunChance(BattleManager);
                    float chance = bypassAllyChances ? 0 : RandomProvider.Value01();

                    Logger.Info("Run probability: " + probability + ", roll: " + chance + ".");

                    if (chance < probability)
                    {
                        DialogManager.ShowDialog("Battle/Run/Ally",
                                                 switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed,
                                                 acceptInput: false);

                        for (int i = 0; i < BattleManager.AllyBattlerSprites.Length; ++i)
                        {
                            if (!Battlers.IsBattlerFighting(BattlerType.Ally, i)) continue;

                            yield return battler.OnRunAway(BattleManager);

                            Audio.PlayRunAway();
                            BattleManager.AllyPanels[i].SlideOut();

                            if (playAnimation)
                                yield return BattleManager.AllyBattlerSprites[i].SlideOut(BattleManager.BattleSpeed);
                        }

                        BattleManager.DidRunAway = true;
                        BattleManager.IsBattleOver = true;
                    }
                    else
                        DialogManager.ShowDialog("Battle/Run/Fail",
                                                 switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed,
                                                 acceptInput: false);

                    yield return DialogManager.WaitForDialog;

                    break;
            }
        }
    }
}