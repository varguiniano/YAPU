﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain
{
    /// <summary>
    /// Data class for the ElectricTerrain.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Terrain/Psychic", fileName = "PsychicTerrain")]
    public class PsychicTerrain : Terrain
    {
        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Volume PostPrefab;

        /// <summary>
        /// Audio for the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Color values for post processing.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector4 ColorValues;

        /// <summary>
        /// Multi target moves that are affected by priority modification.
        /// Most multi target are not affected.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllMoves))]
        #endif
        private List<Move> PriorityMultiTargetAffectedMoves;

        /// <summary>
        /// Is a battler affected by this terrain?
        /// </summary>
        /// <param name="battler">Battler to check.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>True if it is affected.</returns>
        public override bool IsAffected(Battler battler, BattleManager battleManager)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (KeyValuePair<VolatileStatus, int> pair in battler.VolatileStatuses)
                if (!pair.Key.IsAffectedByTerrain(battler, this))
                    return false;

            return battler.IsGrounded(battleManager, false);
        }

        /// <summary>
        /// Animation for when the terrain starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator TerrainStartAnimation(BattleManager battleManager)
        {
            yield return TerrainAnimation(battleManager);

            yield return DialogManager.ShowDialogAndWait(StartLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the terrain ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator TerrainTick(BattleManager battleManager)
        {
            yield return TerrainAnimation(battleManager);

            yield return DialogManager.ShowDialogAndWait(TickLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the terrain ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator TerrainEndAnimation(BattleManager battleManager)
        {
            yield return DialogManager.ShowDialogAndWait(EndLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Called when the battler is about to be hit by a move.
        /// </summary>
        /// <param name="target">Battler.</param>
        /// <param name="move">The move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="user">User of the move.</param>
        /// <param name="didShowMoveMessageNormally"></param>
        /// <param name="callback">States true if it will still hit.</param>
        public override IEnumerator OnAboutToBeHitByMove(Battler target,
                                                         Move move,
                                                         BattleManager battleManager,
                                                         Battler user,
                                                         bool didShowMoveMessageNormally,
                                                         Action<bool> callback)
        {
            if (move.AffectedByProtect && move.GetPriority(user, new List<Battler> {target}, battleManager, false) > 0)
            {
                if (!didShowMoveMessageNormally)
                    DialogManager.ShowDialog("Battle/Move/Used",
                                             acceptInput: false,
                                             localizableModifiers: false,
                                             modifiers: new[]
                                                        {
                                                            user.GetNameOrNickName(battleManager.Localizer),
                                                            battleManager.Localizer[move.LocalizableName]
                                                        },
                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);

                yield return DialogManager.ShowDialogAndWait("Battle/Protected",
                                                             localizableModifiers: false,
                                                             modifiers: target
                                                                .GetNameOrNickName(battleManager.Localizer),
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                callback.Invoke(false);
            }
            else
                callback.Invoke(true);
        }

        /// <summary>
        /// Play the terrain animation.
        /// </summary>
        private IEnumerator TerrainAnimation(BattleManager battleManager)
        {
            Volume volume = Instantiate(PostPrefab, null);

            volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

            effect.shadows.overrideState = true;
            effect.midtones.overrideState = true;
            effect.highlights.overrideState = true;

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            Vector4 color = new(1, 1, 1, 0);

            bool finished = false;

            DOTween.To(() => color,
                       x => color = x,
                       ColorValues,
                       .25f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(color));
                                 effect.midtones.SetValue(new Vector4Parameter(color));
                                 effect.highlights.SetValue(new Vector4Parameter(color));
                             })
                   .OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            yield return new WaitForSeconds(1.5f / battleManager.BattleSpeed);

            finished = false;

            DOTween.To(() => color,
                       x => color = x,
                       new Vector4(1, 1, 1, 0),
                       .25f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(color));
                                 effect.midtones.SetValue(new Vector4Parameter(color));
                                 effect.highlights.SetValue(new Vector4Parameter(color));
                             })
                   .OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            DOVirtual.DelayedCall(3f, () => Destroy(volume.gameObject));
        }
    }
}