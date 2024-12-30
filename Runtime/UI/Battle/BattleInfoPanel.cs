using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Battle.PlayerControl;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Global;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Side;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile;
using Varguiniano.YAPU.Runtime.Battle.Statuses.Weather;
using Varguiniano.YAPU.Runtime.GameFlow;
using Varguiniano.YAPU.Runtime.Input;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Stats;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using Varguiniano.YAPU.Runtime.UI.Moves;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Battle
{
    /// <summary>
    /// Controller for the general battle info panel.
    /// </summary>
    public class BattleInfoPanel : HidableUiElement<MoveInfoPanel>, IInputReceiver
    {
        /// <summary>
        /// Reference to the move pivot
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform MovePivot;

        /// <summary>
        /// Reference to the Title text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Title;

        /// <summary>
        /// Reference to the icon for the time.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image TimeIcon;

        /// <summary>
        /// Reference to the Scenario text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Scenario;

        /// <summary>
        /// Reference to the weather text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Weather;

        /// <summary>
        /// Reference to the terrain text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro Terrain;

        /// <summary>
        /// Reference to the statuses list text.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private LocalizedTextMeshPro StatusesList;

        /// <summary>
        /// Reference to the general info panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement GeneralInfo;

        /// <summary>
        /// Reference to the stage info panel.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private HidableUiElement StageInfo;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text AttackStage;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text DefenseStage;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpAttackStage;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpDefenseStage;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text SpeedStage;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text AccuracyStage;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text EvasionStage;

        /// <summary>
        /// Stage reference.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text CriticalStage;

        /// <summary>
        /// Position when shown.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform ShownPosition;

        /// <summary>
        /// Position when hidden.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Transform HiddenPosition;

        /// <summary>
        /// Reference to all the tips.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private List<HidableUiElement> Tips;

        /// <summary>
        /// Reference to the main battle menu.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private MainBattleMenu MainBattleMenu;

        /// <summary>
        /// Reference to the targets selector.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TargetMonstersMenuSelector TargetsSelector;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private BattleManager BattleManager;

        /// <summary>
        /// Reference to the PlayerControlManager.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private PlayerControlManager PlayerControlManager;

        /// <summary>
        /// Duration of the animation.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private float AnimationDuration;

        /// <summary>
        /// Audio to play when selecting.
        /// </summary>
        [FoldoutGroup("Animation")]
        [SerializeField]
        private AudioReference SelectAudio;

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the time manager.
        /// </summary>
        [Inject]
        private TimeManager timeManager;

        /// <summary>
        /// Show or hide the panel.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        public override void Show(bool show = true) => StartCoroutine(ShowRoutine(show));

        /// <summary>
        /// Show or hide the panel.
        /// </summary>
        /// <param name="show">Show or hide?</param>
        private IEnumerator ShowRoutine(bool show)
        {
            // Make sure it stays in place.
            MovePivot.DOKill();

            yield return WaitAFrame;

            UpdateInfo();

            MovePivot.position = Shown ? ShownPosition.position : HiddenPosition.position;

            if (show)
            {
                // ReSharper disable once BaseMethodCallWithDefaultParameter
                base.Show();

                GeneralInfo.Show();
                StageInfo.Show(false);

                MovePivot.DOMove(ShownPosition.position, AnimationDuration)
                         .SetEase(Ease.OutBack);

                inputManager.RequestInput(this);
            }
            else
            {
                TargetsSelector.Show(false);

                MovePivot.DOMove(HiddenPosition.position, AnimationDuration)
                         .SetEase(Ease.InBack)
                         .OnComplete(() =>
                                     {
                                         base.Show(false);

                                         inputManager.ReleaseInput(this);

                                         MainBattleMenu.Show();
                                     });
            }
        }

        /// <summary>
        /// Close the info panel.
        /// </summary>
        public void OnExtra1(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectAudio);

            Show(false);
        }

        /// <summary>
        /// Same as closing.
        /// </summary>
        public void OnBack(InputAction.CallbackContext context) => OnExtra1(context);

        /// <summary>
        /// Switch to showing monster info.
        /// </summary>
        public void OnExtra2(InputAction.CallbackContext context)
        {
            // Get input only when key goes down.
            if (context.phase != InputActionPhase.Started) return;

            audioManager.PlayAudio(SelectAudio);

            List<Battler> enemyBattlers = BattleManager.Battlers.GetBattlersFighting(BattlerType.Enemy);
            List<Battler> allyBattlers = BattleManager.Battlers.GetBattlersFighting(BattlerType.Ally);
            List<Battler> battlers = new();

            bool[] valid = {false, false, false, false};

            for (int i = 0; i < enemyBattlers.Count; i++)
            {
                valid[i] = true;
                battlers.Add(enemyBattlers[i]);
            }

            while (battlers.Count < 2) battlers.Add(null);

            for (int i = 2; i < allyBattlers.Count + 2; i++)
            {
                valid[i] = true;
                battlers.Add(allyBattlers[i - 2]);
            }

            while (battlers.Count < 4) battlers.Add(null);

            float[] effectiveness = {0f, 0f, 0f, 0f};

            TargetsSelector.SetMonsters(battlers.ToArray(), BattleManager, valid, effectiveness, false);

            TargetsSelector.OnBackSelected += () =>
                                              {
                                                  TargetsSelector.OnBackSelected = null;
                                                  TargetsSelector.OnHovered = null;

                                                  TargetsSelector.Show(false);

                                                  GeneralInfo.Show();
                                                  StageInfo.Show(false);

                                                  foreach (HidableUiElement tip in Tips) tip.Show();

                                                  UpdateInfo();

                                                  if (BattleManager.BattleType == BattleType.DoubleBattle)
                                                      PlayerControlManager
                                                         .AllyPanels[PlayerControlManager.CurrentBattlerInBattleIndex]
                                                         .StartBouncing();
                                              };

            TargetsSelector.OnHovered += UpdateMonsterInfo;

            foreach (HidableUiElement tip in Tips) tip.Show(false);

            GeneralInfo.Show(false);
            StageInfo.Show();

            PlayerControlManager.AllyPanels[PlayerControlManager.CurrentBattlerInBattleIndex].StopBouncing();

            TargetsSelector.Show();
        }

        /// <summary>
        /// Update the info on the panel.
        /// </summary>
        private void UpdateInfo()
        {
            Title.SetValue("Common/General");
            TimeIcon.sprite = timeManager.DayMomentIcon;

            Scenario.SetValue(BattleManager.Scenario.BattleScenario.LocalizableName);

            Weather.SetValue(!BattleManager.Scenario.GetWeather(out Weather weather)
                                 ? "Common/Normal"
                                 : weather.LocalizableName);

            Terrain.SetValue(BattleManager.Scenario.Terrain == null
                                 ? "Common/Normal"
                                 : BattleManager.Scenario.Terrain.LocalizableName);

            StringBuilder builder = new();

            foreach (GlobalStatus status in BattleManager.Statuses.GetGlobalStatuses().Select(slot => slot.Key))
            {
                builder.Append(localizer[status.LocalizableName]);
                builder.AppendLine(".");
            }

            string statuses = builder.ToString();

            if (statuses.IsNullEmptyOrWhiteSpace())
                StatusesList.SetValue("Battle/Info/NoGlobal");
            else
                StatusesList.Text.SetText(statuses);
        }

        /// <summary>
        /// Update the info on the panel with a monster's info.
        /// </summary>
        /// <param name="selectedIndex">Index of the selected monster.</param>
        private void UpdateMonsterInfo(int selectedIndex)
        {
            MonsterInstance monster = ((TargetMonsterButton) TargetsSelector.MenuOptions[selectedIndex]).Monster;

            (BattlerType battlerType, int battlerIndex) =
                BattleManager.Battlers.GetTypeAndIndexOfBattler((Battler) monster);

            Battler battler = BattleManager.Battlers.GetBattlerFromBattleIndex(battlerType, battlerIndex);

            Title.Text.SetText(battler.GetNameOrNickName(localizer));

            SetStageText(AttackStage, battler.StatStage[Stat.Attack]);
            SetStageText(DefenseStage, battler.StatStage[Stat.Defense]);
            SetStageText(SpAttackStage, battler.StatStage[Stat.SpecialAttack]);
            SetStageText(SpDefenseStage, battler.StatStage[Stat.SpecialDefense]);
            SetStageText(SpeedStage, battler.StatStage[Stat.Speed]);

            SetStageText(AccuracyStage, battler.GetAccuracyStage(BattleManager, false));
            SetStageText(EvasionStage, battler.GetEvasionStage(BattleManager, false));

            SetStageText(CriticalStage, battler.CriticalStage);

            StringBuilder builder = new();

            if (battler.GetStatus() != null)
            {
                builder.Append(localizer[battler.GetStatus().LocalizableName]);
                builder.AppendLine(".");
            }

            foreach (KeyValuePair<SideStatus, int> pair in BattleManager.Statuses.GetSideStatuses(battlerType))
            {
                builder.Append(localizer[pair.Key.LocalizableNameKey]);
                builder.AppendLine(".");
            }

            foreach (KeyValuePair<VolatileStatus, int> pair in battler.VolatileStatuses)
            {
                builder.Append(localizer[pair.Key.LocalizableNameKey]);
                builder.AppendLine(".");
            }

            string statuses = builder.ToString();

            if (statuses.IsNullEmptyOrWhiteSpace())
                StatusesList.SetValue("Battle/Info/NoStatus");
            else
                StatusesList.Text.SetText(statuses);
        }

        /// <summary>
        /// Set the text on a stage text.
        /// </summary>
        /// <param name="targetText">Target TMP text.</param>
        /// <param name="value">Value of the stage.</param>
        private static void SetStageText(TMP_Text targetText, int value) =>
            targetText.SetText(value == 0 ? "-" : value.ToString());

        /// <summary>
        /// This receiver is of type UI.
        /// </summary>
        /// <returns></returns>
        public InputType GetInputType() => InputType.UI;

        /// <summary>
        /// Name used for input debugging.
        /// </summary>
        /// <returns></returns>
        public string GetDebugName() => name;

        #region Unused input callbacks

        public void OnStateEnter()
        {
        }

        public void OnStateExit()
        {
        }

        public void OnNavigation(InputAction.CallbackContext context)
        {
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
        }

        public void OnRun(InputAction.CallbackContext context)
        {
        }

        public void OnExtra(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTextBackspace(InputAction.CallbackContext context)
        {
        }

        public void OnTextSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnTextCancel(InputAction.CallbackContext context)
        {
        }

        public void OnAnyTextKey(InputAction.CallbackContext context)
        {
        }

        #endregion
    }
}