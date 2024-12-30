using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.MonsterDatabase.Experience;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Dialogs.XPGainPopup
{
    /// <summary>
    /// Controller of the xp gain popup dialog.
    /// </summary>
    public class RosterXPPanel : HidableUiElement<RosterXPPanel>
    {
        /// <summary>
        /// Reference to the panels.
        /// </summary>
        [SerializeField]
        private MonsterXPPanel[] Panels;

        /// <summary>
        /// Position when shown.
        /// </summary>
        [SerializeField]
        private Transform ShownPosition;

        /// <summary>
        /// Position when hidden.
        /// </summary>
        [SerializeField]
        private Transform HiddenPosition;

        /// <summary>
        /// Flag to know if we are waiting for dialog.
        /// </summary>
        private bool waitingForDialog;

        /// <summary>
        /// Experience lookup table to use.
        /// </summary>
        [Inject]
        private ExperienceLookupTable experienceLookupTable;

        /// <summary>
        /// Show the popup with the XP gain animations and data.
        /// </summary>
        /// <param name="roster">Roster to show.</param>
        /// <param name="amounts">Amounts they have gained.</param>
        /// <param name="levelUps">Level ups they have achieved.</param>
        public IEnumerator ShowXPGain(List<MonsterInstance> roster,
                                      List<uint> amounts,
                                      List<List<MonsterInstance.LevelUpData>> levelUps)
        {
            for (int i = 0; i < Panels.Length; ++i)
            {
                if (i >= roster.Count || roster[i] == null || roster[i].IsNullEntry)
                {
                    Panels[i].gameObject.SetActive(false);
                    continue;
                }

                Panels[i].gameObject.SetActive(true);
                Panels[i].SetMonster(roster[i]);

                if (levelUps[i].Count > 0)
                {
                    Panels[i].UpdateLevel((byte)(levelUps[i][0].Level - 1));

                    Panels[i]
                       .SetBarValue(levelUps[i][0].PreviousXP,
                                    (uint)experienceLookupTable.GetExperienceNeededForNextLevel(roster[i]
                                           .FormData.GrowthRate,
                                        levelUps[i][0].Level - 1));
                }
                else
                    Panels[i]
                       .SetBarValue(roster[i].StatData.CurrentLevelExperience - amounts[i],
                                    (uint)roster[i].GetExperienceForNextLevel(experienceLookupTable));
            }

            transform.position = HiddenPosition.position;

            Show();

            yield return WaitAFrame;

            yield return transform.DOMove(ShownPosition.position, .5f).SetEase(Ease.OutBack).WaitForCompletion();

            bool[] finished = { false, false, false, false, false, false };

            for (int i = 0; i < Panels.Length; ++i)
            {
                if (i >= roster.Count || roster[i] == null || roster[i].IsNullEntry) continue;
                int index = i;
                Panels[i].PlayAnimation(amounts[i], levelUps[i], () => finished[index] = true);
            }

            yield return new WaitForSeconds(1f);

            yield return new WaitUntil(AllAnimationsFinished);

            DialogManager.AcceptInput = true;

            for (int i = 0; i < Panels.Length; ++i)
            {
                if (i >= roster.Count || roster[i] == null || roster[i].IsNullEntry) continue;
                int index = i;
                yield return new WaitUntil(() => finished[index]);
            }

            yield return DialogManager.WaitForDialog;

            yield return transform.DOMove(HiddenPosition.position, .5f)
                                  .SetEase(Ease.InBack)
                                  .WaitForCompletion();

            Show(false);
        }

        /// <summary>
        /// Check if all panel animations have finished.
        /// </summary>
        /// <returns></returns>
        private bool AllAnimationsFinished()
        {
            bool finished = true;

            foreach (MonsterXPPanel panel in Panels)
                if (panel.Animating)
                    finished = false;

            return finished;
        }
    }
}