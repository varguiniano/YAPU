using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class for a ball that heals the monster after capturing it.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallThatHealsAfterCapture", fileName = "BallThatHealsAfterCapture")]
    public class BallThatHealsAfterCapture : Ball
    {
        /// <summary>
        /// Used to modify the battler after being captured.
        /// </summary>
        /// <param name="battler">Reference to the captured battler.</param>
        public override void AfterCapture(ref Battler battler)
        {
            base.AfterCapture(ref battler);

            battler.CompletelyHeal();
        }
    }
}