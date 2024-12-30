using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Battle;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase;
using Varguiniano.YAPU.Runtime.UI.Abilities;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using Varguiniano.YAPU.Runtime.UI.Monsters;
using Varguiniano.YAPU.Runtime.UI.Moves;
using Varguiniano.YAPU.Runtime.UI.Types;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.TwoDAudio.Runtime;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Battle
{
    /// <summary>
    /// Controller for the monsters menu inside a battle.
    /// </summary>
    public class BattleMonstersMenu : HidableUiElement<BattleMonstersMenu>
    {
        /// <summary>
        /// Event raised when the player has chosen a monster to switch to.
        /// </summary>
        public Action<int> MonsterToSwitchChosen;

        /// <summary>
        /// Event raised when the menu is closed.
        /// </summary>
        public Action MenuClosed;

        /// <summary>
        /// Reference to the background image.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Image Background;

        /// <summary>
        /// Left section of the menu.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSection;

        /// <summary>
        /// Position for the left section when it is closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSectionClosedPosition;

        /// <summary>
        /// Position of the left section when it is open.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform LeftSectionOpenPosition;

        /// <summary>
        /// Upper section of the menu.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform UpperSection;

        /// <summary>
        /// Position for the Upper section when it is closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform UpperSectionClosedPosition;

        /// <summary>
        /// Position of the Upper section when it is open.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private Transform UpperSectionOpenPosition;

        /// <summary>
        /// Reference to the monsters selector.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private MenuSelector MonstersSelector;

        /// <summary>
        /// State of this monster.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private LocalizedTextMeshPro State;

        /// <summary>
        /// Reference to the first type badge.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private TypeBadge FirstType;

        /// <summary>
        /// Reference to the second type badge.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private TypeBadge SecondType;

        /// <summary>
        /// Reference to the display of the moves of this monster.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private MonsterMovesDisplay MovesDisplay;

        /// <summary>
        /// Reference to the panel to show the ability.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private AbilityPanel AbilityPanel;

        /// <summary>
        /// Reference to the monster sprite.
        /// </summary>
        [FormerlySerializedAs("MonsterSprite")]
        [SerializeField]
        [FoldoutGroup("References")]
        private UIMonsterSprite UIMonsterSprite;

        /// <summary>
        /// Reference to the hider of the monster sprite.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private HidableUiElement SpriteHider;

        /// <summary>
        /// Reference to the main menu.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private MenuSelector MainMenu;

        /// <summary>
        /// Reference to the battle manager.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("References")]
        private BattleManager BattleManager;

        /// <summary>
        /// Duration of the open/close animation.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Animation")]
        private float OpenCloseDuration;

        /// <summary>
        /// Audio to play when the summary menu is closed.
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Animation")]
        private AudioReference OnSummaryBackAudio;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// Reference to the audio manager.
        /// </summary>
        [Inject]
        private IAudioManager audioManager;

        /// <summary>
        /// Flag to know if the menu can be closed.
        /// This helps differentiating between opening this menu as choosing an action or as choosing after fainting.
        /// </summary>
        private bool canBeClosed;

        /// <summary>
        /// Flag to know if the main menu should be opened on close.
        /// This helps differentiating between opening this menu as choosing an action or as choosing for other reasons.
        /// </summary>
        private bool mainMenuOnClose;

        /// <summary>
        /// The current battler selecting the action.
        /// </summary>
        private Battler currentBattler;

        /// <summary>
        /// List of displayed battlers.
        /// </summary>
        private List<Battler> battlers;

        /// <summary>
        /// Subscribe to the main menu selecting moves.
        /// </summary>
        private void OnEnable()
        {
            MonstersSelector.OnButtonSelected += OnMonsterSelected;
            MonstersSelector.OnBackSelected += OnBackSelected;
            MonstersSelector.OnHovered += OnMonsterHovered;
        }

        /// <summary>
        /// Unsubscribe.
        /// </summary>
        private void OnDisable()
        {
            MonstersSelector.OnButtonSelected -= OnMonsterSelected;
            MonstersSelector.OnBackSelected -= OnBackSelected;
            MonstersSelector.OnHovered -= OnMonsterHovered;
        }

        /// <summary>
        /// Open the menu.
        /// </summary>
        [Button]
        [FoldoutGroup("Debug")]
        public void OpenMenu(Battler currentBattlerReference = null,
                             bool canMenuBeClosed = true,
                             bool openMainMenuOnClose = true)
        {
            canBeClosed = canMenuBeClosed;
            mainMenuOnClose = openMainMenuOnClose;

            currentBattler = currentBattlerReference;

            LeftSection.localPosition = LeftSectionClosedPosition.localPosition;

            Show();

            Background.DOFade(1, OpenCloseDuration);

            UpperSection.DOLocalMove(UpperSectionOpenPosition.localPosition, OpenCloseDuration);

            LeftSection.DOLocalMove(LeftSectionOpenPosition.localPosition, OpenCloseDuration)
                       .OnComplete(() => MonstersSelector.Show());
        }

        /// <summary>
        /// Open the menu.
        /// </summary>
        [Button]
        [FoldoutGroup("Debug")]
        public void CloseMenu(bool reopenMain)
        {
            MonstersSelector.Show(false);
            MovesDisplay.Show(false);
            AbilityPanel.Show(false);
            SpriteHider.Show(false);

            UpperSection.DOLocalMove(UpperSectionClosedPosition.localPosition, OpenCloseDuration);
            LeftSection.DOLocalMove(LeftSectionClosedPosition.localPosition, OpenCloseDuration);

            Background.DOFade(0, OpenCloseDuration)
                      .OnComplete(() =>
                                  {
                                      Show(false);
                                      if (reopenMain) MainMenu.Show();
                                      MenuClosed?.Invoke();
                                  });
        }

        /// <summary>
        /// Sets the monsters of the menu.
        /// </summary>
        /// <param name="newBattlers">List of battlers to set.</param>
        public void SetMonsters(List<Battler> newBattlers)
        {
            battlers = newBattlers;

            for (int i = 0; i < MonstersSelector.MenuOptions.Count; i++)
            {
                MenuItem menuItem = MonstersSelector.MenuOptions[i];

                if (i >= battlers.Count)
                    menuItem.Hide();
                else
                    ((MonsterButton) menuItem).Panel.SetMonster(battlers[i], false);
            }
        }

        /// <summary>
        /// Called when a monster is hovered.
        /// </summary>
        /// <param name="index">Index of the monster hovered.</param>
        private void OnMonsterHovered(int index)
        {
            MonsterInstance monster = ((MonsterButton) MonstersSelector.MenuOptions[index]).Panel.GetMonster();

            // If not in battle or double battle, no need for effectiveness.
            if (BattleManager == null || BattleManager.BattleType == BattleType.DoubleBattle)
                MovesDisplay.SetMoves(monster);
            else
            {
                Battler enemy =
                    BattleManager.Battlers.GetBattlerFromBattleIndex(BattlerType.Enemy, 0);

                Battler moveOwner = (Battler) monster;

                List<bool> useEffectiveness = new();
                List<float> effectiveness = new();

                foreach (MoveSlot slot in monster.CurrentMoves)
                    if (BattleManager.BattleType == BattleType.SingleBattle)
                    {
                        useEffectiveness.Add(enemy.GetEffectivenessOfMove(moveOwner,
                                                                          slot.Move,
                                                                          false,
                                                                          BattleManager,
                                                                          false,
                                                                          out float effectivenessValue));

                        effectiveness.Add(effectivenessValue);
                    }
                    else
                    {
                        useEffectiveness.Add(false);
                        effectiveness.Add(0);
                    }

                MovesDisplay.SetMoves(monster, effectiveness, useEffectiveness);
            }

            MovesDisplay.Show(!monster.EggData.IsEgg);

            if (monster.EggData.IsEgg)
                State.SetValue("Monsters/Egg");
            else if (monster.CurrentHP == 0)
                State.SetValue("Status/Fainted");
            else if (BattleManager.Battlers.IsBattlerFighting(BattlerType.Ally, MonstersSelector.CurrentSelection))
                State.SetValue("Battle/Monster/State/InBattle");
            else
                State.SetValue("Battle/Monster/State/CanBattle");

            (MonsterType firstType, MonsterType secondType) = monster.GetTypes(BattleManager.YAPUSettings);

            FirstType.SetType(firstType);
            SecondType.SetType(secondType);

            AbilityPanel.SetAbility(monster.GetAbility());
            AbilityPanel.Show(!monster.EggData.IsEgg);

            UIMonsterSprite.SetMonster(monster);
            SpriteHider.Show(monster.EggData.IsEgg);
        }

        /// <summary>
        /// Called when a monster is selected.
        /// </summary>
        /// <param name="index">Index of the selected monster.</param>
        private void OnMonsterSelected(int index) =>
            DialogManager.ShowChoiceMenu(new List<string>
                                         {
                                             "Common/Switch",
                                             "Monsters/Summary",
                                             "Common/Cancel"
                                         },
                                         position: ((MonsterButton) MonstersSelector.MenuOptions[index])
                                        .ContextMenuPosition,
                                         callback: OnMonsterSubmenuChosen,
                                         onBackCallback: () => audioManager.PlayAudio(OnSummaryBackAudio));

        /// <summary>
        /// Called when the back button has been pressed.
        /// </summary>
        private void OnBackSelected()
        {
            if (!canBeClosed) return;

            CloseMenu(mainMenuOnClose);
        }

        /// <summary>
        /// Called when an option on the monster submenu is chosen.
        /// </summary>
        /// <param name="index">Index of the chosen option.</param>
        private void OnMonsterSubmenuChosen(int index) => StartCoroutine(OnMonsterSubmenuChosenRoutine(index));

        /// <summary>
        /// Called when an option on the monster submenu is chosen.
        /// We need to keep the dialog on top of other menus so routine time.
        /// </summary>
        /// <param name="index">Index of the chosen option.</param>
        private IEnumerator OnMonsterSubmenuChosenRoutine(int index)
        {
            switch (index)
            {
                case 0:

                    Battler battler =
                        BattleManager.Battlers.GetBattlerFromRosterAndIndex(BattlerType.Ally,
                                                                            0,
                                                                            MonstersSelector.CurrentSelection);

                    if (currentBattler != null
                     && !currentBattler.CanSwitch(BattleManager,
                                                  BattlerType.Ally,
                                                  0,
                                                  null,
                                                  false,
                                                  null,
                                                  false,
                                                  true))
                        yield break;

                    if (battler.EggData.IsEgg)
                    {
                        yield return WaitAFrame;

                        DialogManager.ShowDialog("Battle/SwapMon/IsEgg");

                        yield break;
                    }

                    if (battler.CurrentHP <= 0)
                    {
                        yield return WaitAFrame;

                        DialogManager.ShowDialog("Battle/SwapMon/IsFainted",
                                                 localizableModifiers: false,
                                                 modifiers:
                                                 battler.GetNameOrNickName(localizer));

                        yield break;
                    }

                    if (MonstersSelector.CurrentSelection
                      < BattleManager.Battlers.GetNumberOfBattlersUnderPlayersControl()
                     && BattleManager.Battlers.IsBattlerFighting(BattlerType.Ally, MonstersSelector.CurrentSelection))
                    {
                        yield return WaitAFrame;

                        DialogManager.ShowDialog("Battle/SwapMon/AlreadyFighting",
                                                 localizableModifiers: false,
                                                 modifiers: battler.GetNameOrNickName(localizer));

                        yield break;
                    }

                    SwitchMonster(MonstersSelector.CurrentSelection);

                    CloseMenu(false);
                    break;
                case 1:
                    DialogManager.ShowMonsterSummary(new List<MonsterInstance>(battlers),
                                                     MonstersSelector.CurrentSelection,
                                                     BattleManager,
                                                     BattleManager.PlayerCharacter,
                                                     lastIndex =>
                                                         DOVirtual.DelayedCall(.1f,
                                                                               () => MonstersSelector.Select(lastIndex,
                                                                                   false)));

                    break;
                // Nothing if canceled, closing the submenu is directly handled by it.
            }
        }

        /// <summary>
        /// Called when the player chooses to switch a monster.
        /// </summary>
        /// <param name="index">Index of the monster to switch to.</param>
        private void SwitchMonster(int index) => MonsterToSwitchChosen?.Invoke(index);
    }
}