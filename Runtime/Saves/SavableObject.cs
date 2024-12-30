using System.Collections;
using Varguiniano.YAPU.Runtime.Characters;
using Varguiniano.YAPU.Runtime.Configuration;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using WhateverDevs.Core.Runtime.Serialization;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Base class for objects that can be saved, loaded and reset.
    /// </summary>
    public abstract class SavableObject : MonsterDatabaseScriptable<SavableObject>
    {
        /// <summary>
        /// Save the object to a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to be used to save to strings.</param>
        /// <param name="playerCharacter">Reference to the player character.</param>
        /// <returns>A string with the saved object.</returns>
        public abstract string SaveToText(ISerializer<string> serializer, PlayerCharacter playerCharacter);

        /// <summary>
        /// Load the object from a persistable text.
        /// </summary>
        /// <param name="serializer">Serializer to use when loading.</param>
        /// <param name="data">Text containing the data to load.</param>
        /// <param name="yapuSettings">Reference to the YAPUSettings.</param>
        /// <param name="monsterDatabase">Reference to the monster database.</param>
        public abstract IEnumerator LoadFromText(ISerializer<string> serializer,
                                                 string data,
                                                 YAPUSettings yapuSettings,
                                                 MonsterDatabaseInstance monsterDatabase);

        /// <summary>
        /// Reset the data to its default values.
        /// </summary>
        public abstract IEnumerator ResetSave();
    }
}