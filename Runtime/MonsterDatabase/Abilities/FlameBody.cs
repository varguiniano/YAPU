using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities
{
    /// <summary>
    /// FlameBody ability implementation.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Abilities/FlameBody", fileName = "FlameBody")]
    public class FlameBody : SetStatusOnContactAbility
    {
        /// <summary>
        /// Half the steps needed for an egg to hatch.
        /// </summary>
        public override float ModifyStepsNeededForEggCycle(MonsterInstance owner) => .5f;
    }
}