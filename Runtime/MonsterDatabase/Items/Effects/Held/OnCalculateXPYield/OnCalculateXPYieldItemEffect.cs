using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Player;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateXPYield
{
    /// <summary>
    /// Data class representing a held item effect that is called when calculating XP to yield.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateXPYieldItemEffect/BasicMultiplier",
                     fileName = "OnCalculateXPYieldItemEffect")]
    public class OnCalculateXPYieldItemEffect : MonsterDatabaseScriptable<OnCalculateXPYieldItemEffect>
    {
        /// <summary>
        /// Modifier to apply.
        /// </summary>
        [SerializeField]
        private float Modifier = 1f;

        /// <summary>
        /// Calculate the object modifier to apply to the XP yield.
        /// </summary>
        /// <param name="settings">Reference to the YAPU settings.</param>
        /// <param name="playerSettings">Reference to the player settings.</param>
        /// <param name="enemyType">The type of enemy it was.</param>
        /// <param name="faintedBattler">Reference to the fainted battler.</param>
        /// <param name="receiver">Reference to the receiver battler.</param>
        public float CalculateXPYieldModifier(YAPUSettings settings,
                                              PlayerSettings playerSettings,
                                              EnemyType enemyType,
                                              Battler faintedBattler,
                                              Battler receiver) =>
            Modifier;
    }
}