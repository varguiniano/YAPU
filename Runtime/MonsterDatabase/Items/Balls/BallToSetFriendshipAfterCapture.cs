using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class for a ball that sets the friendship value after capturing the monster.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/BallToSetFriendshipAfterCapture",
                     fileName = "BallToSetFriendshipAfterCapture")]
    public class BallToSetFriendshipAfterCapture : Ball
    {
        /// <summary>
        /// Friendship to set after capture.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private byte FriendshipToSet = 200;

        /// <summary>
        /// Used to modify the battler after being captured.
        /// </summary>
        /// <param name="battler">Reference to the captured battler.</param>
        public override void AfterCapture(ref Battler battler)
        {
            base.AfterCapture(ref battler);

            battler.Friendship = FriendshipToSet;
        }
    }
}