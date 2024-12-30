using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Actors.CommandGraph;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.Saves;
using WhateverDevs.Core.Runtime.Serialization;
using WhateverDevs.Localization.Runtime;
using Direction = Varguiniano.YAPU.Runtime.Characters.CharacterController.Direction;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Varguiniano.YAPU.Runtime.Characters
{
    /// <summary>
    /// Data class to hold information about a character.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Characters/Character", fileName = "Character")]
    public class CharacterData : SavableObject, IWorldDataContainer, ICommandParameter
    {
        /// <summary>
        /// Localizable name for this character.
        /// </summary>
        [FoldoutGroup("Localization")]
        public string LocalizableName;

        /// <summary>
        /// Get the full localized name of this character.
        /// </summary>
        /// <param name="localizer">Reference to the localizer to use.</param>
        /// <returns>The full localized name.</returns>
        public string GetLocalizedFullName(ILocalizer localizer) =>
            localizer[CharacterType.LocalizableName] + " " + localizer[LocalizableName];

        /// <summary>
        /// Auto fill the localization values.
        /// </summary>
        [FoldoutGroup("Localization")]
        [Button("Auto")]
        private void RefreshLocalizableNames() => LocalizableName = "Characters/" + name;

        /// <summary>
        /// Type of this character.
        /// </summary>
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllCharacterTypes))]
        #endif
        public CharacterType CharacterType;

        /// <summary>
        /// Preview of the character to show in the inspector.
        /// </summary>
        [ShowInInspector]
        [PreviewField(100, ObjectFieldAlignment.Left)]
        [HideLabel]
        [PropertyOrder(-1)]
        private Sprite Preview => CharacterType == null ? null : CharacterType.WorldSprites.LookingDown;

        /// <summary>
        /// Is this character data going to be saved?
        /// </summary>
        [FoldoutGroup("Saves")]
        [SerializeField]
        private bool IsSavable;

        /// <summary>
        /// Default character name.
        /// </summary>
        [FoldoutGroup("Saves")]
        [SerializeField]
        [ShowIf(nameof(IsSavable))]
        private string DefaultName;

        /// <summary>
        /// Default character type.
        /// </summary>
        [FoldoutGroup("Saves")]
        [SerializeField]
        [ShowIf(nameof(IsSavable))]
        #if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllCharacterTypes))]
        #endif
        private CharacterType DefaultType;

        /// <summary>
        /// Get the sprite to look in a direction.
        /// </summary>
        /// <param name="direction">Direction to look into.</param>
        /// <param name="swimming">Is the character swimming?</param>
        /// <param name="biking">Is the character biking?</param>
        /// <param name="running">Is the character running?</param>
        /// <param name="fishing">Is the character fishing?</param>
        /// <returns>A sprite.</returns>
        // ReSharper disable once CyclomaticComplexity
        public Sprite GetLooking(Direction direction, bool swimming, bool biking, bool running, bool fishing) =>
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            direction switch
            {
                Direction.Down when fishing && swimming => CharacterType.WorldSprites.LookingDownFishingSwimming,
                Direction.Down when fishing => CharacterType.WorldSprites.LookingDownFishing,
                Direction.Down when swimming => CharacterType.WorldSprites.LookingDownSwimming,
                Direction.Down when biking => CharacterType.WorldSprites.LookingDownBiking,
                Direction.Down when running => CharacterType.WorldSprites.LookingDownRunning,
                Direction.Down => CharacterType.WorldSprites.LookingDown,

                Direction.Up when fishing && swimming => CharacterType.WorldSprites.LookingUpFishingSwimming,
                Direction.Up when fishing => CharacterType.WorldSprites.LookingUpFishing,
                Direction.Up when swimming => CharacterType.WorldSprites.LookingUpSwimming,
                Direction.Up when biking => CharacterType.WorldSprites.LookingUpBiking,
                Direction.Up when running => CharacterType.WorldSprites.LookingUpRunning,
                Direction.Up => CharacterType.WorldSprites.LookingUp,

                Direction.Left when fishing && swimming => CharacterType.WorldSprites.LookingLeftFishingSwimming,
                Direction.Left when fishing => CharacterType.WorldSprites.LookingLeftFishing,
                Direction.Left when swimming => CharacterType.WorldSprites.LookingLeftSwimming,
                Direction.Left when biking => CharacterType.WorldSprites.LookingLeftBiking,
                Direction.Left when running => CharacterType.WorldSprites.LookingLeftRunning,
                Direction.Left => CharacterType.WorldSprites.LookingLeft,

                Direction.Right when fishing && swimming => CharacterType.WorldSprites.LookingRightFishingSwimming,
                Direction.Right when fishing => CharacterType.WorldSprites.LookingRightFishing,
                Direction.Right when swimming => CharacterType.WorldSprites.LookingRightSwimming,
                Direction.Right when biking => CharacterType.WorldSprites.LookingRightBiking,
                Direction.Right when running => CharacterType.WorldSprites.LookingRightRunning,
                Direction.Right => CharacterType.WorldSprites.LookingRight,

                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

        /// <summary>
        /// Get the sprites to walk in a direction.
        /// </summary>
        /// <param name="direction">Direction to look into.</param>
        /// <param name="swimming">Is the character swimming?</param>
        /// <param name="biking">Is the character biking?</param>
        /// <param name="running">Is the character running.</param>
        /// <returns>A list of sprites.</returns>
        public List<Sprite> GetWalking(Direction direction, bool swimming, bool biking, bool running) =>
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            direction switch
            {
                Direction.Down when swimming => CharacterType.WorldSprites.WalkingDownSwimming,
                Direction.Down when biking => CharacterType.WorldSprites.BikingDown,
                Direction.Down when running => CharacterType.WorldSprites.RunningDown,
                Direction.Down => CharacterType.WorldSprites.WalkingDown,

                Direction.Up when swimming => CharacterType.WorldSprites.WalkingUpSwimming,
                Direction.Up when biking => CharacterType.WorldSprites.BikingUp,
                Direction.Up when running => CharacterType.WorldSprites.RunningUp,
                Direction.Up => CharacterType.WorldSprites.WalkingUp,

                Direction.Left when swimming => CharacterType.WorldSprites.WalkingLeftSwimming,
                Direction.Left when biking => CharacterType.WorldSprites.BikingLeft,
                Direction.Left when running => CharacterType.WorldSprites.RunningLeft,
                Direction.Left => CharacterType.WorldSprites.WalkingLeft,

                Direction.Right when swimming => CharacterType.WorldSprites.WalkingRightSwimming,
                Direction.Right when biking => CharacterType.WorldSprites.BikingRight,
                Direction.Right when running => CharacterType.WorldSprites.RunningRight,
                Direction.Right => CharacterType.WorldSprites.WalkingRight,

                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

        /// <summary>
        /// Copy the data from the given character data.
        /// </summary>
        /// <param name="characterData">Data to copy.</param>
        /// <param name="setLocalizedNameAsKey">Set the localized name as the key? this is useful for the player character.</param>
        /// <param name="localizer">Localizer to use to set the localized name as the key.</param>
        public void CopyFrom(CharacterData characterData,
                             bool setLocalizedNameAsKey = false,
                             ILocalizer localizer = null)
        {
            LocalizableName = setLocalizedNameAsKey
                                  ? localizer![characterData.LocalizableName]
                                  : characterData.LocalizableName;

            CharacterType = characterData.CharacterType;
        }

        /// <summary>
        /// Save the object to a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to be used to save to strings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>A string with the saved object.</returns>
        public override string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter) =>
            serializer.To(new SavableCharacterData(this));

        /// <summary>
        /// Load the object from a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="yapuSettings"></param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        public override IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase)
        {
            SavableCharacterData readData = serializer.From<SavableCharacterData>(data);

            yield return WaitAFrame;

            readData.LoadCharacterData(this, monsterDatabase);
        }

        /// <summary>
        /// Reset the save game.
        /// </summary>
        public override IEnumerator ResetSave()
        {
            if (!IsSavable) yield break;
            LocalizableName = DefaultName;
            CharacterType = DefaultType;
        }

        /// <summary>
        /// Get the localized name of this object.
        /// </summary>
        /// <param name="localizer">Reference to the localizer.</param>
        /// <returns>The localized string.</returns>
        public string GetLocalizedName(ILocalizer localizer) => localizer[CharacterType.LocalizableName];

        #if UNITY_EDITOR

        /// <summary>
        /// Update the name of the object to match the character data.
        /// Only to be used in editor.
        /// </summary>
        [Button]
        [PropertyOrder(-1)]
        private void UpdateName()
        {
            string path = AssetDatabase.GetAssetPath(this);
            AssetDatabase.RenameAsset(path, CharacterType.name + name);
            AssetDatabase.SaveAssets();
        }
        #endif

        /// <summary>
        /// Serializable version of the character data.
        /// </summary>
        [Serializable]
        public class SavableCharacterData
        {
            /// <summary>
            /// Character name.
            /// </summary>
            public string Name;

            /// <summary>
            /// Character type.
            /// </summary>
            public string CharacterType;

            /// <summary>
            /// Constructor from the character data.
            /// </summary>
            /// <param name="characterData">Original character data.</param>
            public SavableCharacterData(CharacterData characterData)
            {
                Name = characterData.LocalizableName;
                CharacterType = characterData.CharacterType.name;
            }

            /// <summary>
            /// Transform back to character data.
            /// </summary>
            /// <returns>A character data object.</returns>
            public void LoadCharacterData(CharacterData characterData, MonsterDatabaseInstance database)
            {
                characterData.LocalizableName = Name;
                characterData.CharacterType = database.GetCharacterTypeByName(CharacterType);
            }
        }
    }
}