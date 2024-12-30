using System;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Random = UnityEngine.Random;

namespace Varguiniano.YAPU.Runtime.Monster
{
    /// <summary>
    /// Physical data of a monster instance.
    /// </summary>
    [Serializable]
    public struct MonsterPhysicalData
    {
        /// <summary>
        /// Modifier to apply to the species height.
        /// </summary>
        public float HeightModifier;

        /// <summary>
        /// Modifier to apply to the species weight.
        /// </summary>
        public float WeightModifier;

        /// <summary>
        /// Gender this monster has.
        /// </summary>
        public MonsterGender Gender;

        /// <summary>
        /// Create the monster's physical data.
        /// Height and weight are generated from the monster's species.
        /// </summary>
        /// <param name="settings">Internal settings for YAPU reference.</param>
        /// <param name="data">Monster species and form data.</param>
        /// <param name="gender">Monster gender.</param>
        /// <param name="heightModifierOverride">Override for the height multiplier.</param>
        /// <param name="weightModifierOverride">Override for the weight modifier.</param>
        internal MonsterPhysicalData(YAPUSettings settings,
                                     DataByFormEntry data,
                                     MonsterGender gender,
                                     float heightModifierOverride,
                                     float weightModifierOverride)
        {
            HeightModifier = heightModifierOverride < 0
                                 ? 1 + Random.Range(-settings.HeightVariability, settings.HeightVariability)
                                 : heightModifierOverride;

            WeightModifier = weightModifierOverride < 0
                                 ? 1 + Random.Range(-settings.WeightVariability, settings.WeightVariability)
                                 : weightModifierOverride;

            if (data.HasBinaryGender)
            {
                if (gender != MonsterGender.NonBinary)
                    Gender = gender;
                else
                    Gender = Random.Range(0f, 1f) <= data.FemaleRatio ? MonsterGender.Female : MonsterGender.Male;
            }
            else
                Gender = MonsterGender.NonBinary;
        }
    }
}