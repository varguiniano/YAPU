using System;

namespace Varguiniano.YAPU.Runtime.Monster.Evolution
{
    /// <summary>
    /// Struct that stores necessary data for specific monsters.
    /// </summary>
    [Serializable]
    public struct ExtraData
    {
        /// <summary>
        /// Value that determines the personality of the monster.
        /// This is used for specific stuff like Dunsparce/Wurmple evolutions or Spinda patterns.
        /// </summary>
        public int PersonalityValue;
        
        /// <summary>
        /// Flag marking if the monster needs to check its level up evolutions.
        /// </summary>
        public bool NeedsLevelUpEvolutionCheck;

        /// <summary>
        /// Counter for monsters that evolve after performing certain actions that need to be counted.
        /// For example, Primeape evolves after using RageFist 20 times.
        /// </summary>
        public uint EvolutionCounter;

        /// <summary>
        /// Times it landed a critical hit in the last battle.
        /// </summary>
        public uint TimesLandedCriticalHitLastBattle;

        /// <summary>
        /// Reset data used to check evolution.
        /// </summary>
        public void ResetEvolutionData()
        {
            NeedsLevelUpEvolutionCheck = false;
            EvolutionCounter = 0;
        }

        /// <summary>
        /// Reset data that must be reset after a battle.
        /// </summary>
        public void ResetBattleData() => TimesLandedCriticalHitLastBattle = 0;
    }
}