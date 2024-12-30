using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Varguiniano.YAPU.Runtime.UI.Monsters
{ 
    /// <summary>
    /// Monster panel that also shows the trainer.
    /// </summary>
    public class MonsterPanelWithTrainer : MonsterPanel
    {
        /// <summary>
        /// Reference to the trainer field.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private TMP_Text Trainer;
        
        /// <summary>
        /// Update the icon.
        /// </summary>
        public override void UpdatePanel(float speed, bool playLowHealthSound = true, bool tween = false, Action finished = null)
        {
            if (Trainer != null)
                Trainer.SetText(GetMonster().CurrentTrainer);
            
            base.UpdatePanel(speed, playLowHealthSound, tween, finished);
        }
    }
}