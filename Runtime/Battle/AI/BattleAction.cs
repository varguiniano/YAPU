namespace Varguiniano.YAPU.Runtime.Battle.AI
{
    /// <summary>
    /// Data structure that represents an action that can be taken in battle.
    /// </summary>
    public struct BattleAction
    {
        /// <summary>
        /// Type of battler for this action.
        /// </summary>
        public BattlerType BattlerType;

        /// <summary>
        /// In battle index.
        /// </summary>
        public int Index;

        /// <summary>
        /// Type of the action to take.
        /// </summary>
        public Type ActionType;

        /// <summary>
        /// Have the monster change to its mega form.
        /// </summary>
        public bool TriggerMegaForm;

        /// <summary>
        /// Parameters for action.
        /// Ex. Target monster for a move, index of an item and target monster...
        /// </summary>
        public int[] Parameters;
        
        /// <summary>
        /// Parameters used in rare cases.
        /// Ex. Using a move that it's not in the current moves.
        /// </summary>
        public object[] AdditionalParameters;

        /// <summary>
        /// Type of action.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Use a move.
            /// </summary>
            Move,

            /// <summary>
            /// Switch the monster for another.
            /// </summary>
            Switch,

            /// <summary>
            /// Use an item.
            /// </summary>
            Item,

            /// <summary>
            /// Run from the battle.
            /// </summary>
            Run
        }
    }
}