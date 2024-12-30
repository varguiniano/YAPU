using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Weather
{
    /// <summary>
    /// Data class representing the foggy weather.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Weather/Foggy", fileName = "FoggyWeather")]
    public class FoggyWeather : Weather
    {
        /// <summary>
        /// Reference to the fog prefab.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private SpriteRenderer FogPrefab;

        /// <summary>
        /// Animation for when the weather starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherStartAnimation(BattleManager battleManager)
        {
            CoroutineRunner.RunRoutine(FogAnimation(battleManager.BattleSpeed));

            yield return DialogManager.ShowDialogAndWait(StartLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the weather ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherTick(BattleManager battleManager)
        {
            CoroutineRunner.RunRoutine(FogAnimation(battleManager.BattleSpeed));

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
        /// Play the fog animation.
        /// </summary>
        private IEnumerator FogAnimation(float speed)
        {
            SpriteRenderer fog = Instantiate(FogPrefab);

            yield return WaitAFrame;

            yield return fog.DOFade(1, .65f / speed).WaitForCompletion();

            yield return new WaitForSeconds(.2f / speed);

            yield return fog.DOFade(0, .65f / speed).WaitForCompletion();

            DOVirtual.DelayedCall(3, () => Destroy(fog.gameObject));
        }
    }
}