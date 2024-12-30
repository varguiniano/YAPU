using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Items.Bags;
using Varguiniano.YAPU.Runtime.Player;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Battle
{
    /// <summary>
    /// Controller for the Main battle menu.
    /// </summary>
    public class MainBattleMenu : MenuSelector, IPlayerDataReceiver
    {
        /// <summary>
        /// Reference to the battle info panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BattleInfoPanel BattleInfoPanel;

        /// <summary>
        /// Reference to the battle info panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LastBallMenu LastBallMenu;

        /// <summary>
        /// Reference to the tip that displays the last used ball.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LastBallTip LastBallTip;

        /// <summary>
        /// Reference tot he battle manager.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BattleManager BattleManager;

        /// <summary>
        /// Reference to the global game data.
        /// </summary>
        [Inject]
        private GlobalGameData globalGameData;

        /// <summary>
        /// Reference to the player bag.
        /// </summary>
        [Inject]
        private Bag playerBag;

        /// <summary>
        /// Can the last ball menu be accessed?
        /// </summary>
        private bool lastBallCanBeAccessed;

        /// <summary>
        /// Check if the last ball menu should be accessed.
        /// </summary>
        /// <param name="show"></param>
        public override void Show(bool show = true)
        {
            if (globalGameData != null)
            {
                lastBallCanBeAccessed = BattleManager.EnemyType == EnemyType.Wild
                                     && BattleManager.Battlers.GetBattlersFighting(BattlerType.Enemy).Count == 1
                                     && globalGameData.LastUsedBall != null
                                     && playerBag.Contains(globalGameData.LastUsedBall);

                LastBallTip.Refresh(lastBallCanBeAccessed, globalGameData.LastUsedBall);
            }

            base.Show(show);
        }

        /// <summary>
        /// Display the battle info panel.
        /// </summary>
        /// <param name="context"></param>
        public override void OnExtra1(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            Show(false);

            AudioManager.PlayAudio(SelectAudio);

            BattleInfoPanel.Show();
        }

        /// <summary>
        /// Display the battle info panel.
        /// </summary>
        /// <param name="context"></param>
        public override void OnExtra2(InputAction.CallbackContext context)
        {
            if (!lastBallCanBeAccessed) return;

            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            Show(false);

            AudioManager.PlayAudio(SelectAudio);

            LastBallMenu.Show();
        }
    }
}