using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Class representing the move DazzlingGleam.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Fighting/FlyingPress", fileName = "FlyingPress")]
    public class FlyingPress : DamageMove
    {
        // TODO: Animation.

        /// <summary>
        /// Secondary type to affect.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMonsterTypes))]
        #endif
        private MonsterType SecondaryType;

        /// <summary>
        /// Calculate the effectiveness of the move.
        /// Secondary also affects.
        /// </summary>
        protected override float CalculateEffectiveness(Battler user,
                                                        Battler target,
                                                        bool ignoresAbilities,
                                                        BattleManager battleManager)
        {
            float effectiveness = base.CalculateEffectiveness(user, target, ignoresAbilities, battleManager);

            effectiveness *=
                target.GetEffectivenessOfType(SecondaryType, battleManager, true, user, this, ignoresAbilities);

            // Max 2.
            return Mathf.Clamp(effectiveness, 0, 2);
        }
    }
}