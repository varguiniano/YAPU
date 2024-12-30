using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Controller of the battle camera.
    /// </summary>
    public class BattleCamera : WhateverBehaviour<BattleCamera>
    {
        /// <summary>
        /// Reference to the brain.
        /// </summary>
        [FoldoutGroup("Cameras")]
        [SerializeField]
        private CinemachineBrain Brain;

        /// <summary>
        /// Reference to the main camera.
        /// </summary>
        [FoldoutGroup("Cameras")]
        [SerializeField]
        private CinemachineVirtualCamera MainCamera;

        /// <summary>
        /// Reference to the enemy camera.
        /// </summary>
        [FoldoutGroup("Cameras")]
        [SerializeField]
        private CinemachineVirtualCamera EnemyCamera;

        /// <summary>
        /// Blend duration for the camera changes.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float BlendDuration = 1f;

        /// <summary>
        /// Flag to know when the cameras are moving.
        /// </summary>
        [FoldoutGroup("Debug")]
        [ShowInInspector]
        [ReadOnly]
        public bool CamerasMoving => Brain.IsBlending;

        /// <summary>
        /// Stack of all modified virtual cameras.
        /// </summary>
        private readonly Stack<CinemachineVirtualCamera> modifiedCameras = new();

        /// <summary>
        /// Return priorities to main.
        /// <param name="speed">Speed at which to perform the blend.</param>
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void ReturnToMain(float speed)
        {
            UpdateSpeed(speed);
            while (modifiedCameras.Count > 0) modifiedCameras.Pop().Priority = 0;
        }

        /// <summary>
        /// Focus on the enemy.
        /// <param name="speed">Speed at which to perform the blend.</param>
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void FocusOnEnemy(float speed)
        {
            UpdateSpeed(speed);
            EnemyCamera.Priority = 20;
            modifiedCameras.Push(EnemyCamera);
        }

        /// <summary>
        /// Updates the speed of the blend.
        /// </summary>
        /// <param name="speed">Speed at which to perform the blend.</param>
        private void UpdateSpeed(float speed) => Brain.DefaultBlend.Time = BlendDuration / speed;

        /// <summary>
        /// Shake the main camera for a certain amount of time.
        /// </summary>
        /// <param name="duration">Time to shake the camera.</param>
        /// <param name="amplitude">Shake amplitude.</param>
        /// <param name="frequency">Shake frequency.</param>
        public IEnumerator ShakeCamera(float duration, float amplitude = 1, float frequency = 1)
        {
            CinemachineBasicMultiChannelPerlin noise =
                MainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            noise.AmplitudeGain = amplitude;
            noise.FrequencyGain = frequency;

            yield return new WaitForSeconds(duration);

            noise.AmplitudeGain = 0;
            noise.FrequencyGain = 0;
        }
    }
}