using System;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Class defining a single game variable.
    /// </summary>
    [Serializable]
    public class GameVariable<T>
    {
        /// <summary>
        /// Name of the variable.
        /// </summary>
        public string Name;
        
        /// <summary>
        /// Value when a new game is started.
        /// </summary>
        public T DefaultValue;

        /// <summary>
        /// Value of the variable.
        /// </summary>
        public T Value;
    }
}