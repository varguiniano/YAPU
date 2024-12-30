using UnityEngine;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.World;
using WhateverDevs.Core.Runtime.Build;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Varguiniano.YAPU.Runtime.Build
{
    /// <summary>
    /// Refresh the World and Monster databases before building.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Build/RefreshDatabasesBeforeBuild", fileName = "RefreshDatabasesBeforeBuild")]
    public class RefreshDatabasesBeforeBuild : BuildProcessorHook
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
        public override bool RunHook(string buildPath)
        {
            #if UNITY_EDITOR
            try
            {
                EditorUtility.DisplayProgressBar("Loading", "Refreshing databases...", .25f);

                WorldDatabase.UpdateAll();

                EditorUtility.DisplayProgressBar("Loading", "Refreshing databases...", .5f);
                MonsterDatabase.UpdateAll();
                
                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            #endif

            return true;
        }
    }
}