using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Weather
{
    /// <summary>
    /// Data class representing the sunny weather.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Weather/Sunny", fileName = "SunnyWeather")]
    public class SunnyWeather : Weather
    {
        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private GameObject AnimationPrefab;

        /// <summary>
        /// Reference to the sunny audio.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference Audio;

        /// <summary>
        /// Normal value for midtones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float NormalTone;

        /// <summary>
        /// Highest value for the midtones.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float HightestTone;

        /// <summary>
        /// Animation for when the weather starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherStartAnimation(BattleManager battleManager)
        {
            yield return SunnyAnimation(battleManager);

            yield return DialogManager.ShowDialogAndWait(StartLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the weather ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherTick(BattleManager battleManager)
        {
            yield return SunnyAnimation(battleManager);

            yield return DialogManager.ShowDialogAndWait(TickLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the weather ends.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherEndAnimation(BattleManager battleManager)
        {
            yield return DialogManager.ShowDialogAndWait(EndLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Play the sunny animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator SunnyAnimation(BattleManager battleManager)
        {
            Volume volume = Instantiate(AnimationPrefab).GetComponent<Volume>();

            volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

            effect.shadows.overrideState = true;
            effect.midtones.overrideState = true;
            effect.highlights.overrideState = true;

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            for (int i = 0; i < 2; i++)
            {
                float tone = NormalTone;

                bool finished = false;

                DOTween.To(() => tone,
                           x => tone = x,
                           HightestTone,
                           .5f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     effect.shadows.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.midtones.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.highlights.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 })
                       .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);

                finished = false;

                DOTween.To(() => tone,
                           x => tone = x,
                           NormalTone,
                           .5f / battleManager.BattleSpeed)
                       .OnUpdate(() =>
                                 {
                                     effect.shadows.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.midtones.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                     effect.highlights.SetValue(new Vector4Parameter(new Vector4(1, 1, 1, tone)));
                                 })
                       .OnComplete(() => finished = true);

                yield return new WaitUntil(() => finished);
            }

            effect.shadows.overrideState = false;
            effect.midtones.overrideState = false;
            effect.highlights.overrideState = false;

            Destroy(volume.gameObject);
        }
    }
}