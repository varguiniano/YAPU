using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// Data class for the ability HeavyMetal.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/HeavyMetal", fileName = "HeavyMetal")]
    public class HeavyMetal : Ability
    {
        /// <summary>
        /// Half the weight of the monster.
        /// </summary>
        /// <param name="monster">The owner of the ability.</param>
        /// <returns>The modifier and the multiplier to apply to the weight.</returns>
        public override (float, float) GetMonsterWeight(MonsterInstance monster) => (0, 2f);

        /// <summary>
        /// Half the weight of the monster.
        /// </summary>
        /// <param name="battler">The owner of the ability.</param>
        /// <returns>The modifier and the multiplier to apply to the weight.</returns>
        public override (float, float) GetMonsterWeightInBattle(Battler battler) => (0, 2f);
    }
}