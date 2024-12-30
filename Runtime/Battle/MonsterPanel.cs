using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using WhateverDevs.Core.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.Battle
{
    /// <summary>
    /// Controller for the panel that shows the state of a monster.
    /// </summary>
    public class MonsterPanel : MonsterPanelBase
    {
        /// <summary>
        /// Reference to the panel transform.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform Panel;

        /// <summary>
        /// Icon to represent if the monster has been caught before.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private HidableUiElement CaughtIcon;

        /// <summary>
        /// Position to go to when off camera.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private float OffCameraPosition;

        /// <summary>
        /// Sliding duration.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Configuration")]
        private float SlideDuration = .5f;

        /// <summary>
        /// Is the panel sliding?
        /// </summary>
        private bool sliding;

        /// <summary>
        /// Reference to the bouncing tween.
        /// </summary>
        private Tween bouncing;

        /// <summary>
        /// Is the panel bouncing?
        /// </summary>
        private bool isBouncing;

        /// <summary>
        /// Updates the data in the panel.
        /// </summary>
        public void SetMonsterInBattle(MonsterInstance monsterReference,
                                       BattleManager battleManager,
                                       BattlerType battlerType,
                                       bool playLowHealthSound = true)
        {
            if (CaughtIcon != null)
                StartCoroutine(battleManager.Dex.GetEntry(monsterReference.Species,
                                                          entry =>
                                                          {
                                                              CaughtIcon.Show(battlerType == BattlerType.Enemy
                                                                           && battleManager.EnemyType == EnemyType.Wild
                                                                           && entry
                                                                             .GetEntryForForm(monsterReference.Form)
                                                                             .HasFormBeenCaught);
                                                          }));

            SetMonster(monsterReference, playLowHealthSound);
        }

        /// <summary>
        /// Slide in camera.
        /// </summary>
        /// <param name="immediately">Tween or immediate?</param>
        [Button]
        [HideInEditorMode]
        public void SlideIn(bool immediately = false) => Slide(0, immediately);

        /// <summary>
        /// Slide off camera.
        /// </summary>
        /// <param name="immediately">Tween or immediate?</param>
        [Button]
        [HideInEditorMode]
        public void SlideOut(bool immediately = false) => Slide(OffCameraPosition, immediately);

        /// <summary>
        /// Start bouncing the panel.
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void StartBouncing() => StartCoroutine(StartBoundingRoutine());

        /// <summary>
        /// Start bouncing the panel.
        /// Wait if sliding.
        /// </summary>
        private IEnumerator StartBoundingRoutine()
        {
            yield return new WaitUntil(() => !sliding);

            isBouncing = true;
            bouncing = Panel.DOShakePosition(1, new Vector3(10, 0, 0)).SetLoops(-1).OnKill(() => isBouncing = false);
        }

        /// <summary>
        /// Stop bouncing the panel.
        /// </summary>
        [Button]
        [HideInEditorMode]
        public void StopBouncing() => bouncing?.Kill();

        /// <summary>
        /// Slide to the target position.
        /// </summary>
        /// <param name="target">Target X position.</param>
        /// <param name="immediately">Tween or immediate?</param>
        private void Slide(float target, bool immediately = false)
        {
            if (isBouncing) StopBouncing();

            if (immediately)
            {
                Vector3 localPosition = Panel.localPosition;
                localPosition = new Vector3(target, localPosition.y, localPosition.z);
                Panel.localPosition = localPosition;
            }
            else
            {
                sliding = true;
                Panel.DOLocalMoveX(target, SlideDuration).OnComplete(() => sliding = false);
            }
        }
    }
}