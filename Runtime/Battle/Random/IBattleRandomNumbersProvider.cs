using System.Collections.Generic;

namespace Varguiniano.YAPU.Runtime.Battle.Random
{
    /// <summary>
    /// Interface defining a battle manager module that provides random numbers.
    /// </summary>
    public interface IBattleRandomNumbersProvider
    {
        /// <summary>
        /// Provide a random number between 0 and 1 (both inclusive).
        /// </summary>
        /// <returns>A float number.</returns>
        float Value01();

        /// <summary>
        /// Provide a random number between min and max (both inclusive).
        /// </summary>
        /// <returns>A float number.</returns>
        float Range(float min, float max);

        /// <summary>
        /// Provide a random number between min (inclusive) and max (exclusive).
        /// </summary>
        /// <returns>An int number.</returns>
        int Range(int min, int max);
        
        /// <summary>
        /// Get a random element of a list.
        /// </summary>
        /// <param name="original">Original list.</param>
        /// <typeparam name="T">Type of element in the list.</typeparam>
        /// <returns>A random element of that list.</returns>
        public T RandomElement<T>(List<T> original);
    }
}