using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Varguiniano.YAPU.Runtime.Characters;

namespace Varguiniano.YAPU.Runtime.World.OutOfBattleWeathers
{
    /// <summary>
    /// Data class for sunny weather outside battles.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/Weathers/Sunny", fileName = "Sunny")]
    public class Sunny : OutOfBattleWeather
    {
        /// <summary>
        /// Color values for post processing.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private Vector4 ColorValues;

        /// <summary>
        /// Reference to the weather post pro effect.
        /// </summary>
        private ShadowsMidtonesHighlights weatherEffect;

        /// <summary>
        /// Start this weather.
        /// </summary>
        public override IEnumerator StartWeather(PlayerCharacter playerCharacter)
        {
            playerCharacter.PostProcessing.WeatherVolume.sharedProfile.TryGet(out weatherEffect);

            weatherEffect.shadows.overrideState = true;
            weatherEffect.midtones.overrideState = true;
            weatherEffect.highlights.overrideState = true;

            Vector4 color = weatherEffect.shadows.GetValue<Vector4>();

            yield return DOTween.To(() => color,
                                    x => color = x,
                                    ColorValues,
                                    .5f)
                                .OnUpdate(() =>
                                          {
                                              weatherEffect.shadows.SetValue(new Vector4Parameter(color));
                                              weatherEffect.midtones.SetValue(new Vector4Parameter(color));
                                              weatherEffect.highlights.SetValue(new Vector4Parameter(color));
                                          })
                                .WaitForCompletion();
        }

        /// <summary>
        /// End this weather.
        /// </summary>
        /// <param name="playerCharacter">Reference to the player's character.</param>
        /// <param name="isDestroyingCharacter">Is the character being destroyed?</param>
        public override IEnumerator EndWeather(PlayerCharacter playerCharacter, bool isDestroyingCharacter)
        {
            Vector4 colorTarget = new(1, 1, 1, 0);

            if (isDestroyingCharacter)
            {
                weatherEffect.shadows.SetValue(new Vector4Parameter(colorTarget));
                weatherEffect.midtones.SetValue(new Vector4Parameter(colorTarget));
                weatherEffect.highlights.SetValue(new Vector4Parameter(colorTarget));
                weatherEffect.shadows.overrideState = false;
                weatherEffect.midtones.overrideState = false;
                weatherEffect.highlights.overrideState = false;
            }
            else
            {
                Vector4 color = weatherEffect.shadows.GetValue<Vector4>();

                yield return DOTween.To(() => color,
                                        x => color = x,
                                        colorTarget,
                                        .25f)
                                    .OnUpdate(() =>
                                              {
                                                  weatherEffect.shadows.SetValue(new Vector4Parameter(color));
                                                  weatherEffect.midtones.SetValue(new Vector4Parameter(color));
                                                  weatherEffect.highlights.SetValue(new Vector4Parameter(color));
                                              })
                                    .OnComplete(() =>
                                                {
                                                    weatherEffect.shadows.overrideState = false;
                                                    weatherEffect.midtones.overrideState = false;
                                                    weatherEffect.highlights.overrideState = false;
                                                })
                                    .WaitForCompletion();
            }
        }
    }
}