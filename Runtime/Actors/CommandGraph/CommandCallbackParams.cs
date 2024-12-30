namespace Varguiniano.YAPU.Runtime.Actors.CommandGraph
{
    /// <summary>
    /// Callback params sent by a command callback.
    /// </summary>
    public class CommandCallbackParams
    {
        /// <summary>
        /// Should the looping be updated?
        /// </summary>
        public bool UpdateLooping;

        /// <summary>
        /// New looping value.
        /// </summary>
        public bool NewLooping;

        /// <summary>
        /// Stop the player for further movement.
        /// </summary>
        public bool StopFurtherPlayerMovement;

        /// <summary>
        /// Destroy the actor that was pushed.
        /// </summary>
        public bool DestroyPushedActor;
    }
}