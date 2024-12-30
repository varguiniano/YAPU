using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase
{
    /// <summary>
    /// Base class for localizable database scriptables that also have links to documentation.
    /// </summary>
    /// <typeparam name="T">Inheriting class.</typeparam>
    public abstract class DocumentableMonsterDatabaseScriptable<T> : LocalizableMonsterDatabaseScriptable<T>
        where T : DocumentableMonsterDatabaseScriptable<T>
    {
        /// <summary>
        /// Quick access link to open this move in https://bulbapedia.bulbagarden.net/.
        /// </summary>
        [Button("Open documentation")]
        [PropertyOrder(-1)]
        [HideIf(nameof(IsURLEmpty))]
        private void OpenDocs() => Application.OpenURL(DocsURL);

        /// <summary>
        /// URL to its Bulbapedia page.
        /// </summary>
        [ShowIf(nameof(IsURLEmpty))]
        [PropertyOrder(-1)]
        [SerializeField]
        internal string DocsURL;

        /// <summary>
        /// Is the URL empty?
        /// </summary>
        private bool IsURLEmpty => DocsURL.IsNullEmptyOrWhiteSpace();
    }
}