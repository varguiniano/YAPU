using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Weather
{
    /// <summary>
    /// Data class for base weather statuses.
    /// </summary>
    public abstract class Weather : LocalizableMonsterDatabaseScriptable<Weather>
    {
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
        [FoldoutGroup("Effect")]
        [SerializeField]
        protected SerializableDictionary<MonsterType, float> TypeDamageModifiers;

        /// <summary>
        /// Modifiers to apply when calculating a monster's stat if it matches the type.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private SerializableDictionary<MonsterType, SerializableDictionary<Stat, float>> StatModifiers;

        /// <summary>
        /// Modifier to apply to the accuracy of moves.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        private float MoveAccuracyModifier = 1;

        /// <summary>
        /// Animation for when the weather starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public abstract IEnumerator WeatherStartAnimation(BattleManager battleManager);

        /// <summary>
        /// Animation for when the weather ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public abstract IEnumerator WeatherTick(BattleManager battleManager);

        /// <summary>
        /// Animation for when the weather ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public abstract IEnumerator WeatherEndAnimation(BattleManager battleManager);

        /// <summary>
        /// Callback for when a battler is about to use a move.
        /// </summary>
        /// <param name="battler">Battler about to use the move.</param>
        /// <param name="move">Move they will use.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="targets">Move's targets.</param>
        /// <param name="ignoreStatus">Does the move ignore the battler's status?</param>
        /// <param name="ignoresAbilities">Does this move ignore abilities?</param>
        /// <param name="finished">Callback stating if the move will still be used.</param>
        public IEnumerator OnAboutToPerformMove(Battler battler,
                                                Move move,
                                                BattleManager battleManager,
                                                List<(BattlerType Type, int Index)> targets,
                                                bool ignoreStatus,
                                                bool ignoresAbilities,
                                                Action<bool> finished)
        {
            bool performMove = true;

            foreach (KeyValuePair<MonsterType, float> pair in TypeDamageModifiers)
                if (pair.Key == move.GetMoveTypeInBattle(battler, battleManager))
                    if (pair.Value < 0)
                        performMove = false;

            if (!performMove)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/Used",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetNameOrNickName(battleManager
                                                                               .Localizer),
                                                                            battleManager.Localizer
                                                                                [move.LocalizableName]
                                                                        });

                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);
            }

            finished.Invoke(performMove);
        }

        /// <summary>
        /// Calculate the multiplier for a move's power.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <param name="owner">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier for that move.</returns>
        public virtual float CalculateMovesDamageMultiplier(Move move, Battler owner, BattleManager battleManager)
        {
            foreach (KeyValuePair<MonsterType, float> pair in TypeDamageModifiers)
                if (pair.Key == move.GetMoveTypeInBattle(owner, battleManager))
                    return pair.Value;

            return 1;
        }

        /// <summary>
        /// Called when a stat is about to be calculated.
        /// </summary>
        /// <param name="battler">Battler being calculated.</param>
        /// <param name="stat">Stat to be calculated.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier to apply to that stat.</returns>
        public float OnCalculateStat(Battler battler, Stat stat, BattleManager battleManager)
        {
            float multiplier = 1;

            foreach (KeyValuePair<MonsterType, SerializableDictionary<Stat, float>> typeModifier in StatModifiers)
                if (battler.IsOfType(typeModifier.Key, battleManager.YAPUSettings)
                 && typeModifier.Value.TryGetValue(stat, out float value))
                    multiplier *= value;

            return multiplier;
        }

        /// <summary>
        /// Get the general accuracy modifier to apply to all moves.
        /// </summary>
        public float GetGeneralAccuracyModifier() => MoveAccuracyModifier;

        /// <summary>
        /// Base localization root for the asset.
        /// </summary>
        protected override string BaseLocalizationRoot => "Weather/";

        /// <summary>
        /// Auto fill the localization values.
        /// </summary>
        protected override void RefreshLocalizableNames()
        {
            base.RefreshLocalizableNames();
            StartLocalizationKey = BaseLocalizationRoot + name + "/Start";
            TickLocalizationKey = BaseLocalizationRoot + name + "/Tick";
            EndLocalizationKey = BaseLocalizationRoot + name + "/End";
        }
    }
}