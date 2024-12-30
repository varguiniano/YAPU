using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Toxic.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Poison/Toxic", fileName = "Toxic")]
    public class Toxic : PowderMove
    {
        /// <summary>
        /// Reference to the poison type.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        [FoldoutGroup("Effect")]
        [SerializeField]
        private MonsterType PoisonType;

        /// <summary>
        /// Will never miss if used by a poison type mon.
        /// </summary>
        /// <param name="user">User of the move.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The accuracy.</returns>
        public override float CalculateAccuracy(Battler user,
                                                Battler target,
                                                bool ignoresAbilities,
                                                BattleManager battleManager) =>
            user.IsOfType(PoisonType, battleManager.YAPUSettings)
                ? 100
                : base.CalculateAccuracy(user, target, ignoresAbilities, battleManager);
    }
}