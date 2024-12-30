using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.World.Encounters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the Hustle ability.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/Hustle", fileName = "Hustle")]
    public class Hustle : Ability
    {
        /// <summary>
        /// Increases attack by 50%.
        /// </summary>
        public override float OnCalculateStat(MonsterInstance monster, Stat stat, BattleManager battleManager) =>
            (stat == Stat.Attack ? 1.5f : 1) * base.OnCalculateStat(monster, stat, battleManager);

        /// <summary>
        /// Decrease physical move accuracy by 20%.
        /// </summary>
        public override float GetMoveAccuracyMultiplierWhenUsed(BattleManager battleManager,
                                                                Battler user,
                                                                Battler target,
                                                                Move move,
                                                                bool ignoresAbilities) =>
            move.GetMoveCategory(user, target, ignoresAbilities, battleManager) == Move.Category.Physical ? 0.8f : 1;

        /// <summary>
        /// Only use the upper half.
        /// </summary>
        /// <param name="monster">Owner of the ability.</param>
        /// <param name="encounter">Encounter type.</param>
        /// <param name="minimum">Minimum level.</param>
        /// <param name="maximum">Maximum level.</param>
        /// <returns>The new limits.</returns>
        public override (byte minimum, byte maximum)
            ModifyEncounterLevels(MonsterInstance monster, EncounterType encounter, byte minimum, byte maximum) =>
            ((byte) (minimum + (maximum - minimum) / 2f), maximum);
    }
}