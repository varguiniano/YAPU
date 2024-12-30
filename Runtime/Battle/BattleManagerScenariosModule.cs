using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Scenarios;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.World;
using Varguiniano.YAPU.Runtime.World.Encounters;
using Terrain = Varguiniano.YAPU.Runtime.Battle.Statuses.Terrain.Terrain;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Scenarios module for the battle manager.
    /// </summary>
    public class BattleManagerScenariosModule : BattleManagerModule<BattleManagerScenariosModule>
    {
        /// <summary>
        /// Scenario for this battle.
        /// </summary>
        [FoldoutGroup("Scenario")]
        [ShowInInspector]
        [ReadOnly]
        public BattleScenario BattleScenario { get; private set; }

        /// <summary>
        /// Encounter type for this battle.
        /// </summary>
        [FoldoutGroup("Scenario")]
        [ShowInInspector]
        [ReadOnly]
        public EncounterType EncounterType { get; private set; }

        /// <summary>
        /// Scene this battle is happening in.
        /// </summary>
        [FoldoutGroup("Scenario")]
        [ShowInInspector]
        [ReadOnly]
        public SceneInfoAsset CurrentScene { get; private set; }

        /// <summary>
        /// Current weather in battle.
        /// </summary>
        [FoldoutGroup("Weather")]
        [ShowInInspector]
        [ReadOnly]
        private Weather weather;

        /// <summary>
        /// Turns left for the weather.
        /// -1 = infinite.
        /// </summary>
        [FoldoutGroup("Weather")]
        [ShowInInspector]
        [ReadOnly]
        private int weatherCountdown;

        /// <summary>
        /// Current Terrain in battle.
        /// </summary>
        [FoldoutGroup("Terrain")]
        [ShowInInspector]
        [ReadOnly]
        public Terrain Terrain { get; private set; }

        /// <summary>
        /// Turns left for the terrain.
        /// -1 = infinite.
        /// </summary>
        [FoldoutGroup("Terrain")]
        [ShowInInspector]
        [ReadOnly]
        private int terrainCountdown;

        /// <summary>
        /// Initialize the scenario.
        /// </summary>
        /// <param name="scenario">Scenario to use.</param>
        /// <param name="encounterType">Encounter type to use.</param>
        /// <param name="scene">Scene this battle is happening in.</param>
        internal void SetScenario(BattleScenario scenario, EncounterType encounterType, SceneInfoAsset scene)
        {
            BattleScenario = scenario;
            CurrentScene = scene;
            EncounterType = encounterType;

            Instantiate(BattleScenario.BackgroundPrefab, BattleManager.transform);
        }

        /// <summary>
        /// Test method to set a new weather condition.
        /// </summary>
        /// <param name="newWeather">The new weather to set.</param>
        /// <param name="countdown">The countdown it will have.</param>
        [FoldoutGroup("Weather")]
        [Button]
        private void SetWeatherTest(Weather newWeather, int countdown) =>
            StartCoroutine(SetWeather(newWeather, countdown));

        /// <summary>
        /// Set a new weather condition.
        /// </summary>
        /// <param name="newWeather">The new weather to set.</param>
        /// <param name="countdown">The countdown it will have.</param>
        public IEnumerator SetWeather(Weather newWeather, int countdown)
        {
            if (weather != newWeather)
            {
                if (weather != null) yield return weather.WeatherEndAnimation(BattleManager);

                weather = newWeather;
            }

            if (newWeather == null) yield break;

            yield return weather.WeatherStartAnimation(BattleManager);

            weatherCountdown = countdown;
        }

        /// <summary>
        /// Clear the current weather.
        /// </summary>
        public IEnumerator ClearWeather()
        {
            if (weather == null) yield break;

            yield return weather.WeatherEndAnimation(BattleManager);
            weather = null;
        }

        /// <summary>
        /// Test method to set a new Terrain condition.
        /// </summary>
        /// <param name="newTerrain">The new Terrain to set.</param>
        /// <param name="countdown">The countdown it will have.</param>
        [FoldoutGroup("Terrain")]
        [Button]
        private void SetTerrainTest(Terrain newTerrain, int countdown) =>
            StartCoroutine(SetTerrain(newTerrain, countdown));

        /// <summary>
        /// Set a new Terrain condition.
        /// </summary>
        /// <param name="newTerrain">The new Terrain to set.</param>
        /// <param name="countdown">The countdown it will have.</param>
        public IEnumerator SetTerrain(Terrain newTerrain, int countdown)
        {
            if (Terrain != newTerrain)
            {
                if (Terrain != null) yield return Terrain.TerrainEndAnimation(BattleManager);

                Terrain = newTerrain;
            }

            yield return Terrain.TerrainStartAnimation(BattleManager);

            terrainCountdown = countdown;

            foreach (Battler battler in Battlers.GetBattlersFighting())
                yield return battler.OnTerrainSet(BattleManager, Terrain);
        }

        /// <summary>
        /// Get the current weather.
        /// </summary>
        /// <param name="currentWeather">Current weather in the battle.</param>
        /// <returns>True if the weather has any effect.</returns>
        public bool GetWeather(out Weather currentWeather)
        {
            currentWeather = weather;
            return DoesWeatherHaveEffect();
        }

        /// <summary>
        /// Clear the current terrain.
        /// </summary>
        public IEnumerator ClearTerrain()
        {
            if (Terrain == null) yield break;

            yield return Terrain.TerrainEndAnimation(BattleManager);
            Terrain = null;
        }

        /// <summary>
        /// Does the weather have effect?
        /// It could be negated by, for example, Cloud Nine.
        /// </summary>
        private bool DoesWeatherHaveEffect()
        {
            bool hasEffect = weather != null;

            return Battlers.GetBattlersFighting()
                           .Aggregate(hasEffect,
                                      (current, battler) => current && battler.DoesWeatherHaveEffect(BattleManager));
        }

        /// <summary>
        /// Trigger the countdown of the weather.
        /// </summary>
        internal IEnumerator TriggerWeatherCountdown()
        {
            if (weather == null) yield break;

            if (weatherCountdown != -1) weatherCountdown--;

            if (weatherCountdown == 0)
                yield return ClearWeather();
            else
            {
                if (DoesWeatherHaveEffect()) yield return weather.WeatherTick(BattleManager);
            }

            yield return BattlerHealth.CheckFaintedBattlers();
        }

        /// <summary>
        /// Trigger the countdown of the Terrain.
        /// </summary>
        internal IEnumerator TriggerTerrainCountdown()
        {
            if (Terrain == null) yield break;

            if (terrainCountdown != -1) terrainCountdown--;

            if (terrainCountdown == 0)
                yield return ClearTerrain();
            else
                yield return Terrain.TerrainTick(BattleManager);

            yield return BattlerHealth.CheckFaintedBattlers();
        }
    }
}