using System.Runtime.Serialization;
using UnityEngine;
using Varguiniano.YAPU.Runtime.GameFlow;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Core.Runtime.Serialization;
using Zenject;

namespace Varguiniano.YAPU.Runtime.Saves
{
    /// <summary>
    /// Serializer to use when saving and loading the game.
    /// </summary>
    public class YAPUSavesSerializer : Loggable<JsonSerializer>, ISerializer<string>
    {
        /// <summary>
        /// Reference the configuration manager.
        /// </summary>
        [Inject]
        private IConfigurationManager configurationManager;

        /// <summary>
        /// Transform the data to a Json string.
        /// </summary>
        /// <param name="original">Original data.</param>
        /// <typeparam name="TOriginal">Type of the original data.</typeparam>
        /// <returns>The data as a Json string.</returns>
        public string To<TOriginal>(TOriginal original)
        {
            if (typeof(TOriginal).IsSerializable)
            {
                if (configurationManager.GetConfiguration(out GameplayConfiguration gameplayConfiguration))
                    return JsonUtility.ToJson(original,
                                              gameplayConfiguration.SavegameFormat
                                           == GameplayConfiguration.SaveFormat.ReadableJson);

                Logger.Error("Error retrieving the configuration.");
                return "";
            }

            Logger.Error("Data is not serializable, will not serialize.");
            return "";
        }

        /// <summary>
        /// Transforms data from a Json string to the given type.
        /// </summary>
        /// <param name="serialized">Data as a Json string.</param>
        /// <typeparam name="TOriginal">Type of the original data.</typeparam>
        /// <returns>The original data in the original type.</returns>
        public TOriginal From<TOriginal>(string serialized)
        {
            if (typeof(TOriginal).IsSerializable) return JsonUtility.FromJson<TOriginal>(serialized);

            Logger.Error("Data type is not serializable, will not deserialize.");
            throw new SerializationException(); // We can't just return null as TOriginal may not be nullable.
        }
    }
}