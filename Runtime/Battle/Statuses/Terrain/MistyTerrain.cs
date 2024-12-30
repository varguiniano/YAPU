using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Statuses;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain
{
    /// <summary>
    /// Data class for the MistyTerrain.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Terrain/Misty", fileName = "MistyTerrain")]
    public class MistyTerrain : Terrain
    {
        /// <summary>
        /// Statuses that are prevented by this terrain.
        /// </summary>
        [FoldoutGroup("Effect")]
        [SerializeField]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllVolatileStatuses))]
        #endif
        private List<VolatileStatus> PreventedStatuses;

        /// <summary>
        /// Reference to the move's sound.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Sound;

        /// <summary>
        /// Reference to the post processing that can change battle color tones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject TonesPostProcessing;

        /// <summary>
        /// Pink color to set.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector4 Pink;

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
            yield return PlayAnimation(battleManager);

            yield return DialogManager.ShowDialogAndWait(StartLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the terrain ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator TerrainTick(BattleManager battleManager)
        {
            yield return PlayAnimation(battleManager);

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
        /// Check if a status can be added to the monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public override IEnumerator CanAddStatus(Status status,
                                                 BattlerType targetType,
                                                 int targetIndex,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 Action<bool> callback)
        {
            yield return DialogManager.ShowDialogAndWait("Moves/MistyTerrain/PreventStatus",
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                         modifiers: status.LocalizableName);

            callback.Invoke(false);
        }

        /// <summary>
        /// Check if a status can be added to the monster.
        /// </summary>
        /// <param name="status">Status to add.</param>
        /// <param name="targetType">Type of the battler to add the status to.</param>
        /// <param name="targetIndex">Index of the battler to add the status to.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="userType">The type of the monster that triggered this change.</param>
        /// <param name="userIndex">The index of the monster that triggered this change.</param>
        /// <param name="callback">Callback telling if it can be added</param>
        public override IEnumerator CanAddStatus(VolatileStatus status,
                                                 BattlerType targetType,
                                                 int targetIndex,
                                                 BattleManager battleManager,
                                                 BattlerType userType,
                                                 int userIndex,
                                                 Action<bool> callback)
        {
            bool prevent = PreventedStatuses.Contains(status);

            if (prevent)
                yield return DialogManager.ShowDialogAndWait("Moves/MistyTerrain/PreventStatus",
                                                             switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed,
                                                             modifiers: status.LocalizableNameKey);

            callback.Invoke(!prevent);
        }

        /// <summary>
        /// Play the terrain animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator PlayAnimation(BattleManager battleManager)
        {
            Volume volume = Instantiate(TonesPostProcessing).GetComponent<Volume>();

            volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

            effect.shadows.overrideState = true;
            effect.midtones.overrideState = true;
            effect.highlights.overrideState = true;

            battleManager.Audio.PlayAudio(Sound, pitch: battleManager.BattleSpeed);

            Vector4 original = effect.midtones.GetValue<Vector4>();
            Vector4 tone = original;

            bool finished = false;

            DOTween.To(() => tone,
                       x => tone = x,
                       Pink,
                       .3f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(tone));
                                 effect.midtones.SetValue(new Vector4Parameter(tone));
                                 effect.highlights.SetValue(new Vector4Parameter(tone));
                             })
                   .OnComplete(() => finished = true);

            yield return new WaitUntil(() => finished);

            finished = false;

            DOTween.To(() => tone,
                       x => tone = x,
                       original,
                       1.4f / battleManager.BattleSpeed)
                   .OnUpdate(() =>
                             {
                                 effect.shadows.SetValue(new Vector4Parameter(tone));
                                 effect.midtones.SetValue(new Vector4Parameter(tone));
                                 effect.highlights.SetValue(new Vector4Parameter(tone));
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