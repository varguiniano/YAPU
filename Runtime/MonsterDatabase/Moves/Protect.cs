using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move Protect.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Normal/Protect", fileName = "Protect")]
    public class Protect : SetVolatileStatusMove
    {
        /// <summary>
        /// Prefab for the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SpriteRenderer AnimationPrefab;

        /// <summary>
        /// Prefab for the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        [ColorUsage(true, true)]
        private Color BarrierColor;

        /// <summary>
        /// Were to place the barrier when its on the enemy side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 EnemyPosition;

        /// <summary>
        /// Were to place the barrier when its on the enemy side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private int EnemyPriority;

        /// <summary>
        /// Were to place the barrier when its on the allied side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector3 AllyPosition;

        /// <summary>
        /// Were to place the barrier when its on the allied side.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private int AllyPriority;

        /// <summary>
        /// Animation audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        protected AudioReference Audio;

        /// <summary>
        /// Dictionary that keeps track of the battlers that have performed this move.
        /// The key is the battler, the value is the times it has performed it in a row, the last turn it used it and if it will fail next turn.
        /// </summary>
        private readonly Dictionary<Battler, (int, int, bool)> battlersLastUsed = new();

        /// <summary>
        /// Check if the move will fail reasons other than accuracy.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="customFailMessage"></param>
        public override bool WillMoveFail(BattleManager battleManager,
                                          ILocalizer localizer,
                                          BattlerType userType,
                                          int userIndex,
                                          ref List<(BattlerType Type, int Index)> targets,
                                          int hitNumber,
                                          int expectedHits,
                                          bool ignoresAbilities,
                                          out string customFailMessage)
        {
            Battler battler = battleManager.Battlers.GetBattlerFromBattleIndex(userType, userIndex);

            // ReSharper disable once InvertIf
            if (battlersLastUsed.ContainsKey(battler))
            {
                (int _, int lastTurn, bool fail) = battlersLastUsed[battler];

                if (lastTurn < battleManager.TurnCounter - 1)
                {
                    battlersLastUsed.Remove(battler);
                    fail = false;
                }

                if (!fail)
                    return base.WillMoveFail(battleManager,
                                             localizer,
                                             userType,
                                             userIndex,
                                             ref targets,
                                             hitNumber,
                                             expectedHits,
                                             ignoresAbilities,
                                             out customFailMessage);

                customFailMessage = "";
                return true;
            }

            return base.WillMoveFail(battleManager,
                                     localizer,
                                     userType,
                                     userIndex,
                                     ref targets,
                                     hitNumber,
                                     expectedHits,
                                     ignoresAbilities,
                                     out customFailMessage);
        }

        /// <summary>
        /// Play the move animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="speed">Speed at which the animation should be done.</param>
        /// <param name="userType">Type of battler the user is.</param>
        /// <param name="userIndex">User index.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="userPosition">Position of the user.</param>
        /// <param name="targets">Targets types and indexes.</param>
        /// <param name="targetPositions">Position of the targets.</param>
        /// <param name="ignoresAbilities"></param>
        public override IEnumerator PlayAnimation(BattleManager battleManager,
                                                  float speed,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  Transform userPosition,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  List<Transform> targetPositions,
                                                  bool ignoresAbilities)
        {
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                BattleMonsterSprite sprite = battleManager.GetMonsterSprite(targetType, targetIndex);

                SpriteRenderer animation = Instantiate(AnimationPrefab, sprite.Pivot);

                animation.transform.localPosition = targetType == BattlerType.Ally ? AllyPosition : EnemyPosition;

                animation.sortingOrder = targetType == BattlerType.Ally ? AllyPriority : EnemyPriority;

                animation.color = BarrierColor;

                AudioManager.Instance.PlayAudio(Audio, pitch: speed);

                bool finished = false;

                // 0.7843137255 = 200/255
                animation.DOFade(0.7843137255f, .75f / speed).OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                finished = false;

                animation.DOFade(0, .75f / speed).OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                DOVirtual.DelayedCall(2f, () => Destroy(animation.gameObject));
            }
        }

        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber"></param>
        /// <param name="expectedHits"></param>
        /// <param name="externalPowerMultiplier"></param>
        /// <param name="ignoresAbilities"></param>
        /// <param name="finishedCallback">Callback stating if the move successfully executed.</param>
        public override IEnumerator ExecuteEffect(BattleManager battleManager,
                                                  ILocalizer localizer,
                                                  BattlerType userType,
                                                  int userIndex,
                                                  Battler user,
                                                  List<(BattlerType Type, int Index)> targets,
                                                  int hitNumber,
                                                  int expectedHits,
                                                  float externalPowerMultiplier,
                                                  bool ignoresAbilities,
                                                  Action<bool> finishedCallback)
        {
            int timesUsed = 0;
            bool fail = false;

            if (battlersLastUsed.TryGetValue(user, out (int, int, bool) value))
            {
                int lastTurn;
                (timesUsed, lastTurn, fail) = value;

                if (lastTurn != battleManager.TurnCounter - 1) fail = false;
            }

            if (fail)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/Failed",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                battlersLastUsed.Remove(user);
            }
            else
            {
                yield return base.ExecuteEffect(battleManager,
                                                localizer,
                                                userType,
                                                userIndex,
                                                user,
                                                targets,
                                                hitNumber,
                                                expectedHits,
                                                externalPowerMultiplier,
                                                ignoresAbilities,
                                                finishedCallback);

                timesUsed++;

                float roll = battleManager.RandomProvider.Value01();
                float chance = 1 / (3f * timesUsed);

                Logger.Info("Chance to use " + name + " next time: " + chance + ". Rolled: " + roll + ".");

                battlersLastUsed[user] = (timesUsed, battleManager.TurnCounter, roll > chance);
            }

            finishedCallback.Invoke(!fail);
        }

        /// <summary>
        /// Called when the battle ends to clean up the turn used data.
        /// </summary>
        /// <param name="battler">Battler owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerable OnBattleEnded(Battler battler, BattleManager battleManager)
        {
            battlersLastUsed.Clear();

            return base.OnBattleEnded(battler, battleManager);
        }
    }
}