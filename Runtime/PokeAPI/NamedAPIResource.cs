// ReSharper disable InconsistentNaming

using System;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.PokeAPI
{
    /// <summary>
    /// Class that stores information about a resource in the API.
    /// </summary>
    /// <typeparam name="T">The type of the resoource.</typeparam>
    [Serializable]
    public class NamedAPIResource<T> : Loggable<NamedAPIResource<T>>
    {
        /// <summary>
        /// Name of the resource.
        /// </summary>
        public string name;

        /// <summary>
        /// URL of the resource.
        /// </summary>
        public string url;

        /// <summary>
        /// Cached copy of the resource.
        /// </summary>
        private T resource;

        /// <summary>
        /// Retrieve the data from this resource.
        /// </summary>
        /// <returns></returns>
        public T Resource => resource ??= PokeAPIConnector.GetResource<T>(url);
    }
}