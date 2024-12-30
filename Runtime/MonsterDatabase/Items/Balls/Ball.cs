using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls
{
    /// <summary>
    /// Data class representing a ball.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Balls/NormalBall", fileName = "NormalBall")]
    public class Ball : Item
    {
        /// <summary>
        /// Reference to its animation flipbook.
        /// </summary>
        [FoldoutGroup("Graphics")]
        [PreviewField(100)]
        public Material AnimationFlipBook;

        /// <summary>
        /// This ball will never fail.
        /// </summary>
        [FoldoutGroup("Capture")]
        public bool NeverFails;

        /// <summary>
        /// Basic multiplier to directly apply to the ball.
        /// Override this class for multipliers depending on the battler.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        protected float BasicCatchMultiplier = 1f;

        /// <summary>
        /// Multiplier to use when catching ultra beasts.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        [HideIf(nameof(NeverFails))]
        protected float UltraBeastMultiplier = .1f;

        /// <summary>
        /// Friendship to be applied every time the ball owner changes their friendship.
        /// </summary>
        [FoldoutGroup("Ball Modifiers")]
        [SerializeField]
        protected int FriendshipIncreaseModifier;

        /// <summary>
        /// Get the catch multiplier of this ball based on the battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler to check.</param>
        /// <param name="ownBattler">Reference to our own battler.</param>
        /// <param name="callback"></param>
        /// <returns>A float with the multiplier.</returns>
        public virtual IEnumerator GetCatchMultiplier(BattleManager battleManager,
                                                      Battler battler,
                                                      Battler ownBattler,
                                                      Action<float> callback)
        {
            callback.Invoke(GetCatchMultiplierOutOfBattle(battler.FormData));
            yield break;
        }

        /// <summary>
        /// Return the catch multiplier out of battle, useful for the dex.
        /// </summary>
        public float GetCatchMultiplierOutOfBattle(DataByFormEntry target) =>
            target.IsUltraBeast ? UltraBeastMultiplier : BasicCatchMultiplier;

        /// <summary>
        /// Get a number to add instead of a multiplier to the formula based on the battler.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Battler to check.</param>
        /// <param name="ownBattler">Reference to our own battler.</param>
        /// <returns>A number to add to the formula.</returns>
        public virtual float GetCatchAddition(BattleManager battleManager, Battler battler, Battler ownBattler) => 0;

        /// <summary>
        /// Used to modify the battler after being captured.
        /// </summary>
        /// <param name="battler">Reference to the captured battler.</param>
        public virtual void AfterCapture(ref Battler battler) => battler.OriginData.Ball = this;

        /// <summary>
        /// Get the modifier this ball applies to its owner when it changes friendship.
        /// </summary>
        /// <returns>The modifier to add.</returns>
        public virtual int GetFriendShipModifier() => FriendshipIncreaseModifier;

        /// <summary>
        /// Calls can stack.
        /// </summary>
        public override bool CanStack => true;

        /// <summary>
        /// Can't be used outside battle.
        /// </summary>
        public override bool CanBeUsed => false;

        /// <summary>
        /// This type of items can't be registered.
        /// </summary>
        public override bool CanBeRegistered => false;

        /// <summary>
        /// Balls are used without a target.
        /// </summary>
        public override bool CanBeUsedOnTarget => false;

        /// <summary>
        /// Balls can't be used on moves.
        /// </summary>
        public override bool CanBeUsedOnTargetMove => false;

        /// <summary>
        /// Balls are used in battle without a target.
        /// </summary>
        public override bool CanBeUsedInBattle => true;

        /// <summary>
        /// Balls are used without a target.
        /// </summary>
        public override bool CanBeUsedInBattleOnTarget => false;

        /// <summary>
        /// Balls can't be used on moves.
        /// </summary>
        public override bool CanBeUsedInBattleOnTargetMove => false;
    }
}