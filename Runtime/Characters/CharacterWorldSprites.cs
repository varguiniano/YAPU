using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// World sprites for a character.
    /// </summary>
    [Serializable]
    public class CharacterWorldSprites
    {
        /// <summary>
        /// Sprite for idling down.
        /// </summary>
        [PreviewField]
        public Sprite LookingDown;

        /// <summary>
        /// Sprite for idling up.
        /// </summary>
        [PreviewField]
        public Sprite LookingUp;

        /// <summary>
        /// Sprite for idling left.
        /// </summary>
        [PreviewField]
        public Sprite LookingLeft;

        /// <summary>
        /// Sprite for idling right.
        /// </summary>
        [PreviewField]
        public Sprite LookingRight;
        
        /// <summary>
        /// Sprite for standing while running.
        /// </summary>
        [PreviewField]
        public Sprite LookingDownRunning;

        /// <summary>
        /// Sprite for standing while running.
        /// </summary>
        [PreviewField]
        public Sprite LookingUpRunning;

        /// <summary>
        /// Sprite for standing while running.
        /// </summary>
        [PreviewField]
        public Sprite LookingLeftRunning;

        /// <summary>
        /// Sprite for standing while running.
        /// </summary>
        [PreviewField]
        public Sprite LookingRightRunning;

        /// <summary>
        /// Sprites for walking down.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingDown;

        /// <summary>
        /// Sprites for walking up.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingUp;

        /// <summary>
        /// Sprites for walking left.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingLeft;

        /// <summary>
        /// Sprites for walking right.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingRight;
        
        /// <summary>
        /// Sprites for Running down.
        /// </summary>
        [PreviewField]
        public List<Sprite> RunningDown;

        /// <summary>
        /// Sprites for Running up.
        /// </summary>
        [PreviewField]
        public List<Sprite> RunningUp;

        /// <summary>
        /// Sprites for Running left.
        /// </summary>
        [PreviewField]
        public List<Sprite> RunningLeft;

        /// <summary>
        /// Sprites for Running right.
        /// </summary>
        [PreviewField]
        public List<Sprite> RunningRight;
        
        /// <summary>
        /// Sprite for idling down on a bike.
        /// </summary>
        [PreviewField]
        public Sprite LookingDownBiking;

        /// <summary>
        /// Sprite for idling up on a bike.
        /// </summary>
        [PreviewField]
        public Sprite LookingUpBiking;

        /// <summary>
        /// Sprite for idling left on a bike.
        /// </summary>
        [PreviewField]
        public Sprite LookingLeftBiking;

        /// <summary>
        /// Sprite for idling right on a bike.
        /// </summary>
        [PreviewField]
        public Sprite LookingRightBiking;
        
        /// <summary>
        /// Sprites for Biking down.
        /// </summary>
        [PreviewField]
        public List<Sprite> BikingDown;

        /// <summary>
        /// Sprites for Biking up.
        /// </summary>
        [PreviewField]
        public List<Sprite> BikingUp;

        /// <summary>
        /// Sprites for Biking left.
        /// </summary>
        [PreviewField]
        public List<Sprite> BikingLeft;

        /// <summary>
        /// Sprites for Biking right.
        /// </summary>
        [PreviewField]
        public List<Sprite> BikingRight;
        
        /// <summary>
        /// Sprite for idling down while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingDownSwimming;

        /// <summary>
        /// Sprite for idling up while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingUpSwimming;

        /// <summary>
        /// Sprite for idling left while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingLeftSwimming;

        /// <summary>
        /// Sprite for idling right while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingRightSwimming;
        
        /// <summary>
        /// Sprites for walking down while swimming.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingDownSwimming;

        /// <summary>
        /// Sprites for walking up while swimming.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingUpSwimming;

        /// <summary>
        /// Sprites for walking left while swimming.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingLeftSwimming;

        /// <summary>
        /// Sprites for walking right while swimming.
        /// </summary>
        [PreviewField]
        public List<Sprite> WalkingRightSwimming;
        
        /// <summary>
        /// Sprite for Fishing down.
        /// </summary>
        [PreviewField]
        public Sprite LookingDownFishing;

        /// <summary>
        /// Sprite for Fishing up.
        /// </summary>
        [PreviewField]
        public Sprite LookingUpFishing;

        /// <summary>
        /// Sprite for Fishing left.
        /// </summary>
        [PreviewField]
        public Sprite LookingLeftFishing;

        /// <summary>
        /// Sprite for Fishing right.
        /// </summary>
        [PreviewField]
        public Sprite LookingRightFishing;
        
        /// <summary>
        /// Sprite for Fishing down while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingDownFishingSwimming;

        /// <summary>
        /// Sprite for Fishing down while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingUpFishingSwimming;

        /// <summary>
        /// Sprite for Fishing down while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingLeftFishingSwimming;

        /// <summary>
        /// Sprite for Fishing down while swimming.
        /// </summary>
        [PreviewField]
        public Sprite LookingRightFishingSwimming;
    }
}