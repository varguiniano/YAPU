using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Audio module for the battle manager.
    /// </summary>
    public class BattleManagerAudioModule : BattleManagerModule<BattleManagerAudioModule>
    {
        /// <summary>
        /// Music to play when winning a wild battle.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        internal AudioReference WildWinMusic;

        /// <summary>
        /// Music to play when winning a trainer battle.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        internal AudioReference TrainerWinMusic;

        /// <summary>
        /// Music to play when running away.
        /// </summary>
        [FoldoutGroup("Audio")]
        [SerializeField]
        internal AudioReference RunAwaySound;

        /// <summary>
        /// Play an audio once.
        /// </summary>
        /// <param name="audioReference">The audio to play.</param>
        /// <param name="loop">Loop the audio?</param>
        /// <param name="pitch">Pitch of the audio. This affects both pitch and tempo.</param>
        /// <param name="volume">Volume of the audio.</param>
        /// <param name="fadeTime">If DoTween integration is available, set a time for the audio to fade in.</param>
        public void PlayAudio(AudioReference audioReference,
                              bool loop = false,
                              float pitch = 1,
                              float volume = 1,
                              float fadeTime = 0) =>
            BattleManager.AudioManager.PlayAudio(audioReference, loop, pitch, volume, fadeTime);

        /// <summary>
        /// Play cries for all battlers in the battle.
        /// </summary>
        /// <param name="type"></param>
        internal void PlayCries(BattlerType type)
        {
            PlayCry(type, 0);

            if (BattleManager.BattleType == BattleType.DoubleBattle) PlayCry(type, 1);
        }

        /// <summary>
        /// Plays a battler cry.
        /// </summary>
        internal void PlayCry(BattlerType type, int index) =>
            BattleManager.AudioManager.PlayAudio(Battlers
                                                .GetBattlerFromBattleIndex(type, index)
                                                .FormData.Cry,
                                                 pitch: BattleManager.BattleSpeed);

        /// <summary>
        /// Play the sound for running away.
        /// </summary>
        internal void PlayRunAway() => BattleManager.AudioManager.PlayAudio(Audio.RunAwaySound);
    }
}