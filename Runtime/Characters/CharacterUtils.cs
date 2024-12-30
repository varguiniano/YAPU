using System;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Class with character utilities.
    /// </summary>
    public static class CharacterUtils
    {
        /// <summary>
        /// Invert a direction.
        /// </summary>
        /// <param name="direction">Direction to invert.</param>
        /// <returns>Inverted direction.</returns>
        public static CharacterController.Direction Invert(this CharacterController.Direction direction) =>
            direction switch
            {
                CharacterController.Direction.None => CharacterController.Direction.None,
                CharacterController.Direction.Down => CharacterController.Direction.Up,
                CharacterController.Direction.Up => CharacterController.Direction.Down,
                CharacterController.Direction.Left => CharacterController.Direction.Right,
                CharacterController.Direction.Right => CharacterController.Direction.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
    }
}