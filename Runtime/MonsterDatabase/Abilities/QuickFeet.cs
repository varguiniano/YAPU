using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the QuickFeet ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/QuickFeet", fileName = "QuickFeet")]
    public class QuickFeet : Ability
    {
        /// <summary>
        /// Double attack when having a status.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            (stat == Stat.Speed && monster.GetStatus() != null ? 1.5f : 1)
          * base.OnCalculateStat(monster, stat, battleManager);

        /// <summary>
        /// Not affected.
        /// </summary>
        public override bool IsAffectedByParalysisSpeedReduction(Battler battler, BattleManager battleManager) => false;

        /// <summary>
        /// Called when the encounter chances are calculated and modifies them.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <param name="encounterType">Type of encounter to calculate.</param>
        public override float
            OnCalculateEncounterChance(PlayerCharacter playerCharacter, EncounterType encounterType) =>
            0.125f;
    }
}