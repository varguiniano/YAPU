using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Module class for the battle manager that handles stuff related to battler stats.
    /// </summary>
    public class BattleManagerBattlerStatsModule : BattleManagerModule<BattleManagerBattlerStatsModule>
    {
        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">Roster number.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        [Button("Change stat stage")]
        [HideInEditorMode]
        [FoldoutGroup("Debug")]
        private void TestChangeStatStage(BattlerType type,
                                         int index,
                                         Stat stat,
                                         short modifier,
                                         BattlerType userType,
                                         int userIndex,
                                         Ability changingAbility = null) =>
            StartCoroutine(ChangeStatStage(type, index, stat, modifier, userType, userIndex, changingAbility));

        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// </summary>
        /// <param name="type">Type of battler.</param>
        /// <param name="index">Roster number.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        [Button("Change stat stage")]
        [HideInEditorMode]
        [FoldoutGroup("Debug")]
        private void TestChangeStatStage(BattlerType type,
                                         int index,
                                         BattleStat stat,
                                         short modifier,
                                         BattlerType userType,
                                         int userIndex) =>
            StartCoroutine(ChangeStatStage(type, index, stat, modifier, userType, userIndex));

        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// </summary>
        /// <param name="target">Battler affected.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="user">Monster that triggered this change.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        /// <param name="bypassSubstitute">Can the effect bypass the substitute?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        /// <param name="showCantChangeMessage">Show a message when the stat can't change?</param>
        /// <param name="finished">Callback stating the final modifier that was applied.</param>
        public IEnumerator ChangeStatStage(Battler target,
                                           Stat stat,
                                           short modifier,
                                           Battler user,
                                           Ability changingAbility = null,
                                           bool bypassSubstitute = false,
                                           bool ignoreAbilities = false,
                                           bool showCantChangeMessage = true,
                                           Action<short> finished = null)
        {
            yield return ChangeStatStage(BattleManager.Battlers.GetTypeAndIndexOfBattler(target),
                                         stat,
                                         modifier,
                                         BattleManager.Battlers.GetTypeAndIndexOfBattler(user),
                                         changingAbility,
                                         bypassSubstitute,
                                         ignoreAbilities,
                                         showCantChangeMessage,
                                         finished);
        }

        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// </summary>
        /// <param name="targetTuple">Type and index of battler.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userTuple">The type and index of the monster that triggered this change.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        /// <param name="bypassSubstitute">Can the effect bypass the substitute?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        /// <param name="showCantChangeMessage">Show a message when the stat can't change?</param>
        /// <param name="finished">Callback stating the final modifier that was applied.</param>
        public IEnumerator ChangeStatStage((BattlerType targetType, int targetIndex) targetTuple,
                                           Stat stat,
                                           short modifier,
                                           (BattlerType userType, int userIndex) userTuple,
                                           Ability changingAbility = null,
                                           bool bypassSubstitute = false,
                                           bool ignoreAbilities = false,
                                           bool showCantChangeMessage = true,
                                           Action<short> finished = null)
        {
            yield return ChangeStatStage(targetTuple.targetType,
                                         targetTuple.targetIndex,
                                         stat,
                                         modifier,
                                         userTuple.userType,
                                         userTuple.userIndex,
                                         changingAbility,
                                         bypassSubstitute,
                                         ignoreAbilities,
                                         showCantChangeMessage,
                                         finished);
        }

        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="changingAbility">Ability that changed the stat, if any.</param>
        /// <param name="bypassSubstitute">Can the effect bypass the substitute?</param>
        /// <param name="ignoreAbilities">Does the effect making the change ignore abilities?</param>
        /// <param name="showCantChangeMessage">Show a message when the stat can't change?</param>
        /// <param name="finished">Callback stating the final modifier that was applied.</param>
        public IEnumerator ChangeStatStage(BattlerType targetType,
                                           int targetIndex,
                                           Stat stat,
                                           short modifier,
                                           BattlerType userType,
                                           int userIndex,
                                           Ability changingAbility = null,
                                           bool bypassSubstitute = false,
                                           bool ignoreAbilities = false,
                                           bool showCantChangeMessage = true,
                                           Action<short> finished = null)
        {
            Battler target = Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);
            Battler user = Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            ignoreAbilities &= (userType != targetType || userIndex != targetIndex);

            if (user.CanUseAbility(BattleManager, false)
             && user.GetAbility().ByPassesSubstitute(targetType, targetIndex, BattleManager, userType, userIndex))
                bypassSubstitute = true;

            short currentValue = target.StatStage[stat];
            string statLocalizationKey = stat.GetLocalizationString();

            short clampedModifier = modifier;

            foreach (KeyValuePair<SideStatus, int> sideStatuses in Statuses.GetSideStatuses(targetType))
                yield return sideStatuses.Key.OnStatChange(targetType,
                                                           targetIndex,
                                                           stat,
                                                           clampedModifier,
                                                           userType,
                                                           userIndex,
                                                           BattleManager,
                                                           BattleManager.Localizer,
                                                           newModifier => clampedModifier = newModifier);

            if (clampedModifier == 0)
            {
                finished?.Invoke(clampedModifier);
                yield break;
            }

            if (!bypassSubstitute && target.Substitute.SubstituteEnabled && clampedModifier < 0)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Substitute/Protected",
                                                             localizableModifiers: false,
                                                             modifiers: target.GetNameOrNickName(BattleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);

                finished?.Invoke(0);
                yield break;
            }

            yield return target.OnStatChange(stat,
                                             modifier,
                                             userType,
                                             userIndex,
                                             ignoreAbilities,
                                             BattleManager,
                                             changingAbility,
                                             newModifier => clampedModifier = newModifier);

            if (clampedModifier == 0)
            {
                finished?.Invoke(clampedModifier);
                yield break;
            }

            yield return CalculateStatStageModifierAndMessage(target,
                                                              statLocalizationKey,
                                                              currentValue,
                                                              clampedModifier,
                                                              showCantChangeMessage,
                                                              newClamped => clampedModifier = newClamped);

            if (clampedModifier == 0)
            {
                finished?.Invoke(clampedModifier);

                yield break;
            }

            target.StatStage[stat] += clampedModifier;

            yield return PlayStatChangeAnimation(clampedModifier < 0, targetType, targetIndex);

            switch (clampedModifier)
            {
                case > 0:
                    target.IncreasedStatsThisTurn = true;
                    break;
                case < 0:
                    target.DecreasedStatsThisTurn = true;
                    break;
            }

            yield return target.AfterStatChanged(stat,
                                                 clampedModifier,
                                                 userType,
                                                 userIndex,
                                                 ignoreAbilities,
                                                 BattleManager);

            finished?.Invoke(clampedModifier);
        }

        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// </summary>
        /// <param name="target">Battler affected.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="user">Monster that triggered this change.</param>
        /// <param name="ignoreAbilities"></param>
        /// <param name="showCantChangeMessage">Show a message when the stat can't change?</param>
        public IEnumerator ChangeStatStage(Battler target,
                                           BattleStat stat,
                                           short modifier,
                                           Battler user,
                                           bool ignoreAbilities = false,
                                           bool showCantChangeMessage = true)
        {
            yield return ChangeStatStage(BattleManager.Battlers.GetTypeAndIndexOfBattler(target),
                                         stat,
                                         modifier,
                                         BattleManager.Battlers.GetTypeAndIndexOfBattler(user),
                                         ignoreAbilities,
                                         showCantChangeMessage);
        }

        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// </summary>
        /// <param name="targetTuple">Type and index of battler.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userTuple">The type and index of the monster that triggered this change.</param>
        /// <param name="ignoreAbilities">does the changing effect ignore abilities?</param>
        /// <param name="showCantChangeMessage">Show a message when the stat can't change?</param>
        public IEnumerator ChangeStatStage((BattlerType targetType, int targetIndex) targetTuple,
                                           BattleStat stat,
                                           short modifier,
                                           (BattlerType userType, int userIndex) userTuple,
                                           bool ignoreAbilities,
                                           bool showCantChangeMessage = true)
        {
            yield return ChangeStatStage(targetTuple.targetType,
                                         targetTuple.targetIndex,
                                         stat,
                                         modifier,
                                         userTuple.userType,
                                         userTuple.userIndex,
                                         showCantChangeMessage,
                                         ignoreAbilities: ignoreAbilities);
        }

        /// <summary>
        /// Change the stat stage of a stat of one of the battlers.
        /// TODO: This is mostly the same code as the previous method, it would be good to see if we can merge them.
        /// </summary>
        /// <param name="targetType">Type of battler.</param>
        /// <param name="targetIndex">In battle index.</param>
        /// <param name="stat">Stat to change.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="showCantChangeMessage">Show a message when the stat can't change?</param>
        /// <param name="bypassSubstitute">Does this bypass the substitute?</param>
        /// <param name="ignoreAbilities">Does the changing effect ignore abilities?</param>
        public IEnumerator ChangeStatStage(BattlerType targetType,
                                           int targetIndex,
                                           BattleStat stat,
                                           short modifier,
                                           BattlerType userType,
                                           int userIndex,
                                           bool showCantChangeMessage = true,
                                           bool bypassSubstitute = false,
                                           bool ignoreAbilities = false)
        {
            Battler target = Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);
            Battler user = Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(BattleManager, false)
             && user.GetAbility().ByPassesSubstitute(targetType, targetIndex, BattleManager, userType, userIndex))
                bypassSubstitute = true;

            short currentValue = target.BattleStatStage[stat];
            string statLocalizationKey = stat.GetLocalizationString();

            short clampedModifier = modifier;

            foreach (KeyValuePair<SideStatus, int> sideStatuses in Statuses.GetSideStatuses(targetType))
                yield return sideStatuses.Key.OnStatChange(targetType,
                                                           targetIndex,
                                                           stat,
                                                           clampedModifier,
                                                           userType,
                                                           userIndex,
                                                           BattleManager,
                                                           BattleManager.Localizer,
                                                           newModifier => clampedModifier = newModifier);

            yield return target.OnStatChange(stat,
                                             modifier,
                                             userType,
                                             userIndex,
                                             ignoreAbilities,
                                             BattleManager,
                                             newModifier => clampedModifier = newModifier);

            if (clampedModifier == 0) yield break;

            if (!bypassSubstitute && target.Substitute.SubstituteEnabled && clampedModifier < 0)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Substitute/Protected",
                                                             localizableModifiers: false,
                                                             modifiers: target.GetNameOrNickName(BattleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);

                yield break;
            }

            yield return CalculateStatStageModifierAndMessage(target,
                                                              statLocalizationKey,
                                                              currentValue,
                                                              modifier,
                                                              showCantChangeMessage,
                                                              x => clampedModifier = x);

            if (clampedModifier == 0) yield break;

            target.BattleStatStage[stat] += clampedModifier;

            yield return PlayStatChangeAnimation(clampedModifier < 0, targetType, targetIndex);

            switch (clampedModifier)
            {
                case > 0:
                    target.IncreasedStatsThisTurn = true;
                    break;
                case < 0:
                    target.DecreasedStatsThisTurn = true;
                    break;
            }

            yield return target.AfterStatChanged(stat,
                                                 clampedModifier,
                                                 userType,
                                                 userIndex,
                                                 ignoreAbilities,
                                                 BattleManager);
        }

        /// <summary>
        /// Change the critical stage of a monster.
        /// </summary>
        /// <param name="target">Battler to change.</param>
        /// <param name="modifier">Amount to change.</param>
        /// <param name="user">The monster that triggered this change.</param>
        public IEnumerator ChangeCriticalStage(Battler target,
                                               short modifier,
                                               Battler user)
        {
            yield return ChangeCriticalStage(BattleManager.Battlers.GetTypeAndIndexOfBattler(target),
                                             modifier,
                                             BattleManager.Battlers.GetTypeAndIndexOfBattler(user));
        }

        /// <summary>
        /// Change the critical stage of a monster.
        /// </summary>
        /// <param name="targetTuple">Type and index of battler to change.</param>
        /// <param name="modifier">Amount to change.</param>
        /// <param name="userTuple">The type and index of the monster that triggered this change.</param>
        public IEnumerator ChangeCriticalStage((BattlerType targetType,
                                                   int targetIndex) targetTuple,
                                               short modifier,
                                               (BattlerType userType,
                                                   int userIndex) userTuple)
        {
            yield return ChangeCriticalStage(targetTuple.targetType,
                                             targetTuple.targetIndex,
                                             modifier,
                                             userTuple.userType,
                                             userTuple.userIndex);
        }

        /// <summary>
        /// Change the critical stage of a monster.
        /// </summary>
        /// <param name="targetType">Type of battler to change.</param>
        /// <param name="targetIndex">In battle index of the battler to change</param>
        /// <param name="modifier">Amount to change.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="bypassSubstitute">Does it bypass the substitute?</param>
        public IEnumerator ChangeCriticalStage(BattlerType targetType,
                                               int targetIndex,
                                               short modifier,
                                               BattlerType userType,
                                               int userIndex,
                                               bool bypassSubstitute = false)
        {
            Battler battler = Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);
            Battler user = Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            if (user.CanUseAbility(BattleManager, false)
             && user.GetAbility().ByPassesSubstitute(targetType, targetIndex, BattleManager, userType, userIndex))
                bypassSubstitute = true;

            modifier = (short) (battler.CriticalStage + modifier < 0 ? -battler.CriticalStage : modifier);

            if (!bypassSubstitute && battler.Substitute.SubstituteEnabled && modifier < 0)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Substitute/Protected",
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(BattleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);

                yield break;
            }

            battler.CriticalStage = (byte) (battler.CriticalStage + modifier);

            foreach (KeyValuePair<SideStatus, int> sideStatuses in Statuses.GetSideStatuses(targetType))
                yield return sideStatuses.Key.OnCriticalStageChange(targetType,
                                                                    targetIndex,
                                                                    modifier,
                                                                    userType,
                                                                    userIndex,
                                                                    BattleManager,
                                                                    BattleManager.Localizer,
                                                                    newModifier => modifier = newModifier);

            if (modifier <= 0) yield break;

            yield return DialogManager.ShowDialogAndWait("Battle/CriticalStage/Increase",
                                                         localizableModifiers: false,
                                                         modifiers: battler.GetNameOrNickName(BattleManager.Localizer),
                                                         switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);
        }

        /// <summary>
        /// Calculates the stat stage modifier and message needed to rise or lower stat stages.
        /// </summary>
        /// <param name="battler">Battler of the stat.</param>
        /// <param name="statLocalizationKey">Localization key for that stat.</param>
        /// <param name="currentValue">Current value of that stage.</param>
        /// <param name="modifier">Modifier to apply.</param>
        /// <param name="showCantChangeMessage">Show a message when the stat can't change?</param>
        /// <param name="clampedModifierCallback">Clamped modifier that will be applied sent through a callback.</param>
        private IEnumerator CalculateStatStageModifierAndMessage(MonsterInstance battler,
                                                                 string statLocalizationKey,
                                                                 short currentValue,
                                                                 short modifier,
                                                                 bool showCantChangeMessage,
                                                                 Action<short> clampedModifierCallback)
        {
            short clampedModifier = modifier switch
            {
                > 0 => (short) Mathf.Min(modifier, 6 - currentValue),
                < 0 => (short) Mathf.Max(modifier, -6 - currentValue),
                _ => 0
            };

            string messageLocalizationKey;

            switch (clampedModifier)
            {
                case > 3 when clampedModifier + currentValue == 6:
                    messageLocalizationKey = "Battle/StatChange/Maxed";
                    break;
                case >= 3:
                    messageLocalizationKey = "Battle/StatChange/3";
                    break;
                case 2:
                    messageLocalizationKey = "Battle/StatChange/2";
                    break;
                case 1:
                    messageLocalizationKey = "Battle/StatChange/1";
                    break;
                case 0:
                    if (showCantChangeMessage)
                        yield return DialogManager.ShowDialogAndWait(modifier > 0
                                                                         ? "Battle/StatChange/CantHigher"
                                                                         : "Battle/StatChange/CantLower",
                                                                     localizableModifiers: false,
                                                                     modifiers: new[]
                                                                         {
                                                                             battler.GetNameOrNickName(BattleManager
                                                                                .Localizer),
                                                                             BattleManager.Localizer
                                                                                 [statLocalizationKey]
                                                                         },
                                                                     switchToNextAfterSeconds: 1.5f
                                                                       / BattleManager.BattleSpeed);

                    // End here instead of playing an animation.
                    clampedModifierCallback?.Invoke(clampedModifier);
                    yield break;
                case -1:
                    messageLocalizationKey = "Battle/StatChange/-1";
                    break;
                case -2:
                    messageLocalizationKey = "Battle/StatChange/-2";
                    break;
                // ReSharper disable once PatternIsRedundant
                case <= -3:
                    messageLocalizationKey = "Battle/StatChange/-3";
                    break;
            }

            DialogManager.ShowDialog(messageLocalizationKey,
                                     acceptInput: false,
                                     localizableModifiers: false,
                                     modifiers: new[]
                                                {
                                                    battler.GetNameOrNickName(BattleManager.Localizer),
                                                    BattleManager.Localizer[statLocalizationKey]
                                                },
                                     switchToNextAfterSeconds: 1.5f / BattleManager.BattleSpeed);

            clampedModifierCallback?.Invoke(clampedModifier);
        }

        /// <summary>
        /// Play the stat change animation.
        /// </summary>
        /// <param name="lower">Lower or rise the stat?</param>
        /// <param name="type">type of fighter.</param>
        /// <param name="index">Fighter index.</param>
        private IEnumerator PlayStatChangeAnimation(bool lower, BattlerType type, int index)
        {
            MonsterFXAnimator fxAnimator = type switch
            {
                BattlerType.Ally => BattleManager.AllyBattlerSprites[index].FXAnimator,
                BattlerType.Enemy => BattleManager.EnemyBattlerSprites[index].FXAnimator,
                _ => null
            };

            if (fxAnimator == null) yield break;

            if (lower)
                fxAnimator.PlayLowerStat(BattleManager.BattleSpeed,
                                         () =>
                                         {
                                         });
            else
                fxAnimator.PlayRiseStat(BattleManager.BattleSpeed,
                                        () =>
                                        {
                                        });

            yield return DialogManager.WaitForDialog;
        }

        /// <summary>
        /// Change the critical stage of a monster.
        /// </summary>
        /// <param name="inBattleIndex">In battle index</param>
        /// <param name="amount">Amount to change.</param>
        /// <param name="targetType">Type of the monster.</param>
        [HideInEditorMode]
        [FoldoutGroup("Debug")]
        [Button("Change critical stage")]
        public void ChangeCriticalStage(BattlerType targetType, int inBattleIndex, int amount)
        {
            Battler battler = Battlers.GetBattlerFromBattleIndex(targetType, inBattleIndex);

            if (battler.CriticalStage + amount < 0) return;

            amount = Mathf.Min(amount, 3 - battler.CriticalStage);

            battler.CriticalStage = (byte) (battler.CriticalStage + amount);
        }

        /// <summary>
        /// Raises the friendship of the first ally team
        /// when it is controller by the player and one opponent has a friendship modifier on the character type.
        /// </summary>
        internal void RaiseFriendshipOnBattleStart()
        {
            if (!AI.PlayerControlsFirstRoster) return;

            if (BattleManager.EnemyType != EnemyType.Trainer) return;

            if (!Characters.GetCharacter(BattlerType.Enemy, 0).CharacterType.FriendshipBoostWhenBattled
             && (BattleManager.BattleType != BattleType.DoubleBattle
              || !Characters.GetCharacter(BattlerType.Enemy, 1).CharacterType.FriendshipBoostWhenBattled))
                return;

            foreach (Battler battler in Rosters.GetRoster(BattlerType.Ally, 0))
                switch (battler.Friendship)
                {
                    case < 100:
                        battler.ChangeFriendship(5, BattleManager.PlayerCharacter.Scene.Asset, BattleManager.Localizer);
                        break;
                    case < 199:
                        battler.ChangeFriendship(4, BattleManager.PlayerCharacter.Scene.Asset, BattleManager.Localizer);
                        break;
                    default:
                        battler.ChangeFriendship(3, BattleManager.PlayerCharacter.Scene.Asset, BattleManager.Localizer);
                        break;
                }
        }
    }
}