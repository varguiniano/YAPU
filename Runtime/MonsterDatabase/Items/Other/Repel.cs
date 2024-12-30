using Sirenix.OdinInspector;
using UnityEngine;
using WhateverDevs.TwoDAudio.Runtime;

namespace Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Other
{
    /// <summary>
    /// Base class for repels.
    /// </summary>
    [CreateAssetMenu(menuName = "YAPU/Items/Other/Repel", fileName = "Repel")]
    public class Repel : OutOfBattleUsableItem
    {
        /// <summary>
        /// Amount of steps the repel has.
        /// </summary>
        [FoldoutGroup("Repel")]
        public uint RepelAmount;

        /// <summary>
        /// Sound to play when using the repel.
        /// </summary>
        [FoldoutGroup("Repel")]
        public AudioReference RepelSound;
    }
}