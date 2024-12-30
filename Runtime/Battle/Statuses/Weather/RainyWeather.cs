using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Weather
{
    /// <summary>
    /// Data class representing the rainy weather.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Weather/Rainy", fileName = "RainyWeather")]
    public class RainyWeather : Weather
    {
        /// <summary>
        /// Reference to the animation prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Volume PostPrefab;

        /// <summary>
        /// Reference to the rain FX prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private VisualEffect RainFX;

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
        /// Animation for when the weather starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherStartAnimation(BattleManager battleManager)
        {
            yield return RainAnimation(battleManager);

            yield return DialogManager.ShowDialogAndWait(StartLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the weather ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherTick(BattleManager battleManager)
        {
            yield return RainAnimation(battleManager);

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
        /// Play the rain animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private IEnumerator RainAnimation(BattleManager battleManager)
        {
            Volume volume = Instantiate(PostPrefab, null);

            volume.sharedProfile.TryGet(out ShadowsMidtonesHighlights effect);

            effect.shadows.overrideState = true;
            effect.midtones.overrideState = true;
            effect.highlights.overrideState = true;

            AudioManager.Instance.PlayAudio(Audio, pitch: battleManager.BattleSpeed);

            VisualEffect rainFXInstance = Instantiate(RainFX, null);

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

            yield return new WaitForSeconds(1.6f / battleManager.BattleSpeed);

            rainFXInstance.Stop();

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
            
            effect.shadows.overrideState = false;
            effect.midtones.overrideState = false;
            effect.highlights.overrideState = false;

            DOVirtual.DelayedCall(2f,
                                  () =>
                                  {
                                      Destroy(rainFXInstance.gameObject);
                                      Destroy(volume.gameObject);
                                  });
        }
    }
}