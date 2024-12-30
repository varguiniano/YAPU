using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Animation;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Monsters;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Behaviour to control the sprites of a battler.
    /// </summary>
    [ExecuteInEditMode]
    public class BattleMonsterSprite : WhateverBehaviour<BattleMonsterSprite>
    {
        /// <summary>
        /// Pivot of the sprite.
        /// </summary>
        [FoldoutGroup("References")]
        public Transform Pivot;

        /// <summary>
        /// Pivot to use to apply the size of the monster.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform SizePivot;

        /// <summary>
        /// Reference to the monster renderer.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private SpriteRenderer MonsterSpriteRenderer;

        /// <summary>
        /// Reference to the shadow renderer.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private SpriteRenderer ShadowSpriteRenderer;

        /// <summary>
        /// Reference to the ball prefab.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BattleBallSprite BallPrefab;

        /// <summary>
        /// Reference to the FXAnimator.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        public MonsterFXAnimator FXAnimator;

        /// <summary>
        /// Star to show that this monster is shinny.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        public Transform ShinnyStar;

        /// <summary>
        /// Reference to the ball burst prefab.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BallBurst BallBurstPrefab;

        /// <summary>
        /// Reference to the substitute material.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Material SubstituteFront;

        /// <summary>
        /// Reference to the substitute material.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Material SubstituteBack;

        /// <summary>
        /// Shrink and grow animation durations.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private float ShrinkAndGrowDuration = .5f;

        /// <summary>
        /// Duration of the ball drop animation.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private float BallDropDuration = 2f;

        /// <summary>
        /// Easing for the ball drop animation.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private Ease BallDropEasing = Ease.Linear;

        /// <summary>
        /// Path for the balls to follow when thrown.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private List<Transform> BallPath;

        /// <summary>
        /// Steps for the open flipbook animation.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private float OpenFlipbookSteps;

        /// <summary>
        /// First step of the close flipbook animation.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private int CloseFlipbookStepsFirstStep;

        /// <summary>
        /// Last step of the close flipbook animation.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private int CloseFlipbookStepsLastStep;

        /// <summary>
        /// Close animation duration.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private float CloseDuration;

        /// <summary>
        /// Position to move to when sliding out.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private Transform SlideOutPosition;

        /// <summary>
        /// Position to move to when sliding in.
        /// </summary>
        [FoldoutGroup("Animation Config")]
        [SerializeField]
        private Transform SlideInPosition;

        /// <summary>
        /// Audio for the throw ball animation.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference ThrowBallAudio;

        /// <summary>
        /// Audio for the open ball animation.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference OpenBallAudio;

        /// <summary>
        /// Audio for the close ball animation.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference CloseBallAudio;

        /// <summary>
        /// Audio for the shinny effect.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference ShinnyAudio;

        /// <summary>
        /// Audio for the chew effect.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        private AudioReference ChewAudio;

        /// <summary>
        /// Should the sprite reset its position when a move fails?
        /// </summary>
        public bool ShouldResetOnMoveFail;

        /// <summary>
        /// Array of positions for the ball path.
        /// </summary>
        private Vector3[] ballPathArray;

        /// <summary>
        /// Reference to the current battler.
        /// </summary>
        private Battler battler;

        /// <summary>
        /// Is the substitute currently shown?
        /// </summary>
        private bool substituteShown;

        /// <summary>
        /// Is this sprite for the front or the back of the monster?
        /// </summary>
        private bool isFront;

        /// <summary>
        /// Previous sorting order of the sprite.
        /// </summary>
        private int previousSortingOrder;

        /// <summary>
        /// Is the sprite currently hidden or invisible?
        /// </summary>
        private bool isCurrentlyHidden;

        /// <summary>
        /// Position to be in normally.
        /// </summary>
        private readonly Vector3 basePosition = new(0, -.5f, 0);

        /// <summary>
        /// Position to be in when digging.
        /// </summary>
        private readonly Vector3 digPosition = new(0, -.8f, 0);

        /// <summary>
        /// Position to be in when flying.
        /// </summary>
        private readonly Vector3 flyPosition = new(0, 2f, 0);

        /// <summary>
        /// Reference tot he audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the YAPU settings.
        /// </summary>
        [Inject]
        private YAPUSettings yapuSettings;

        /// <summary>
        /// Reference to the monster property block.
        /// </summary>
        private MaterialPropertyBlock monsterPropertyBlock;

        /// <summary>
        /// Reference to the shadow property block.
        /// </summary>
        private MaterialPropertyBlock shadowPropertyBlock;

        /// <summary>
        /// Material index for shadow.
        /// </summary>
        private static readonly int IsShadow = Shader.PropertyToID("_IsShadow");

        /// <summary>
        /// Prepare the ball path array.
        /// </summary>
        private void OnEnable()
        {
            if (BallPath != null) ballPathArray = BallPath.Select(x => x.position).ToArray();

            previousSortingOrder = MonsterSpriteRenderer.sortingOrder;

            monsterPropertyBlock = new MaterialPropertyBlock();
            shadowPropertyBlock = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Set a monster´s sprite.
        /// </summary>
        /// <param name="newBattler">Monster to set.</param>
        /// <param name="front">Is it the front sprite?</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        [Button]
        public void SetMonster(Battler newBattler, bool front, BattleManager battleManager)
        {
            battler = newBattler;
            isFront = front;

            if (battler.IsNullEntry)
            {
                Logger.Error("This isn't a valid battler!");
                return;
            }

            UpdateMaterial(battleManager);
        }

        /// <summary>
        /// Update the material of the sprite with the monster material.
        /// </summary>
        private void UpdateMaterial(BattleManager battleManager)
        {
            DataByFormEntry data = battler.FormData;

            Material material;

            if (isFront)
                if (battler.Form.IsShiny)
                    material =
                        battler.PhysicalData.Gender == MonsterGender.Male && data.HasMaleMaterialOverride
                            ? data.FrontShinyMale
                            : data.FrontShiny;
                else
                    material =
                        battler.PhysicalData.Gender == MonsterGender.Male && data.HasMaleMaterialOverride
                            ? data.FrontMale
                            : data.Front;
            else if (battler.Form.IsShiny)
                material =
                    battler.PhysicalData.Gender == MonsterGender.Male && data.HasMaleMaterialOverride
                        ? data.BackShinyMale
                        : data.BackShiny;
            else
                material =
                    battler.PhysicalData.Gender == MonsterGender.Male && data.HasMaleMaterialOverride
                        ? data.BackMale
                        : data.Back;

            MonsterSpriteRenderer.material = material;
            ShadowSpriteRenderer.material = material;

            SizePivot.localScale = MonsterMathHelper.CalculateBattleSize(battler, battleManager);

            MonsterSpriteRenderer.GetPropertyBlock(monsterPropertyBlock);

            MonsterMathHelper.AddAdditionalMaterialProperties(battler, isFront, ref monsterPropertyBlock, yapuSettings);

            MonsterSpriteRenderer.SetPropertyBlock(monsterPropertyBlock);

            ShadowSpriteRenderer.GetPropertyBlock(shadowPropertyBlock);

            shadowPropertyBlock.SetFloat(IsShadow, 1);
            MonsterMathHelper.AddAdditionalMaterialProperties(battler, isFront, ref shadowPropertyBlock, yapuSettings);

            ShadowSpriteRenderer.SetPropertyBlock(shadowPropertyBlock);

            if (!isCurrentlyHidden) return;

            MonsterSpriteRenderer.DOFade(0, 0);
            ShadowSpriteRenderer.DOFade(0, 0);
        }

        /// <summary>
        /// Update the material of the sprite with the substitute material.
        /// </summary>
        private void UpdateMaterialToSubstitute()
        {
            if (isFront)
            {
                MonsterSpriteRenderer.material = SubstituteFront;
                ShadowSpriteRenderer.material = SubstituteFront;
            }
            else
            {
                MonsterSpriteRenderer.material = SubstituteBack;
                ShadowSpriteRenderer.material = SubstituteBack;
            }

            MaterialPropertyBlock propertyBlock = new();

            ShadowSpriteRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetFloat(IsShadow, 1);

            ShadowSpriteRenderer.SetPropertyBlock(propertyBlock);

            if (!isCurrentlyHidden) return;

            MonsterSpriteRenderer.DOFade(1, 0);
            ShadowSpriteRenderer.DOFade(1, 0);
        }

        /// <summary>
        /// Shrink the monster to 0 scale.
        /// </summary>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="immediately">Do it immediately or tween?</param>
        /// <param name="finished">Callback when complete.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void Shrink(float speed, bool immediately = false, Action finished = null) =>
            ChangeSize(speed, Vector3.zero, immediately, finished);

        /// <summary>
        /// Enlarge the monster to 1 scale.
        /// </summary>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="immediately"></param>
        /// <param name="finished">Callback when complete.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void Enlarge(float speed, bool immediately = false, Action finished = null) =>
            ChangeSize(speed, Vector3.one, immediately, finished);

        /// <summary>
        /// Enlarge the monster to 1 scale while playing vfx..
        /// </summary>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="finished">Callback when complete.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void EnlargeWithVFX(float speed, Action finished = null)
        {
            Shrink(speed, true);

            BallBurst ballBurst = Instantiate(BallBurstPrefab, Pivot);

            ballBurst.PlayAnimation(speed);

            Enlarge(speed,
                    finished: () =>
                              {
                                  finished?.Invoke();

                                  // Destroy the ball burst object after a while.
                                  DOVirtual.DelayedCall(3,
                                                        () =>
                                                        {
                                                            ballBurst.DOKill();
                                                            Destroy(ballBurst.gameObject);
                                                        });
                              });
        }

        /// <summary>
        /// Throw the ball, show up from the ball and enlarge.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="finished">Finished callback.</param>
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        public void DropBallAndEnlarge(float speed, Action finished = null)
        {
            Shrink(speed, true);

            DropBall(speed,
                     () =>
                     {
                         audioManager.PlayAudio(OpenBallAudio, pitch: speed);

                         BallBurst ballBurst = Instantiate(BallBurstPrefab, Pivot);

                         ballBurst.PlayAnimation(speed);

                         Enlarge(speed,
                                 finished: () =>
                                           {
                                               finished?.Invoke();

                                               // Give a second to the rest of effects.
                                               DOVirtual.DelayedCall(1, ShinnyEffects);

                                               // Destroy the ball burst object after a while.
                                               DOVirtual.DelayedCall(3,
                                                                     () =>
                                                                     {
                                                                         if (ballBurst == null) return;

                                                                         ballBurst.DOKill();
                                                                         Destroy(ballBurst.gameObject);
                                                                     });
                                           });
                     });
        }

        /// <summary>
        /// Shrink the mon and close the ball.
        /// </summary>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="finished">Finished callback.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void ShrinkAndCloseBall(float speed, Action finished = null)
        {
            StartCoroutine(FXAnimator.PlayAbsorb(-1, Pivot.position));

            audioManager.PlayAudio(CloseBallAudio, pitch: speed);

            Shrink(speed,
                   finished: () =>
                             {
                                 StartCoroutine(FXAnimator.StopAbsorb());

                                 StartCoroutine(CloseBall(speed,
                                                          () => finished?.Invoke()));
                             });
        }

        /// <summary>
        /// Shrink the mon while moving towards a target.
        /// </summary>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="target">Target to move towards.</param>
        /// <param name="finished">Finished callback.</param>
        [FoldoutGroup("Debug")]
        [Button]
        public void ShrinkWithVFXTowardsTarget(float speed, Vector3 target, Action finished = null)
        {
            StartCoroutine(FXAnimator.PlayAbsorb(-1, Pivot.position));

            audioManager.PlayAudio(CloseBallAudio, pitch: speed);

            Pivot.DOMove(target, ShrinkAndGrowDuration / speed);

            Shrink(speed,
                   finished: () =>
                             {
                                 StartCoroutine(FXAnimator.StopAbsorb());

                                 finished?.Invoke();
                             });
        }

        /// <summary>
        /// Play shinny effects if necessary.
        /// </summary>
        public void ShinnyEffects()
        {
            if (!battler.Form.IsShiny) return;

            ShinnyStar.DOScale(Vector3.one, .25f).OnComplete(() => ShinnyStar.DOScale(Vector3.zero, .25f));

            audioManager.PlayAudio(ShinnyAudio);
        }

        /// <summary>
        /// Slide out of the screen.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// </summary>
        public IEnumerator SlideOut(float speed)
        {
            bool finished = false;

            Pivot.DOMove(SlideOutPosition.position, 1f / speed).SetEase(Ease.InBack).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }

        /// <summary>
        /// Slide out in the screen.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// </summary>
        private IEnumerator SlideIn(float speed)
        {
            bool finished = false;

            Pivot.DOMove(SlideInPosition.position, 1f / speed).SetEase(Ease.OutBack).OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);
        }

        /// <summary>
        /// Change the size of the sprite.
        /// </summary>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="targetSize">New size.</param>
        /// <param name="immediately">Do it immediately or tween?</param>
        /// <param name="finished">Finished callback.</param>
        private void ChangeSize(float speed, Vector3 targetSize, bool immediately = false, Action finished = null)
        {
            if (immediately)
                Pivot.localScale = targetSize;
            else
                Pivot.DOScale(targetSize, ShrinkAndGrowDuration / speed).OnComplete(() => finished?.Invoke());
        }

        /// <summary>
        /// Drop a ball on top of this transform.
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="finished">Event raised when finished.</param>
        /// </summary>
        private void DropBall(float speed, Action finished)
        {
            if (battler.IsNullEntry)
            {
                Logger.Error("This isn't a valid battler!");
                return;
            }

            StartCoroutine(DropBallRoutine(speed, finished));
        }

        /// <summary>
        /// Drop a ball on top of this transform.
        /// </summary>
        private IEnumerator DropBallRoutine(float speed, Action finished)
        {
            audioManager.PlayAudio(ThrowBallAudio, pitch: speed);

            yield return new WaitForSeconds(.2f);

            BattleBallSprite ball = Instantiate(BallPrefab, BallPath[0].position, Quaternion.identity);

            yield return WaitAFrame;

            TweenerCore<Vector3, Path, PathOptions> tween = ball.GetCachedComponent<Transform>()
                                                                .DOPath(ballPathArray,
                                                                        BallDropDuration / speed,
                                                                        PathType.CatmullRom,
                                                                        PathMode.Ignore)
                                                                .SetOptions(false,
                                                                            lockRotation: AxisConstraint.None)
                                                                .SetEase(BallDropEasing)
                                                                .OnComplete(() =>
                                                                            {
                                                                                finished?.Invoke();
                                                                                Destroy(ball.gameObject);
                                                                            });

            tween.OnUpdate(() => ball.UpdateFlipbook(battler.OriginData.Ball,
                                                     (int) (OpenFlipbookSteps
                                                          * tween.Elapsed()
                                                          / BallDropDuration
                                                          / speed)));
        }

        /// <summary>
        /// Play a close ball animation, then destroy the ball.
        /// </summary>
        private IEnumerator CloseBall(float speed, Action finished)
        {
            int totalSteps = CloseFlipbookStepsFirstStep - CloseFlipbookStepsLastStep + 1;

            float interval = CloseDuration / totalSteps / speed;

            BattleBallSprite ball = Instantiate(BallPrefab, Pivot.position, Quaternion.identity);

            yield return WaitAFrame;

            for (int i = 0; i < totalSteps; ++i)
            {
                ball.UpdateFlipbook(battler.OriginData.Ball, CloseFlipbookStepsFirstStep - i);

                yield return new WaitForSeconds(interval);
            }

            Destroy(ball.gameObject);

            finished?.Invoke();
        }

        /// <summary>
        /// Animation to eat a berry.
        /// </summary>
        /// <param name="speed">Speed at which the animation is performed.</param>
        [FoldoutGroup("Debug")]
        [Button]
        private void TestEatBerry(float speed = 1) => StartCoroutine(EatBerry(speed));

        /// <summary>
        /// Animation to eat a berry.
        /// </summary>
        /// <param name="speed">Speed at which the animation is performed.</param>
        public IEnumerator EatBerry(float speed)
        {
            audioManager.PlayAudio(ChewAudio, pitch: speed);

            bool moved = false;

            Pivot.DOJump(Pivot.position, .1f, 3, 1 / speed).SetEase(Ease.Linear).OnComplete(() => moved = true);

            yield return new WaitUntil(() => moved);

            moved = false;

            FXAnimator.PlayBoost(speed, finished: () => moved = true);

            yield return new WaitUntil(() => moved);
        }

        /// <summary>
        /// Show a substitute instead of the monster.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        private void ShowSubstituteTest() => StartCoroutine(ShowSubstitute(1));

        /// <summary>
        /// Show a substitute instead of the monster.
        /// </summary>
        /// <param name="speed">Speed at which the animation is performed.</param>
        public IEnumerator ShowSubstitute(float speed)
        {
            if (substituteShown) yield break;

            yield return SlideOut(speed);

            UpdateMaterialToSubstitute();

            yield return SlideIn(speed);

            substituteShown = true;
        }

        /// <summary>
        /// Hide a substitute and return the monster.
        /// </summary>
        [FoldoutGroup("Debug")]
        [Button]
        private void HideSubstituteTest(BattleManager battleManager) => StartCoroutine(HideSubstitute(battleManager));

        /// <summary>
        /// Hide the substitute and return the monster.
        /// </summary>
        public IEnumerator HideSubstitute(BattleManager battleManager)
        {
            if (!substituteShown) yield break;

            yield return SlideOut(battleManager.BattleSpeed);

            UpdateMaterial(battleManager);

            yield return SlideIn(battleManager.BattleSpeed);

            substituteShown = false;
        }

        /// <summary>
        /// Reset position when moves like dig of fly fail.
        /// </summary>
        public void ResetSpritePosition()
        {
            if (substituteShown) UpdateMaterialToSubstitute();

            MonsterSpriteRenderer.DOFade(1, .1f);
            ShadowSpriteRenderer.DOFade(1, .1f);

            Pivot.localPosition = new Vector3(0, -.5f, 0);
            MonsterSpriteRenderer.sortingOrder = previousSortingOrder;

            ShouldResetOnMoveFail = false;
            isCurrentlyHidden = false;
        }

        /// <summary>
        /// Dig down into the ground.
        /// </summary>
        /// <param name="speed">Speed to dig at.</param>
        public IEnumerator DigDown(float speed)
        {
            MonsterSpriteRenderer.DOFade(0, 1.5f / speed);
            ShadowSpriteRenderer.DOFade(0, 1.5f / speed);

            yield return Pivot.DOLocalJump(new Vector3(0, -.6f, 0), .1f, 1, .5f / speed).WaitForCompletion();
            yield return Pivot.DOLocalJump(new Vector3(0, -.7f, 0), .1f, 1, .5f / speed).WaitForCompletion();
            yield return Pivot.DOLocalJump(digPosition, .1f, 1, .5f / speed).WaitForCompletion();

            isCurrentlyHidden = true;
            ShouldResetOnMoveFail = true;
            Pivot.localPosition = basePosition;
        }

        /// <summary>
        /// Dig down off the ground.
        /// </summary>
        /// <param name="speed">Speed to dig at.</param>
        public IEnumerator DigUp(float speed)
        {
            Pivot.localPosition = digPosition;

            MonsterSpriteRenderer.DOFade(1, .5f / speed);
            ShadowSpriteRenderer.DOFade(1, .5f / speed);

            yield return Pivot.DOLocalJump(basePosition, .1f, 1, .5f / speed).WaitForCompletion();

            isCurrentlyHidden = false;
            ShouldResetOnMoveFail = false;
        }

        /// <summary>
        /// Fly up into the sky.
        /// </summary>
        /// <param name="speed">Speed to fly at.</param>
        public IEnumerator FlyUp(float speed)
        {
            MonsterSpriteRenderer.DOFade(0, 1.7f / speed);
            ShadowSpriteRenderer.DOFade(0, 1.7f / speed);

            yield return Pivot.DOLocalJump(flyPosition, 1, 1, .5f / speed)
                              .SetEase(Ease.InBack)
                              .WaitForCompletion();

            yield return new WaitForSeconds(1.2f / speed);

            isCurrentlyHidden = true;
            ShouldResetOnMoveFail = true;
            Pivot.localPosition = basePosition;
        }

        /// <summary>
        /// Fly up into the sky.
        /// </summary>
        /// <param name="speed">Speed to fly at.</param>
        /// <param name="targetPosition">Position to attack.</param>
        public IEnumerator FlyDownAndAttack(float speed, Vector3 targetPosition)
        {
            Pivot.localPosition = flyPosition;
            previousSortingOrder = MonsterSpriteRenderer.sortingOrder;
            MonsterSpriteRenderer.sortingOrder = 100;

            MonsterSpriteRenderer.DOFade(1, .3f / speed);
            ShadowSpriteRenderer.DOFade(1, .3f / speed);

            yield return Pivot.DOMove(targetPosition, .45f / speed)
                              .WaitForCompletion();

            isCurrentlyHidden = false;
            ShouldResetOnMoveFail = false;
        }

        /// <summary>
        /// Dive down into the water.
        /// </summary>
        /// <param name="speed">Speed to dive at.</param>
        /// <param name="particlesPrefab">Prefab for a particles effect.</param>
        public IEnumerator DiveDown(float speed, VisualEffect particlesPrefab)
        {
            VisualEffect particles = Instantiate(particlesPrefab,
                                                 Pivot.position,
                                                 Quaternion.identity);

            yield return WaitAFrame;

            MonsterSpriteRenderer.DOFade(0, 1.5f / speed);
            ShadowSpriteRenderer.DOFade(0, 1.5f / speed);

            Pivot.DOLocalJump(digPosition, .6f, 1, .7f / speed);

            yield return new WaitForSeconds(.3f / speed);

            particles.EnableAndPlay();

            yield return new WaitForSeconds(.4f / speed);

            particles.Stop();

            yield return new WaitForSeconds(.1f / speed);

            DOVirtual.DelayedCall(3, () => Destroy(particles.gameObject));

            isCurrentlyHidden = true;
            ShouldResetOnMoveFail = true;
            Pivot.localPosition = basePosition;
        }

        /// <summary>
        /// Dive up off the water.
        /// </summary>
        /// <param name="speed">Speed to dive at.</param>
        public IEnumerator DiveUp(float speed)
        {
            Pivot.localPosition = digPosition;

            MonsterSpriteRenderer.DOFade(1, .7f / speed);
            ShadowSpriteRenderer.DOFade(1, .7f / speed);

            yield return Pivot.DOLocalJump(new Vector3(0, -.5f, 0), .6f, 1, 1f / speed).WaitForCompletion();

            yield return new WaitForSeconds(.1f / speed);

            isCurrentlyHidden = false;
            ShouldResetOnMoveFail = false;
        }

        /// <summary>
        /// Fade away.
        /// </summary>
        /// <param name="speed">Battle speed.</param>
        /// <param name="duration">Animation duration.</param>
        public IEnumerator FadeAway(float speed, float duration = 1f)
        {
            MonsterSpriteRenderer.DOFade(0, duration / speed);
            yield return ShadowSpriteRenderer.DOFade(0, duration / speed).WaitForCompletion();

            isCurrentlyHidden = true;
            ShouldResetOnMoveFail = true;
        }

        /// <summary>
        /// Fade back in.
        /// </summary>
        /// <param name="speed">Battle speed.</param>
        /// <param name="duration">Animation duration.</param>
        public IEnumerator FadeIn(float speed, float duration = 1f)
        {
            MonsterSpriteRenderer.DOFade(1, duration / speed);
            yield return ShadowSpriteRenderer.DOFade(1, duration / speed).WaitForCompletion();

            isCurrentlyHidden = false;
            ShouldResetOnMoveFail = false;
        }
    }
}