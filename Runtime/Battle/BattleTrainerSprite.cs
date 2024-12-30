using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Class to handle the sprites of trainers fighting in a battle.
    /// </summary>
    public class BattleTrainerSprite : WhateverBehaviour<BattleTrainerSprite>
    {
        /// <summary>
        /// Reference to the trainer transform.
        /// </summary>
        [SerializeField]
        private Transform TrainerTransform;

        /// <summary>
        /// Reference to the trainer's sprite renderer.
        /// </summary>
        [SerializeField]
        private SpriteRenderer TrainerSpriteRenderer;

        /// <summary>
        /// Target transform for sliding back.
        /// </summary>
        [SerializeField]
        private Transform OriginalPosition;

        /// <summary>
        /// Target transform for sliding.
        /// </summary>
        [SerializeField]
        private Transform SlideTarget;

        /// <summary>
        /// Duration of the slide animation.
        /// </summary>
        [SerializeField]
        private float SlideDuration = 2f;

        /// <summary>
        /// Duration of the throw animation.
        /// </summary>
        [SerializeField]
        private float AnimationDuration = 1f;

        /// <summary>
        /// Update a flipbook in the material when moving?
        /// </summary>
        [SerializeField]
        private bool UseFlipbook;

        /// <summary>
        /// Steps the trainer flipbook has.
        /// </summary>
        [SerializeField]
        [ShowIf(nameof(UseFlipbook))]
        private int FlipbookSteps = 5;

        /// <summary>
        /// Material property block to use with the trainer sprite renderer.
        /// </summary>
        private MaterialPropertyBlock materialPropertyBlock;

        /// <summary>
        /// Material index for the flipbook index.
        /// </summary>
        private static readonly int Index = Shader.PropertyToID("_SpriteIndex");

        /// <summary>
        /// Initialize the material property block.
        /// </summary>
        private void OnEnable() => materialPropertyBlock = new MaterialPropertyBlock();

        /// <summary>
        /// Set the trainer's sprite.
        /// </summary>
        /// <param name="sprite"></param>
        [Button]
        public void SetSprite(Sprite sprite) => TrainerSpriteRenderer.sprite = sprite;

        /// <summary>
        /// Set the trainer's material.
        /// </summary>
        /// <param name="material"></param>
        [Button]
        public void SetMaterial(Material material) => TrainerSpriteRenderer.material = material;

        /// <summary>
        /// Throw the ball and slide sideways.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="throwCallback">Called when it's time to throw.</param>
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void ThrowAndSlide(float speed, Action throwCallback)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> tween = Slide(speed);

            bool thrown = false;

            tween.onUpdate += () =>
                              {
                                  float elapsed = tween.Elapsed();

                                  if (!thrown && elapsed > 0.4f / speed)
                                  {
                                      throwCallback?.Invoke();
                                      thrown = true;
                                  }

                                  if (UseFlipbook) UpdateFlipbook(FlipbookSteps * elapsed / AnimationDuration / speed);
                              };
        }

        /// <summary>
        /// Slide sideways.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// </summary>
        [Button]
        [HideInEditorMode]
        public TweenerCore<Vector3, Vector3, VectorOptions> Slide(float speed) =>
            TrainerTransform.DOMove(SlideTarget.position, SlideDuration / speed);

        /// <summary>
        /// Slide back in scene.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void SlideBack(float speed)
        {
            TrainerSpriteRenderer.enabled = true;

            TrainerTransform.DOMove(OriginalPosition.position, .75f / speed);
        }

        /// <summary>
        /// Immediately hide to the side and disable the renderer.
        /// </summary>
        public void Hide()
        {
            TrainerTransform.position = SlideTarget.position;

            TrainerSpriteRenderer.enabled = false;
        }

        /// <summary>
        /// Update the flipbook with a new step.
        /// </summary>
        /// <param name="step">Step to set.</param>
        private void UpdateFlipbook(float step)
        {
            TrainerSpriteRenderer.GetPropertyBlock(materialPropertyBlock);

            materialPropertyBlock.SetFloat(Index, step);

            TrainerSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}