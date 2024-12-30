using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.Monster;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Side
{
    /// <summary>
    /// Base class for a side status that can have several layers on top of each other.
    /// </summary>
    public abstract class LayeredSideStatus : SideStatus
    {
        /// <summary>
        /// Get the localization key for this status' dialog when it ends.
        /// </summary>
        /// <returns>A string with the localization key.</returns>
        [FoldoutGroup("Localization")]
        public string LayerAddedKey;

        /// <summary>
        /// Layer count for the two sides.
        /// </summary>
        [FoldoutGroup("Layers")]
        [SerializeField]
        [HideInEditorMode]
        protected SerializableDictionary<BattlerType, uint> LayerCount = new();

        /// <summary>
        /// Auto generate the localization based on the object name.
        /// </summary>
        protected override void GenerateLocalization()
        {
            base.GenerateLocalization();
            LayerAddedKey = LocalizableNameKey + "/LayerAdded";
        }

        /// <summary>
        /// Play an animation when this status starts.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="side">Side to add it on.</param>
        /// <param name="sideOwner">Owner of the side, used for dialogs.</param>
        /// <param name="extraData">Extra data provided when adding the status.</param>
        public override IEnumerator StartAnimation(BattleManager battleManager,
                                                   BattlerType side,
                                                   string sideOwner,
                                                   params object[] extraData)
        {
            // Make sure dirty layers that may have been left behind are removed.
            if (LayerCount.ContainsKey(side)) LayerCount.Remove(side);

            yield return base.StartAnimation(battleManager, side, sideOwner, extraData);
        }

        /// <summary>
        /// Add a number of layers to the side.
        /// </summary>
        public IEnumerator AddLayers(BattlerType side,
                                     uint layerNumber,
                                     Battler target,
                                     Battler user,
                                     BattleManager battleManager)
        {
            for (uint i = 0; i < layerNumber; ++i) yield return AddLayer(side, target, user, battleManager, i == 0);
        }

        /// <summary>
        /// Add a new layer a the side.
        /// </summary>
        private IEnumerator AddLayer(BattlerType side,
                                     MonsterInstance target,
                                     Battler user,
                                     BattleManager battleManager,
                                     bool showMessage = true)
        {
            if (LayerCount.ContainsKey(side))
            {
                LayerCount[side]++;

                if (showMessage)
                    yield return DialogManager.ShowDialogAndWait(LayerAddedKey,
                                                                 localizableModifiers: false,
                                                                 modifiers: target.GetNameOrNickName(battleManager
                                                                    .Localizer),
                                                                 switchToNextAfterSeconds: 1.5f
                                                                   / battleManager.BattleSpeed);
            }
            else
                LayerCount[side] = 1;
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="side">Side the status is setup in.</param>
        public override IEnumerator OnBattleEnded(BattlerType side)
        {
            LayerCount.Clear();
            yield break;
        }
    }
}