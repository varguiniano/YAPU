using DG.Tweening;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace Varguiniano.YAPU.Runtime.UI.Dex
{
    /// <summary>
    /// Controller for a Dex info panel that can display different types of info.
    /// </summary>
    public class DexGenericInfoPanel : HidableUiElement<DexGenericInfoPanel>
    {
        /// <summary>
        /// Reference to the panel title.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Title;

        /// <summary>
        /// Reference to the panel description.
        /// </summary>
        [SerializeField]
        private LocalizedTextMeshPro Description;

        /// <summary>
        /// Reference to the panel's open position.
        /// </summary>
        [SerializeField]
        private Transform OpenPosition;

        /// <summary>
        /// Reference to the panel's closed position.
        /// </summary>
        [SerializeField]
        private Transform ClosedPosition;

        /// <summary>
        /// Reference to this object's transform.
        /// </summary>
        private Transform Transform
        {
            get
            {
                if (transformReference == null) transformReference = transform;
                return transformReference;
            }
        }

        /// <summary>
        /// Backfield for Transform.
        /// </summary>
        private Transform transformReference;

        /// <summary>
        /// Open and display the panel.
        /// </summary>
        public void Open()
        {
            Show();

            Transform.DOMove(OpenPosition.position, .25f).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// Close the panel.
        /// </summary>
        public void Close() =>
            Transform.DOMove(ClosedPosition.position, .25f).SetEase(Ease.InBack).OnComplete(() => Show(false));

        /// <summary>
        /// Set the texts to display.
        /// </summary>
        /// <param name="localizableTitle">Localizable title key.</param>
        /// <param name="localizableDescription">Localizable title description.</param>
        public void SetTexts(string localizableTitle, string localizableDescription)
        {
            Title.SetValue(localizableTitle);
            Description.SetValue(localizableDescription);
        }
    }
}