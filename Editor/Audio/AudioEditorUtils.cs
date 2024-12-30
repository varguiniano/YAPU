using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using WhateverDevs.Core.Editor.Utils;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Editor.Audio
{
    /// <summary>
    /// Editor utils for managing audio.
    /// </summary>
    public class AudioEditorUtils
    {
        /// <summary>
        /// Path to the cries folder.
        /// </summary>
        private const string PathToCries = "Assets/YAPUAssets/Audio/Pokemon/Cries";

        /// <summary>
        /// Path to the fx mixer group.
        /// </summary>
        private const string PathToMixerGroup = "Assets/YAPUAssets/Audio/MainAudioMixer.mixer";

        /// <summary>
        /// Register all monster cries to the audio library with the FX mixer group.
        /// </summary>
        [MenuItem("YAPU/Register cries to audio library")]
        private static void RegisterCriesToAudioLibrary() =>
            RegisterAudiosFromFolderToLibrary(PathToCries, PathToMixerGroup);

        /// <summary>
        /// Register all audios in a folder to the audio library with a given mixer group.
        /// </summary>
        private static void RegisterAudiosFromFolderToLibrary(string pathToAudios, string pathToMixerGroup)
        {
            AudioLibrary library = AssetManagementUtils.FindAssetsByType<AudioLibrary>().First();

            AudioMixerGroup group = AssetDatabase.LoadAssetAtPath<AudioMixerGroup>(pathToMixerGroup);

            string[] files = Directory.GetFiles(pathToAudios);

            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];

                    EditorUtility.DisplayProgressBar("Registering audios", file, (float)i / files.Length);

                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(file);

                    if (clip == null) continue;

                    library.AddAudioIfItsNotIn(clip, @group);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
        }
    }
}