using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Moves;
using Varguiniano.YAPU.Runtime.UI.Dialogs;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Weather
{
    /// <summary>
    /// Data class representing the strong winds weather.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Monster Database/Status/Weather/StrongWindsWeather",
                     fileName = "StrongWindsWeather")]
    public class StrongWindsWeather : Weather
    {
        /// <summary>
        /// Type protected by this weather.
        /// </summary>
        [SerializeField]
        private MonsterType ProtectedType;

        /// <summary>
        /// Calculate the multiplier for a move's power.
        /// </summary>
        /// <param name="move">Move to check.</param>
        /// <param name="owner">Owner of the move.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <returns>The multiplier for that move.</returns>
        public override float CalculateMovesDamageMultiplier(Move move, Battler owner, BattleManager battleManager)
        {
            float multiplier = base.CalculateMovesDamageMultiplier(move, owner, battleManager);

            if (move.GetMoveTypeInBattle(owner, battleManager).AttackMultipliers[ProtectedType] > 1)
                multiplier *= 0.75f; // Revert to neutral damage.

            return multiplier;
        }

        /// <summary>
        /// Animation for when the weather starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherStartAnimation(BattleManager battleManager)
        {
            yield return WindAnimation(battleManager);

            yield return DialogManager.ShowDialogAndWait(StartLocalizationKey,
                                                         switchToNextAfterSeconds: 1.5f / battleManager.BattleSpeed);
        }

        /// <summary>
        /// Animation for when the weather ticks each turn.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        public override IEnumerator WeatherTick(BattleManager battleManager)
        {
            yield return WindAnimation(battleManager);

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
        /// Play the wind animation.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        private static IEnumerator WindAnimation(BattleManager battleManager)
        {
            foreach (Battler battler in battleManager.Battlers.GetBattlersFighting())
                battleManager.GetMonsterSprite(battler).Pivot.DOShakePosition(1f / battleManager.BattleSpeed, .05f);

            yield break;
        }
    }
}