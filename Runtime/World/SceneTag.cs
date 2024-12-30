using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.World
{
    /// <summary>
    /// Class representing a tag that can be added to scenes.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Maps/SceneTag", fileName = "SceneTag")]
    public class SceneTag : LocalizableMonsterDatabaseScriptable<SceneTag>
    {
        /// <summary>
        /// Base localization root.
        /// </summary>
        protected override string BaseLocalizationRoot => "SceneTags/";

        /// <summary>
        /// No description.
        /// </summary>
        protected override bool HasLocalizableDescription => false;
    }
}