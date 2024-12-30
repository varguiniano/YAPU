namespace Varguiniano.YAPU.Runtime.Actors
{
    /// <summary>
    /// Actor move target that has no collision so that the player can get on top to interact.
    /// </summary>
    public class OnTopActorMoveTarget : ActorMoveTarget
    {
        /// <summary>
        /// No collision with the player.
        /// </summary>
        public override bool BlocksMovement => false;
    }
}