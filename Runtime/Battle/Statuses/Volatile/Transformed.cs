using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Data class for the Transformed status.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Volatile/Transformed", fileName = "Transformed")]
    public class Transformed : VolatileStatus
    {
        /// <summary>
        /// Dictionary that stored the transformation data for each battler.
        /// </summary>
        private readonly Dictionary<Battler, TransformationData> transformations = new();

        /// <summary>
        /// Save the transformation data for the battler.
        /// </summary>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            transformations[battler] = (TransformationData) extraData[0];

            yield return base.OnAddStatus(battleManager, battler, extraData);
        }

        /// <summary>
        /// Clear the transformation data.
        /// </summary>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            yield return battler.ReturnToOriginalForm(battleManager, false);

            if (transformations.ContainsKey(battler)) transformations.Remove(battler);
        }

        /// <summary>
        /// Override the name.
        /// </summary>
        /// <param name="battler"></param>
        /// <returns></returns>
        public override string GetMonsterName(Battler battler) => transformations[battler].Name;

        /// <summary>
        /// Override the stat.
        /// </summary>
        public override float OnCalculateStat(Battler monster, Stat stat, out uint overrideBaseValue)
        {
            overrideBaseValue = 0;

            if (!transformations.ContainsKey(monster)) return 1;

            overrideBaseValue = transformations[monster].Stats[stat];

            return 1;
        }

        /// <summary>
        /// Called when the monster types are being calculated.
        /// </summary>
        /// <param name="battler">Reference to the monster.</param>
        /// <param name="currentCalculatedFirst">First type already calculated.</param>
        /// <param name="currentCalculatedSecond">Second type already calculated.</param>
        /// <returns>The new calculated types.</returns>
        public override (MonsterType, MonsterType) OnCalculateTypes(Battler battler,
                                                                    MonsterType currentCalculatedFirst,
                                                                    MonsterType currentCalculatedSecond) =>
            transformations[battler].Types;

        /// <summary>
        /// Called to calculate the catch rate.
        /// </summary>
        public override byte GetCatchRate(Battler battler, byte current, BattleManager battleManager) =>
            transformations[battler].CatchRate;

        /// <summary>
        /// Get the weight of the battler.
        /// </summary>
        public override (float, float) GetMonsterWeightInBattle(Battler battler) =>
            (0, transformations[battler].Weight);

        /// <summary>
        /// Get the height of the battler.
        /// </summary>
        public override (float, float) GetMonsterHeightInBattle(Battler battler) =>
            (0, transformations[battler].Height);

        /// <summary>
        /// Cannot transform again.
        /// </summary>
        public override bool CanChangeForm(Battler battler, BattleManager battleManager) => false;

        /// <summary>
        /// Cannot transform again.
        /// </summary>
        public override bool CanTransform(Battler battler, BattleManager battleManager) => false;
    }

    /// <summary>
    /// Data that holds all the information about a transformation.
    /// This is basically a bunch of data that has to be overriden instead of using the base data from the battler.
    /// </summary>
    public class TransformationData
    {
        /// <summary>
        /// Original name of the battler.
        /// </summary>
        public string Name;

        /// <summary>
        /// New stats of the monster.
        /// </summary>
        public Dictionary<Stat, uint> Stats;

        /// <summary>
        /// Types the battler will have.
        /// </summary>
        public (MonsterType, MonsterType) Types;

        /// <summary>
        /// Original catch rate of the monster.
        /// </summary>
        public byte CatchRate;

        /// <summary>
        /// New weight of the monster.
        /// </summary>
        public float Weight;

        /// <summary>
        /// New height of the monster.
        /// </summary>
        public float Height;

        /// <summary>
        /// New gender of the monster.
        /// </summary>
        public MonsterGender Gender;
    }
}