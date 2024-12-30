using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Balls;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Capture module for the battle manager.
    /// </summary>
    public class BattleManagerCaptureModule : BattleManagerModule<BattleManagerCaptureModule>
    {
        /// <summary>
        /// Reference to the ball prefab.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private BattleBallSprite BallPrefab;

        /// <summary>
        /// Duration of the ball drop animation.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private float BallDropDuration;

        /// <summary>
        /// Path for the balls to follow when thrown.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private List<Transform> BallPath;

        /// <summary>
        /// Steps for the open flipbook animation.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private float OpenFlipbookSteps;

        /// <summary>
        /// Steps in which the ball is open.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private int OpenStep = 10;

        /// <summary>
        /// Steps to perform to tip the ball to the left.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private int[] TipLeftSequence = {0, 11, 12, 13, 12, 11, 0};

        /// <summary>
        /// Steps to perform to tip the ball to the left.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private int[] TipRightSequence = {0, 14, 15, 16, 15, 14, 0};

        /// <summary>
        /// Audio for the throw ball animation.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private AudioReference ThrowBallAudio;

        /// <summary>
        /// Audio for the close ball animation.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private AudioReference DropBallAudio;

        /// <summary>
        /// Audio for the shake ball animation.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private AudioReference ShakeBallAudio;

        /// <summary>
        /// Audio for the open ball animation.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private AudioReference OpenBallAudio;

        /// <summary>
        /// Audio for the captured music.
        /// </summary>
        [FoldoutGroup("Capture")]
        [SerializeField]
        private AudioReference CapturedMusic;

        /// <summary>
        /// Store the captured monster to send it back at the end of the battle.
        /// </summary>
        [FoldoutGroup("Capture")]
        [ShowInInspector]
        [ReadOnly]
        internal Battler CapturedMonster;

        /// <summary>
        /// Attempt to capture a monster.
        /// </summary>
        /// <param name="index">Index of the battler to capture.</param>
        /// <param name="ball">Ball to use.</param>
        [FoldoutGroup("Debug")]
        [Button("Test capture")]
        private void TestTryCaptureMonster(int index, Ball ball) => StartCoroutine(TryCaptureMonster(index, ball));

        /// <summary>
        /// Attempt to capture a monster.
        /// </summary>
        /// <param name="index">Index of the battler to capture.</param>
        /// <param name="ball">Ball to use.</param>
        public IEnumerator TryCaptureMonster(int index, Ball ball)
        {
            if (BattleManager.EnemyType == EnemyType.Trainer)
            {
                Logger.Error("Can't capture against trainer.");
                yield break;
            }

            if (Battlers.GetBattlersFighting(BattlerType.Enemy).Count > 1)
            {
                Logger.Error("There can only be one enemy when trying to capture.");
                yield break;
            }

            if (!Battlers.IsBattlerFighting(BattlerType.Enemy, index))
            {
                Logger.Error("Enemy has to be fighting to be able to capture it.");
                yield break;
            }

            BattleManager.GlobalGameData.LastUsedBall = ball;

            Battler battler = Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, index);

            if (!battler.CanBeCaught(BattleManager))
            {
                yield return DialogManager.ShowDialogAndWait("Battle/EvadedBall",
                                                             localizableModifiers: false,
                                                             modifiers: new[]
                                                                        {
                                                                            battler.GetNameOrNickName(BattleManager
                                                                               .Localizer),
                                                                            ball.GetName(BattleManager.Localizer)
                                                                        },
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);

                yield break;
            }

            BattleMonsterSprite sprite = BattleManager.GetMonsterSprite(BattlerType.Enemy, index);

            Vector3 dropPosition = sprite.Pivot.position;

            List<Vector3> path = BallPath.Select(x => x.position).ToList();

            Vector3 lastPathPosition = dropPosition;

            lastPathPosition.y += .6f;

            path.Add(lastPathPosition);

            Vector3[] ballPathArray = path.ToArray();

            BattleManager.AudioManager.PlayAudio(ThrowBallAudio, pitch: BattleManager.BattleSpeed);

            yield return new WaitForSeconds(.2f / BattleManager.BattleSpeed);

            BattleBallSprite ballSprite = Instantiate(BallPrefab, BallPath[0].position, Quaternion.identity);

            yield return WaitAFrame;

            bool animFinished = false;

            TweenerCore<Vector3, Path, PathOptions> tween = ballSprite.GetCachedComponent<Transform>()
                                                                      .DOPath(ballPathArray,
                                                                              BallDropDuration
                                                                            / BattleManager.BattleSpeed,
                                                                              PathType.CatmullRom,
                                                                              PathMode.Ignore)
                                                                      .SetOptions(false,
                                                                           lockRotation: AxisConstraint.None)
                                                                      .SetEase(Ease.Linear);

            tween.OnUpdate(() => ballSprite.UpdateFlipbook(ball,
                                                           (int) (OpenFlipbookSteps
                                                                * tween.Elapsed()
                                                                / BallDropDuration
                                                                / BattleManager.BattleSpeed)));

            yield return tween.WaitForCompletion();

            sprite.ShrinkWithVFXTowardsTarget(BattleManager.BattleSpeed, path.Last(), () => animFinished = true);

            yield return new WaitUntil(() => animFinished);

            ballSprite.UpdateFlipbook(ball, 0);

            yield return new WaitForSeconds(.1f / BattleManager.BattleSpeed);

            BattleManager.AudioManager.PlayAudio(DropBallAudio, pitch: BattleManager.BattleSpeed);

            yield return ballSprite.transform.DOMove(dropPosition, .4f / BattleManager.BattleSpeed)
                                   .SetEase(Ease.OutBounce)
                                   .WaitForCompletion();

            sprite.Pivot.transform.position = dropPosition;

            yield return new WaitForSeconds(1f / BattleManager.BattleSpeed);

            float probability = 0;

            yield return BattleUtils.CalculateShakeProbability(BattleManager,
                                                               battler,
                                                               Battlers.GetBattlersFighting(BattlerType.Ally)
                                                                       .First(),
                                                               ball,
                                                               prob => probability = prob);

            bool captured = true;

            string breakBallMessage = "Battle/Capture/Escape/0";

            for (int i = 0; i < 4; ++i) // Four catch checks.
            {
                float chance = RandomProvider.Value01();

                if (chance < probability)
                {
                    Logger.Info("Shake " + i + " with chance " + chance + " succeeded.");

                    BattleManager.AudioManager.PlayAudio(ShakeBallAudio, pitch: BattleManager.BattleSpeed);

                    foreach (int step in i % 2 == 0 ? TipLeftSequence : TipRightSequence)
                    {
                        ballSprite.UpdateFlipbook(ball, step);

                        yield return new WaitForSeconds(.02f / BattleManager.BattleSpeed);
                    }
                }
                else
                {
                    Logger.Info("Shake " + i + " with chance " + chance + " failed.");

                    captured = false;

                    breakBallMessage = "Battle/Capture/Escape/" + i;

                    yield return new WaitForSeconds(1f / BattleManager.BattleSpeed);

                    break;
                }

                yield return new WaitForSeconds(1f / BattleManager.BattleSpeed);
            }

            if (captured)
            {
                CapturedMonster = battler;

                ball.AfterCapture(ref CapturedMonster);

                BattleManager.AudioManager.StopAllAudios();

                ballSprite.PlayCatchAnimation();

                BattleManager.AudioManager.PlayAudio(CapturedMusic);

                yield return new WaitForSeconds(2f);

                ballSprite.FadeOut();

                yield return new WaitForSeconds(2f);

                BattleManager.IsBattleOver = true;

                yield return DialogManager.WaitForDialog;

                Destroy(ballSprite.gameObject);
            }
            else
            {
                ballSprite.UpdateFlipbook(ball, OpenStep);

                BattleManager.AudioManager.PlayAudio(OpenBallAudio, pitch: BattleManager.BattleSpeed);

                animFinished = false;

                sprite.EnlargeWithVFX(BattleManager.BattleSpeed, () => animFinished = true);

                yield return new WaitUntil(() => animFinished);

                Destroy(ballSprite.gameObject);

                yield return DialogManager.ShowDialogAndWait(breakBallMessage,
                                                             localizableModifiers: false,
                                                             modifiers: battler.GetNameOrNickName(BattleManager
                                                                .Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / BattleManager.BattleSpeed);
            }
        }
    }
}