using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Abilities;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Moves
{
    /// <summary>
    /// Data class for the move SkillSwap.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Moves/Psychic/SkillSwap", fileName = "SkillSwap")]
    public class SkillSwap : Move
    {
        /// <summary>
        /// Execute the effect of the move.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <param name="userType">Type of user of the move.</param>
        /// <param name="userIndex">Index of the user of the move.</param>
        /// <param name="user"></param>
        /// <param name="targets">Targets of the move.</param>
        /// <param name="hitNumber">This is the number of hits already made in this move. It will be always 0 unless it's a multihit move.</param>
        /// <param name="expectedHits">Expected move hits.</param>
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
            foreach ((BattlerType targetType, int targetIndex) in targets)
            {
                Battler target = battleManager.Battlers.GetBattlerFromBattleIndex(targetType, targetIndex);

                Ability temp = user.GetAbility();
                user.SetAbility(target.GetAbility());
                target.SetAbility(temp);

                // Retrigger ability etbs.
                if (user.CanUseAbility(battleManager, false))
                    yield return user.GetAbility().OnMonsterEnteredBattle(battleManager, user);

                if (target.CanUseAbility(battleManager, ignoresAbilities))
                    yield return target.GetAbility().OnMonsterEnteredBattle(battleManager, target);
            }

            finishedCallback.Invoke(true);
        }

        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Particles to display on the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Texture2D Particle;

        /// <summary>
        /// Reference to the post processing that can change battle color tones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject TonesPostProcessing;

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
            for (int i = 0; i < targets.Count; i++)
            {
                (BattlerType targetType, int targetIndex) = targets[i];

                Volume volume = Instantiate(TonesPostProcessing).GetComponent<Volume>();

                volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

                effect.shadows.overrideState = true;
                effect.midtones.overrideState = true;
                effect.highlights.overrideState = true;

                battleManager.Audio.PlayAudio(Sound, pitch: speed);

                Vector4 original = effect.midtones.GetValue<Vector4>();
                Vector4 tone = original;

                Vector4 purple = new(.91f,
                                     .33f,
                                     1f,
                                     0f);

                bool finished = false;

                DOTween.To(() => tone,
                           x => tone = x,
                           purple,
                           .1f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     //effect.shadows.SetValue(new Vector4Parameter(tone));
                                     effect.midtones.SetValue(new Vector4Parameter(tone));
                                     //effect.highlights.SetValue(new Vector4Parameter(tone));
                                 })
                       .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                finished = false;

                CoroutineRunner.RunRoutine(battleManager.GetMonsterSprite(userType, userIndex)
                                                        .FXAnimator.PlayAbsorb(.9f / battleManager.BattleSpeed,
                                                                               userPosition.position,
                                                                               particleTexture: Particle));

                yield return battleManager.GetMonsterSprite(targetType, targetIndex)
                                          .FXAnimator.PlayAbsorb(.9f / battleManager.BattleSpeed,
                                                                 targetPositions[i].position,
                                                                 particleTexture: Particle);

                yield return new WaitForSeconds(.2f);

                DOTween.To(() => tone,
                           x => tone = x,
                           original,
                           .1f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     //effect.shadows.SetValue(new Vector4Parameter(tone));
                                     effect.midtones.SetValue(new Vector4Parameter(tone));
                                     //effect.highlights.SetValue(new Vector4Parameter(tone));
                                 })
                       .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                effect.shadows.overrideState = false;
                effect.midtones.overrideState = false;
                effect.highlights.overrideState = false;

                DOVirtual.DelayedCall(3, () => Destroy(volume.gameObject));
            }
        }
    }
}