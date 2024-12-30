using System;
using System.Collections;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Forms;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.Player;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Helper class to do some of the numbers of Monster data.
    /// </summary>
    public static class MonsterMathHelper
    {
        /// <summary>
        /// Cached names for shader properties.
        /// </summary>
        private static readonly int Spot1Coords = Shader.PropertyToID("_Spot1Coords");

        /// <summary>
        /// Cached names for shader properties.
        /// </summary>
        private static readonly int Spot2Coords = Shader.PropertyToID("_Spot2Coords");

        /// <summary>
        /// Cached names for shader properties.
        /// </summary>
        private static readonly int Spot3Coords = Shader.PropertyToID("_Spot3Coords");

        /// <summary>
        /// Cached names for shader properties.
        /// </summary>
        private static readonly int Spot4Coords = Shader.PropertyToID("_Spot4Coords");

        /// <summary>
        /// Calculate a monster's total stat.
        /// </summary>
        /// <param name="monsterInstance">Reference to the monster instance.</param>
        /// <param name="stat">Reference to the stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>An uint representing the calculated total value for that stat.</returns>
        public static uint CalculateStat(MonsterInstance monsterInstance, Stat stat, BattleManager battleManager) =>
            CalculateStatWithSpecificForm(monsterInstance, monsterInstance.Form, stat, battleManager);

        /// <summary>
        /// Calculate a monster's total stat with a form that may not be the current form.
        /// </summary>
        /// <param name="monsterInstance">Reference to the monster instance.</param>
        /// <param name="form">Form to calculate the data from.</param>
        /// <param name="stat">Reference to the stat to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager if we are in battle.</param>
        /// <returns>An uint representing the calculated total value for that stat.</returns>
        public static uint CalculateStatWithSpecificForm(MonsterInstance monsterInstance,
                                                         Form form,
                                                         Stat stat,
                                                         BattleManager battleManager)
        {
            if (!monsterInstance.Species.AvailableFormsWithShinnies.Contains(form)) return 0;

            float multiplier = monsterInstance.OnCalculateStat(stat, form, battleManager, out uint overrideBaseValue);

            uint statValue;

            if (stat == Stat.Hp)
            {
                uint baseHp = overrideBaseValue > 0
                                  ? overrideBaseValue
                                  : CalculateHP(monsterInstance, stat);

                statValue = (uint) Mathf.FloorToInt(baseHp * multiplier);
            }
            else
            {
                uint baseValue = overrideBaseValue > 0
                                     ? overrideBaseValue
                                     : (uint) Mathf.FloorToInt((Mathf.FloorToInt((2
                                                                              * monsterInstance.FormData
                                                                                   .BaseStats[stat]
                                                                              + monsterInstance.StatData
                                                                                   .IndividualValues[stat]
                                                                              + Mathf
                                                                                   .FloorToInt(monsterInstance
                                                                                           .StatData
                                                                                           .EffortValues
                                                                                                [stat]
                                                                                      * .25f))
                                                                  * monsterInstance.StatData.Level
                                                                  * .01f)
                                                              + 5)
                                                             * monsterInstance.StatData.Nature.GetStatMultiplier(stat));

                statValue = (uint) Mathf.FloorToInt(baseValue * multiplier);
            }

            if (monsterInstance.FormData.StatLimits.TryGetValue(stat, out uint limit))
                statValue = (uint) Mathf.Min(statValue, limit);

            return statValue;
        }

        /// <summary>
        /// Calculate a monster's total hp.
        /// </summary>
        /// <param name="monsterInstance">Reference to the monster instance.</param>
        /// <param name="hpStat">Reference to the stat to calculate.</param>
        /// <returns>An uint representing the calculated total value for that stat.</returns>
        private static uint CalculateHP(MonsterInstance monsterInstance, Stat hpStat) =>
            (uint) Mathf.FloorToInt((2 * monsterInstance.Species[monsterInstance.Form].BaseStats[hpStat]
                                   + monsterInstance.StatData.IndividualValues[hpStat]
                                   + Mathf.FloorToInt(monsterInstance.StatData.EffortValues[hpStat] * .25f))
                                  * monsterInstance.StatData.Level
                                  * .01f)
          + monsterInstance.StatData.Level
          + 10;

        /// <summary>
        /// Get the multiplier of a stage of a stat during the battle.
        /// https://bulbapedia.bulbagarden.net/wiki/Stat#Stage_multipliers
        /// </summary>
        /// <param name="battler">Reference to the battler instance.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <param name="isCritical">Used if calculating a critical damage.</param>
        /// <returns>The multiplier that this stage gives.</returns>
        public static float GetStageMultiplier(Battler battler, Stat stat, bool isCritical = false)
        {
            short stage = battler.StatStage[stat];

            if (isCritical) stage = (short) Mathf.Max(0, stage);

            return stage switch
            {
                >= 6 => 4,
                5 => 3.5f,
                4 => 3,
                3 => 2.5f,
                2 => 2,
                1 => 1.5f,
                0 => 1,
                -1 => 2f / 3,
                -2 => .5f,
                -3 => .4f,
                -4 => 1f / 3,
                -5 => 2f / 7,
                // ReSharper disable once PatternIsRedundant
                <= -6 => .25f
            };
        }

        /// <summary>
        /// Get the multiplier of a stage of a stat during the battle.
        /// https://bulbapedia.bulbagarden.net/wiki/Stat#Stage_multipliers
        /// </summary>
        /// <param name="stage">Stage to calculate.</param>
        /// <param name="stat">Stat to calculate.</param>
        /// <returns>The multiplier that this stage gives.</returns>
        public static float GetStageMultiplier(short stage, BattleStat stat) =>
            // Evasion is the same table as accuracy but inverted.
            (stat == BattleStat.Accuracy ? stage : stage * -1) switch
            {
                >= 6 => 3,
                5 => 8f / 3,
                4 => 7f / 3,
                3 => 2,
                2 => 5f / 3,
                1 => 4f / 3,
                0 => 1,
                -1 => .75f,
                -2 => .6f,
                -3 => .5f,
                -4 => 3f / 7,
                -5 => 3f / 8,
                // ReSharper disable once PatternIsRedundant
                <= -6 => 1f / 3
            };

        /// <summary>
        /// Calculate the XP gain of a battler from a fainted battler.
        /// Base on: https://bulbapedia.bulbagarden.net/wiki/Experience#Gain_formula
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <param name="enemyType">The type of enemy it was.</param>
        /// <param name="faintedBattler">Reference to the fainted battler.</param>
        /// <param name="receiver">Reference to the receiver battler.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The number of XP to gain.</returns>
        public static float CalculateXPYield(YAPUSettings settings,
                                             PlayerSettings playerSettings,
                                             EnemyType enemyType,
                                             Battler faintedBattler,
                                             Battler receiver,
                                             BattleManager battleManager)
        {
            if (receiver.StatData.Level == 100 || receiver.CurrentHP == 0 || receiver.EggData.IsEgg) return 0;

            float a = enemyType == EnemyType.Wild ? 1 : 1.5f;
            float b = faintedBattler.FormData.BaseExperience;

            float e = receiver.CanUseHeldItemInBattle(battleManager)
                          ? receiver.HeldItem.CalculateXPYieldModifier(settings,
                                                                       playerSettings,
                                                                       enemyType,
                                                                       faintedBattler,
                                                                       receiver)
                          : 1;

            float f = receiver.Friendship >= 50 ? 1.2f : 1f;
            float l = faintedBattler.StatData.Level;
            float lp = receiver.StatData.Level;
            const float p = 1f; // XP points power, probably won't implement.

            float s = 1;

            if (playerSettings.AllTeamGainsXPOnFaint
             || (receiver.CanUseHeldItemInBattle(battleManager) && receiver.HeldItem.SharesXP))
            {
                if (!faintedBattler.BattlersFought.Contains(receiver)) s = 2;
            }
            else
            {
                if (!faintedBattler.BattlersFought.Contains(receiver)) return 0;
            }

            float t = receiver.OriginData.Trainer == receiver.CurrentTrainer ? 1f : 1.5f;
            float v = receiver.IsBelowEvolutionLevel ? 1.2f : 1f;

            float firstNumerator = a * b * l * f * v;
            float firstDenominator = 5 * s;
            float secondNumerator = 2 * l + 10;
            float secondDenominator = l + lp + 10;
            float lastMultiplier = t * e * p;
            float firstFraction = firstNumerator / firstDenominator;
            float secondFraction = Mathf.Pow(secondNumerator / secondDenominator, 2.5f);
            return Mathf.Max(1, firstFraction * secondFraction * lastMultiplier);
        }

        /// <summary>
        /// Calculate the chances of getting a critical hit.
        /// </summary>
        /// <param name="battler">Battler to calculate the chance from.</param>
        /// <param name="criticalStageModifier">Modifier applied by the move to the critical stage.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="ignoreAbilities">Does the move ignore abilities?</param>
        /// <param name="finished">Callback stating the final chance.</param>
        /// <returns>The chances of getting a critical hit.</returns>
        public static IEnumerator CalculateCriticalChance(Battler battler,
                                                          int criticalStageModifier,
                                                          Battler target,
                                                          BattleManager battleManager,
                                                          Move move,
                                                          bool ignoreAbilities,
                                                          Action<float> finished)
        {
            float multiplier = 1;
            byte stageModifier = 0;
            bool alwaysHit = false;

            yield return battler.OnCalculateCriticalChance(target,
                                                           battleManager,
                                                           move,
                                                           (wasModified,
                                                            newMultiplier,
                                                            newStageModifier,
                                                            shouldAlwaysHit) =>
                                                           {
                                                               if (!wasModified) return;
                                                               multiplier = newMultiplier;
                                                               stageModifier = newStageModifier;
                                                               alwaysHit = shouldAlwaysHit;
                                                           });

            if (multiplier == 0)
            {
                finished.Invoke(0);
                yield break;
            }

            if (alwaysHit)
            {
                finished.Invoke(1);
                yield break;
            }

            multiplier *=
                target.OnCalculateCriticalChanceWhenTargeted(battler,
                                                             battleManager,
                                                             move,
                                                             ignoreAbilities,
                                                             out alwaysHit);

            if (multiplier == 0)
            {
                finished.Invoke(0);
                yield break;
            }

            if (alwaysHit)
            {
                finished.Invoke(1);
                yield break;
            }

            float chance = battler.CriticalStage
                         + criticalStageModifier
                         + stageModifier switch
                           {
                               >= 3 => 1,
                               2 => .5f,
                               1 => .125f,
                               _ => 1f / 24
                           };

            finished.Invoke(chance * multiplier);
        }

        /// <summary>
        /// Calculate the size of a monster in battle.
        /// </summary>
        /// <param name="battler">Battler to calculate.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns></returns>
        public static Vector3 CalculateBattleSize(Battler battler,
                                                  BattleManager battleManager)
        {
            float heightRatio = battler.GetHeight(battleManager, false) / battleManager.MonsterDatabase.MeanMonsterSize;

            // https://www.sciencedirect.com/topics/mathematics/sigmoid-function
            float standardSigmoid = 1f / (1f + Mathf.Exp(-(heightRatio - .5f)));

            // Sum .5f to have 1 be the middle and .5 and 1.5 the limits.
            float displacedSigmoid = standardSigmoid + .5f;

            // Adapt to the set ranges.
            float scale = displacedSigmoid switch
            {
                < 1 => displacedSigmoid * (battleManager.YAPUSettings.SizeLimitsInBattle.x / .5f),
                > 1 => displacedSigmoid * (battleManager.YAPUSettings.SizeLimitsInBattle.y / 1.5f),
                _ => 1
            };

            return new Vector3(scale, scale, 1);
        }

        /// <summary>
        /// Add additional properties to the material depending on the monster.
        /// </summary>
        /// <param name="monsterInstance">Monster being displayed on the material.</param>
        /// <param name="front">Front or back sprite?</param>
        /// <param name="materialPropertyBlock">Property block to access the material properties.</param>
        /// <param name="yapuSettings">Reference to the YAPU settings.</param>
        public static void AddAdditionalMaterialProperties(MonsterInstance monsterInstance,
                                                           bool front,
                                                           ref MaterialPropertyBlock materialPropertyBlock,
                                                           YAPUSettings yapuSettings) =>
            AddAdditionalMaterialProperties(monsterInstance.Species,
                                            monsterInstance.Form,
                                            monsterInstance.PhysicalData.Gender,
                                            monsterInstance.EggData.IsEgg,
                                            front,
                                            monsterInstance.ExtraData.PersonalityValue,
                                            ref materialPropertyBlock,
                                            yapuSettings);

        /// <summary>
        /// Add additional properties to the material depending on the monster.
        /// </summary>
        /// <param name="species">Monster species.</param>
        /// <param name="form">Monster form.</param>
        /// <param name="gender">Monster gender.</param>
        /// <param name="isEgg">Is the monster an egg?</param>
        /// <param name="front">Front or back sprite?</param>
        /// <param name="personalityValue">Personality value of the monster.</param>
        /// <param name="materialPropertyBlock">Property block to access the material properties.</param>
        /// <param name="yapuSettings">Reference to the YAPU settings.</param>
        public static void AddAdditionalMaterialProperties(MonsterEntry species,
                                                           Form form,
                                                           MonsterGender gender,
                                                           bool isEgg,
                                                           bool front,
                                                           int personalityValue,
                                                           ref MaterialPropertyBlock materialPropertyBlock,
                                                           YAPUSettings yapuSettings)
        {
            DataByFormEntry formData = species[form];

            if (!formData.UsesSpindaShader) return;

            int[] spotCoords = SplitInto4BitSegments(personalityValue);

            Vector2 spot1RelativeCoords = new(spotCoords[0], spotCoords[1]);
            Vector2 spot2RelativeCoords = new(spotCoords[2], spotCoords[3]);
            Vector2 spot3RelativeCoords = new(spotCoords[4], spotCoords[5]);
            Vector2 spot4RelativeCoords = new(spotCoords[6], spotCoords[7]);

            materialPropertyBlock.SetVector(Spot1Coords, spot1RelativeCoords);
            materialPropertyBlock.SetVector(Spot2Coords, spot2RelativeCoords);
            materialPropertyBlock.SetVector(Spot3Coords, spot3RelativeCoords);
            materialPropertyBlock.SetVector(Spot4Coords, spot4RelativeCoords);
        }

        /// <summary>
        /// Add additional properties to the material depending on the monster.
        /// Uses a material instead of a property block to be used in UI.
        /// </summary>
        /// <param name="species">Monster species.</param>
        /// <param name="form">Monster form.</param>
        /// <param name="gender">Monster gender.</param>
        /// <param name="isEgg">Is the monster an egg?</param>
        /// <param name="front">Front or back sprite?</param>
        /// <param name="personalityValue">Personality value of the monster.</param>
        /// <param name="material">Property block to access the material properties.</param>
        /// <param name="yapuSettings">Reference to the YAPU settings.</param>
        public static void AddAdditionalMaterialProperties(MonsterEntry species,
                                                           Form form,
                                                           MonsterGender gender,
                                                           bool isEgg,
                                                           bool front,
                                                           int personalityValue,
                                                           Material material,
                                                           YAPUSettings yapuSettings)
        {
            DataByFormEntry formData = species[form];

            if (!formData.UsesSpindaShader) return;

            int[] spotCoords = SplitInto4BitSegments(personalityValue);

            Vector2 spot1RelativeCoords = new(spotCoords[0], spotCoords[1]);
            Vector2 spot2RelativeCoords = new(spotCoords[2], spotCoords[3]);
            Vector2 spot3RelativeCoords = new(spotCoords[4], spotCoords[5]);
            Vector2 spot4RelativeCoords = new(spotCoords[6], spotCoords[7]);

            if (!material.HasVector(Spot1Coords)) return;
            material.SetVector(Spot1Coords, spot1RelativeCoords);
            material.SetVector(Spot2Coords, spot2RelativeCoords);
            material.SetVector(Spot3Coords, spot3RelativeCoords);
            material.SetVector(Spot4Coords, spot4RelativeCoords);
        }

        /// <summary>
        /// Get the localization string for the egg cycles of a monster.
        /// </summary>
        /// <param name="monster">Monster to check.</param>
        /// <returns>The localization string for its egg cycles.</returns>
        public static string GetEggCyclesLocalizationString(MonsterInstance monster) =>
            monster.EggData.EggCyclesLeft switch
            {
                < 6 => "Monsters/Egg/CyclesLeft/Low",
                < 11 => "Monsters/Egg/CyclesLeft/Medium",
                < 41 => "Monsters/Egg/CyclesLeft/High",
                _ => "Monsters/Egg/CyclesLeft/Highest"
            };

        /// <summary>
        /// Split a number into 4 bit segments.
        /// </summary>
        /// <param name="input">Original 32 bit int.</param>
        /// <returns>The split segments.</returns>
        private static int[] SplitInto4BitSegments(int input)
        {
            int[] segments = new int[8];
            for (int i = 0; i < 8; i++) segments[i] = (input >> (i * 4)) & 0xF;
            return segments;
        }
    }
}