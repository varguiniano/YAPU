using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Sprite controller of a ball inside a battle.
    /// </summary>
    public class BattleBallSprite : WhateverBehaviour<BattleBallSprite>
    {
        /// <summary>
        /// Reference to the sprite transform.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform SpriteTransform;

        /// <summary>
        /// Reference to the attached sprite renderer.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private SpriteRenderer SpriteRenderer;

        /// <summary>
        /// Reference to the catch effect.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private VisualEffect CatchEffect;

        /// <summary>
        /// Material property block to change the sprite.
        /// </summary>
        private MaterialPropertyBlock propertyBlock;

        /// <summary>
        /// Create the property block.
        /// </summary>
        private void OnEnable() => propertyBlock = new MaterialPropertyBlock();

        /// <summary>
        /// Cached sprite index property name.
        /// </summary>
        private static readonly int SpriteIndex = Shader.PropertyToID("_SpriteIndex");

        /// <summary>
        /// Kill tweens when disabled.
        /// </summary>
        private void OnDisable() => SpriteTransform.DOKill();

        /// <summary>
        /// Update the sprite's flipbook to the given index.
        /// </summary>
        /// <param name="ball">Ball to use.</param>
        /// <param name="index">Index of the flipbook.</param>
        public void UpdateFlipbook(Ball ball, int index)
        {
            SpriteRenderer.sharedMaterial = ball.AnimationFlipBook;

            SpriteRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetInt(SpriteIndex, index);

            SpriteRenderer.SetPropertyBlock(propertyBlock);
        }

        /// <summary>
        /// Play the catch animation.
        /// </summary>
        public void PlayCatchAnimation() => CatchEffect.enabled = true;

        /// <summary>
        /// Fade the ball out.
        /// </summary>
        /// <returns></returns>
        public void FadeOut() => SpriteRenderer.DOFade(0, .2f);
    }
}