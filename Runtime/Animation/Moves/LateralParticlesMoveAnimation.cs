using UnityEngine;
using UnityEngine.VFX;
using Varguiniano.YAPU.Runtime.Battle;
using WhateverDevs.Core.Behaviours;

namespace Varguiniano.YAPU.Runtime.Animation.Moves
{
    /// <summary>
    /// Behaviour for the animation of a move that has a different vfx for allies and enemies.
    /// </summary>
    public class LateralParticlesMoveAnimation : WhateverBehaviour<LateralParticlesMoveAnimation>
    {
        /// <summary>
        /// FX to play on the enemy.
        /// </summary>
        [SerializeField]
        private VisualEffect EnemyFX;

        /// <summary>
        /// FX to play on the ally.
        /// </summary>
        [SerializeField]
        private VisualEffect AllyFX;

        /// <summary>
        /// Play the VFX.
        /// </summary>
        /// <param name="target">Target of the VFX.</param>
        /// <param name="textureOverride">Texture override.</param>
        /// <param name="colorOverride">Color override.</param>
        public void PlayFX(BattlerType target,
                           Texture2D textureOverride = null,
                           Color colorOverride = default)
        {
            switch (target)
            {
                case BattlerType.Ally:
                    ActivateFX(AllyFX, textureOverride, colorOverride);
                    break;
                case BattlerType.Enemy:
                    ActivateFX(EnemyFX, textureOverride, colorOverride);
                    break;
                default: throw new BattleManager.UnsupportedBattlerException(target);
            }
        }

        /// <summary>
        /// Activate the FX for one side.
        /// </summary>
        /// <param name="fx">Fx to activate.</param>
        /// <param name="textureOverride">Texture override.</param>
        /// <param name="colorOverride">Color override.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ActivateFX(VisualEffect fx, Texture2D textureOverride = null, Color colorOverride = default)
        {
            fx.enabled = true;

            if (textureOverride != null) fx.SetTexture("Texture", textureOverride);
            if (colorOverride != default) fx.SetVector4("Color", colorOverride);
        }

        /// <summary>
        /// Stop the FX.
        /// </summary>
        public void StopVFX()
        {
            AllyFX.Stop();
            EnemyFX.Stop();
        }
    }
}