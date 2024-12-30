using System.Collections.Generic;

namespace Varguiniano.YAPU.Runtime.Battle.Random
{
    /// <summary>
    /// Single player implementation of  battle random numbers provider that wraps the Unity Random class.
    /// </summary>
    public class SinglePlayerBattleRandomNumbersProvider : BattleManagerModule<SinglePlayerBattleRandomNumbersProvider>,
                                                           IBattleRandomNumbersProvider
    {
        /// <summary>
        /// Provide a random number between 0 and 1 (both inclusive).
        /// </summary>
        /// <returns>A float number.</returns>
        public float Value01() => UnityEngine.Random.value;

        /// <summary>
        /// Provide a random number between min and max (both inclusive).
        /// </summary>
        /// <returns>A float number.</returns>
        public float Range(float min, float max) => UnityEngine.Random.Range(min, max);

        /// <summary>
        /// Provide a random number between min (inclusive) and max (exclusive).
        /// </summary>
        /// <returns>An int number.</returns>
        public int Range(int min, int max) => UnityEngine.Random.Range(min, max);

        /// <summary>
        /// Get a random element of a list.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <typeparam name="T">Type of element in the list.</typeparam>
        /// <returns>A random element of that list.</returns>
        public T RandomElement<T>(List<T> original) => original[UnityEngine.Random.Range(0, original.Count)];
    }
}