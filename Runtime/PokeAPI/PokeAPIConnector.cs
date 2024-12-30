using UnityEngine;
using UnityEngine.Networking;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class that connects to the PokeAPI (https://pokeapi.co/) and retrieves Pokémon data.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PokeAPIConnector : Loggable<PokeAPIConnector>
    {
        /// <summary>
        /// Base URL for the API.
        /// </summary>
        private const string APIUrl = "https://pokeapi.co/api/v2/";

        /// <summary>
        /// Get a Pokémon species by its id.
        /// </summary>
        /// <param name="id">Id of the mon.</param>
        /// <returns>The class with the species data.</returns>
        public static PokemonSpecies GetPokemonSpecies(uint id) =>
            GetResource<PokemonSpecies>(APIUrl + "pokemon-species/" + id);

        /// <summary>
        /// Get a Pokémon by its id.
        /// </summary>
        /// <param name="speciesAndForm">Species and form of the Pokémon.</param>
        /// <returns>The class with the mon data.</returns>
        public static Pokemon GetPokemon(string speciesAndForm) =>
            GetResource<Pokemon>(APIUrl + "pokemon/" + speciesAndForm);

        /// <summary>
        /// Get a resource from the API.
        /// </summary>
        internal static T GetResource<T>(string url)
        {
            //StaticLogger.Info("Getting resource from " + url);

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SendWebRequest();

            while (!request.isDone)
            {
            }

            if (request.result is not (UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                or UnityWebRequest.Result.DataProcessingError))
                return JsonUtility.FromJson<T>(request.downloadHandler.text);

            StaticLogger.Error("Error getting resource from " + url + " : " + request.error);
            return default;
        }
    }
}