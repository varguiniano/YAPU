using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using WhateverDevs.Core.Runtime.DataStructures;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for an implementation of the ability LightningRod.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/LightningRod", fileName = "LightningRod")]
    public class LightningRod : Ability
    {
        /// <summary>
        /// Reference to the type this ability attracts.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType AttractedType;

        /// <summary>
        /// Stats to raise when hit.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<Stat, short> StatsToRaise;

        /// <summary>
        /// Callback for when another battler is about to use a move.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="user">Reference to the user of the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Targets of the move. Can be modified.</param>
        /// <param name="hasBeenReflected">Has the same move been reflected?</param>
        /// <param name="finished">Callback stating if the move will still be used, the new targets for the move.</param>
        public override IEnumerator OnOtherBattlerAboutToUseAMove(Battler owner,
                                                                  Battler user,
                                                                  Move move,
                                                                  BattleManager battleManager,
                                                                  List<(BattlerType Type, int Index)> targets,
                                                                  bool hasBeenReflected,
                                                                  Action<bool, List<(BattlerType Type, int Index)>>
                                                                      finished)
        {
            // There may be immune abilities.
            if (!owner.GetAbility()
                      .AffectsUserOfEffect(user,
                                           owner,
                                           IgnoresOtherAbilities(battleManager, owner, null),
                                           battleManager))
            {
                finished.Invoke(true, targets);
                yield break;
            }

            // Only affects single target moves.
            if (move.CanHaveMultipleTargets || owner == user)
            {
                finished.Invoke(true, targets);
                yield break;
            }

            // Only affect moves of the attracted type.
            if (move.GetMoveTypeInBattle(user, battleManager) != AttractedType)
            {
                finished.Invoke(true, targets);
                yield break;
            }

            (BattlerType ownType, int ownIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(owner);
            (BattlerType userType, int userIndex) = battleManager.Battlers.GetTypeAndIndexOfBattler(user);

            // If the move can't target the battler, do nothing.
            List<Battler> potentialTargets =
                MoveUtils.GenerateValidTargetsForMove(battleManager, userType, userIndex, move, StaticLogger);

            if (!potentialTargets.Contains(owner))
            {
                finished.Invoke(true, targets);
                yield break;
            }

            finished.Invoke(true, new List<(BattlerType Type, int Index)> {(ownType, ownIndex)});
        }

        /// <summary>
        /// Only when its an attracted type move.
        /// </summary>
        public override bool DoesBypassAllAccuracyChecksWhenTargeted(Battler owner,
                                                                     Move move,
                                                                     Battler user,
                                                                     BattleManager battleManager) =>
            move.GetMoveTypeInBattle(user, battleManager) == AttractedType && owner != user;

        /// <summary>
        /// Replace the move's effect when for raising the stat.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="callback">Callback stating if the move should still execute its effect.</param>
        public override IEnumerator ShouldReplaceMoveEffectWhenHit(Battler owner,
                                                                   Move move,
                                                                   Battler user,
                                                                   BattleManager battleManager,
                                                                   Action<bool> callback)
        {
            if (move.GetMoveTypeInBattle(user, battleManager) != AttractedType || owner == user)
            {
                callback.Invoke(true);
                yield break;
            }

            ShowAbilityNotification(owner);

            foreach (KeyValuePair<Stat, short> statSlot in StatsToRaise)
                yield return battleManager.BattlerStats.ChangeStatStage(owner,
                                                                        statSlot.Key,
                                                                        statSlot.Value,
                                                                        owner,
                                                                        this);

            callback.Invoke(false);
        }

        /// <summary>
        /// 50% chance of forcing an encounter with the attracted type.
        /// </summary>
        /// <param name="possibleEncounters">Current possible encounters.</param>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="sceneInfo">Info for the current scene.</param>
        /// <param name="encounterType">Encounter type.</param>
        public override void ModifyPossibleEncounters(ref List<WildEncounter> possibleEncounters,
                                                      MonsterInstance owner,
                                                      SceneInfo sceneInfo,
                                                      EncounterType encounterType)
        {
            if (!(Random.value <= .5f)) return;
            Logger.Info("Attempting to force an encounter with a " + AttractedType.name + " type.");

            List<WildEncounter> newCandidates = possibleEncounters
                                               .Where(encounter =>
                                                          encounter
                                                             .Monster[encounter.FormCalculator
                                                                         .GetEncounterForm(sceneInfo,
                                                                              encounterType)]
                                                             .IsOfType(AttractedType))
                                               .ToList();

            if (newCandidates.Count > 0) possibleEncounters = newCandidates;
        }
    }
}