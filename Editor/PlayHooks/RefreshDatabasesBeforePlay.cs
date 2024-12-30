using UnityEditor;
using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.DefaultToolBarButtons.Editor;

namespace Varguiniano.YAPU.Editor.PlayHooks
{
    /// <summary>
    /// Refresh the World and Monster databases before playing.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/PlayHooks/RefreshDatabasesBeforePlay", fileName = "RefreshDatabasesBeforePlay")]
    public class RefreshDatabasesBeforePlay : PlayHook
    {
        /// <summary>
        /// Reference to the World Database.
        /// </summary>
        [SerializeField]
        private WorldDatabase WorldDatabase;

        /// <summary>
        /// Monster database.
        /// </summary>
        [SerializeField]
        private MonsterDatabaseInstance MonsterDatabase;

        /// <summary>
        /// Refresh the databases.
        /// </summary>
        public override void Run()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Loading", "Refreshing databases...", .75f);
                WorldDatabase.UpdateAll();

                EditorUtility.DisplayProgressBar("Loading", "Refreshing databases...", .85f);
                MonsterDatabase.UpdateAll();

                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}