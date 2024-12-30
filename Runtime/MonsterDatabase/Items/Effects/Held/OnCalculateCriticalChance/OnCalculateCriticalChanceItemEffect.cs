using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Effects.Held.OnCalculateCriticalChance
{
    /// <summary>
    /// Base class for item effects that modify the critical chance when using a move.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Effects/Held/OnCalculateCriticalChanceItemEffect/SimpleModifier",
                     fileName = "OnCalculateCriticalChanceItemEffect")]
    public class OnCalculateCriticalChanceItemEffect : MonsterDatabaseScriptable<OnCalculateCriticalChanceItemEffect>
    {
        /// <summary>
        /// Multiplier to apply.
        /// </summary>
        [SerializeField]
        protected float Multiplier;

        /// <summary>
        /// Number of stages this move adds to the critical stage.
        /// </summary>
        [PropertyRange(0, 3)]
        [SerializeField]
        protected byte CriticalStageModifier;

        /// <summary>
        /// Always land a critical with this move?
        /// </summary>
        [SerializeField]
        protected bool AlwaysHit;

        /// <summary>
        /// Should the item be consumed upon use?
        /// </summary>
        [SerializeField]
        protected bool ShouldConsume;

        /// <summary>
        /// Called when calculating the critical chance of this battler.
        /// </summary>
        /// <param name="owner">Owner of the ability.</param>
        /// <param name="item">Item this effect belongs to.</param>
        /// <param name="target">Target of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="move">Move being used.</param>
        /// <param name="multiplier">Multiplier to apply to the chance.</param>
        /// <param name="modifier">Critical stage modifier to use.</param>
        /// <param name="alwaysHit">Change it to always hit?</param>
        /// <param name="shouldConsume">Should the item be consumed upon use?</param>
        /// <returns>Has the chance been changed?</returns>
        public virtual bool OnCalculateCriticalChance(Battler owner,
                                                      Item item,
                                                      Battler target,
                                                      BattleManager battleManager,
                                                      Move move,
                                                      ref float multiplier,
                                                      ref byte modifier,
                                                      ref bool alwaysHit,
                                                      out bool shouldConsume)
        {
            multiplier *= Multiplier;
            modifier += CriticalStageModifier;
            alwaysHit |= AlwaysHit;
            
            shouldConsume = ShouldConsume;

            return true;
        }
    }
}