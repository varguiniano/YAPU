using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Varguiniano.YAPU.Runtime.Battle.AI;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain
{
    /// <summary>
    /// Base class for battle terrains.
    /// </summary>
    public abstract class Terrain : LocalizableMonsterDatabaseScriptable<Terrain>
    {
        /// <summary>
        /// Localization key for the start dialog.
        /// </summary>
        [FoldoutGroup("Localization")]
        [SerializeField]
        protected string LocalizationKey;

        /// <summary>
        /// Localization key for the start dialog.
        /// </summary>
        [FoldoutGroup("Localization")]
        [SerializeField]
        protected string StartLocalizationKey;

        /// <summary>
        /// Localization key for the tick dialog.
        /// </summary>
        [FoldoutGroup("Localization")]
        [SerializeField]
        protected string TickLocalizationKey;

        /// <summary>
        /// Localization key for the end dialog.
        /// </summary>
        [FoldoutGroup("Localization")]
        [SerializeField]
        protected string EndLocalizationKey;
        
        /// <summary>
        /// Does this asset have a localizable description?
        /// </summary>
        protected override bool HasLocalizableDescription => false;

        /// <summary>
        /// Modifiers to apply when calculating a move's power.
        /// -1 = fail.
        /// </summary>
        [FormerlySerializedAs("TypeDamageModifiers")]
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<MonsterType, float> TypeDamageModifiersWhenUserIsAffected;

        /// <summary>
        /// Modifiers to apply when calculating a move's power.
        /// -1 = fail.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<MonsterType, float> TypeDamageModifiersWhenTargetIsAffected;

        /// <summary>
        /// Move to be used by nature power when this terrain is on.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        [FoldoutGroup("Effect")]
        public Move NaturePowerMove;

        /// <summary>
        /// Is a battler affected by this terrain?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it is affected.</returns>
        public abstract bool IsAffected(Battler battler, BattleManager battleManager);

        /// <summary>
        /// Animation for when the terrain starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public abstract IEnumerator TerrainStartAnimation(BattleManager battleManager);

        /// <summary>
        /// Animation for when the terrain ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public abstract IEnumerator TerrainTick(BattleManager battleManager);

        /// <summary>
        /// Animation for when the terrain ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public abstract IEnumerator TerrainEndAnimation(BattleManager battleManager);

        /// <summary>
        /// Called to modify the action priority of a monster.
        /// </summary>
        /// <param name="owner">Owner of the action.</param>
        /// <param name="action">Action they want to take.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="priority">Precalculated priority.</param>
        public virtual void ModifyActionPriority(Battler owner,
                                                 BattleAction action,
                                                 BattleManager battleManager,
                                                 ref int priority)
        {
        }
        
        /// <summary>
        /// Check if a status can be added to the monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAddStatus(Status status,
                                                BattlerType targetType,
                                                int targetIndex,
                                                BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }
        
        /// <summary>
        /// Check if a status can be added to the monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public virtual IEnumerator CanAddStatus(VolatileStatus status,
                                                BattlerType targetType,
                                                int targetIndex,
                                                BattleManager battleManager,
                                                BattlerType userType,
                                                int userIndex,
                                                Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }
        
        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowMoveMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public virtual IEnumerator OnAboutToBeHitByMove(Battler target,
                                                        Move move,
                                                        BattleManager battleManager,
                                                        Battler user,
                                                        bool didShowMoveMessageNormally,
                                                        Action<bool> callback)
        {
            callback.Invoke(true);
            yield break;
        }

        /// <summary>
        /// Calculate the multiplier for a move's power.
        /// -1 = fail.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <param name="owner">Owner of the move.</param>
        /// <param name="target"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier for that move.</returns>
        public float CalculateMovesDamageMultiplier(Move move,
                                                    Battler owner,
                                                    Battler target,
                                                    BattleManager battleManager)
        {
            float multiplier = 1;

            if (IsAffected(owner, battleManager))
                foreach (KeyValuePair<MonsterType, float> pair in TypeDamageModifiersWhenUserIsAffected)
                    if (pair.Key == move.GetMoveTypeInBattle(owner, battleManager))
                        multiplier *= pair.Value;

            // ReSharper disable once InvertIf
            if (IsAffected(target, battleManager))
                foreach (KeyValuePair<MonsterType, float> pair in TypeDamageModifiersWhenTargetIsAffected)
                    if (pair.Key == move.GetMoveTypeInBattle(owner, battleManager))
                        multiplier *= pair.Value;

            return multiplier;
        }

        /// <summary>
        /// Base localization root for the asset.
        /// </summary>
        protected override string BaseLocalizationRoot => "BattleTerrain/";

        /// <summary>
        /// Auto fill the localization fields.
        /// </summary>
        protected override void RefreshLocalizableNames()
        {
            base.RefreshLocalizableNames();

            LocalizationKey = BaseLocalizationRoot + name;
            StartLocalizationKey = LocalizationKey + "/Start";
            TickLocalizationKey = LocalizationKey + "/Tick";
            EndLocalizationKey = LocalizationKey + "/End";
        }
    }
}