using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Varguiniano.YAPU.Runtime.UI.Dialogs;
using WhateverDevs.Core.Runtime.DataStructures;

namespace Varguiniano.YAPU.Runtime.Battle.Statuses.Volatile
{
    /// <summary>
    /// Base class for a volatile status that can have several layers on top of each other.
    /// </summary>
    public abstract class LayeredVolatileStatus : VolatileStatus
    {
        /// <summary>
        /// Get the localization key for this status' dialog when it ends.
        /// </summary>
        /// <returns>A string with the localization key.</returns>
        [FoldoutGroup("Localization")]
        public string LayerAddedKey;

        /// <summary>
        /// Max layers for this status.
        /// </summary>
        [FoldoutGroup("Layers")]
        [SerializeField]
        private int MaxLayers = 3;

        /// <summary>
        /// Layer count for the two sides.
        /// </summary>
        [FoldoutGroup("Layers")]
        [HideInEditorMode]
        public SerializableDictionary<Battler, int> LayerCount = new();

        /// <summary>
        /// Auto generate the localization based on the object name.
        /// </summary>
        protected override void GenerateLocalization()
        {
            base.GenerateLocalization();
            LayerAddedKey = LocalizableNameKey + "/LayerAdded";
        }

        /// <summary>
        /// Callback for when this status is added.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="extraData">Extra data this status may need.</param>
        public override IEnumerator OnAddStatus(BattleManager battleManager, Battler battler, params object[] extraData)
        {
            // Make sure dirty layers that may have been left behind are removed.
            if (LayerCount.ContainsKey(battler)) LayerCount.Remove(battler);

            yield break;
        }

        /// <summary>
        /// Add a new layer a the side.
        /// </summary>
        public IEnumerator AddLayer(Battler target, Battler user, BattleManager battleManager)
        {
            if (LayerCount.ContainsKey(target) && LayerCount[target] >= MaxLayers)
            {
                yield return DialogManager.ShowDialogAndWait("Battle/Move/NoEffect",
                                                             switchToNextAfterSeconds: 1.5f
                                                               / battleManager.BattleSpeed);

                yield break;
            }

            if (!LayerCount.ContainsKey(target))
                LayerCount[target] = 1;
            else
                LayerCount[target]++;

            yield return DialogManager.ShowDialogAndWait(LayerAddedKey,
                                                         localizableModifiers: false,
                                                         modifiers: new[]
                                                                    {
                                                                        target.GetNameOrNickName(battleManager
                                                                           .Localizer),
                                                                        LayerCount[target].ToString()
                                                                    },
                                                         switchToNextAfterSeconds: 1.5f
                                                           / battleManager.BattleSpeed);

            yield return OnLayerAdded(target, user, battleManager);
        }

        /// <summary>
        /// Called when a layer is added to the status.
        /// </summary>
        /// <param name="target">Target of the status.</param>
        /// <param name="user">Mon that added the status.</param>
        /// <param name="battleManager">Reference to the battle manager.</param>
        protected virtual IEnumerator OnLayerAdded(Battler target, Battler user, BattleManager battleManager)
        {
            yield break;
        }

        /// <summary>
        /// Callback for when this status is removed.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="playAnimation">Play the remove animation?</param>
        public override IEnumerator OnRemoveStatus(BattleManager battleManager,
                                                   Battler battler,
                                                   bool playAnimation = true)
        {
            if (!LayerCount.ContainsKey(battler)) yield break;

            int layers = LayerCount[battler];

            LayerCount.Remove(battler);

            yield return OnAllLayersRemoved(battleManager, battler, layers);
        }

        /// <summary>
        /// Called when all the layers are removed at once.
        /// </summary>
        /// <param name="battleManager">Reference to the battle manager.</param>
        /// <param name="battler">Affected battler.</param>
        /// <param name="layersRemoved">Number of layers removed.</param>
        protected virtual IEnumerator OnAllLayersRemoved(BattleManager battleManager,
                                                         Battler battler,
                                                         int layersRemoved)
        {
            yield break;
        }

        /// <summary>
        /// Called when the battler has ended.
        /// </summary>
        /// <param name="battler">Battler the status is attached to.</param>
        public override IEnumerator OnBattleEnded(Battler battler)
        {
            LayerCount.Clear();
            yield break;
        }
    }
}