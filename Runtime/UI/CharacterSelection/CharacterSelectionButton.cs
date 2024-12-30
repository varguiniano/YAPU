using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Characters;

namespace Varguiniano.YAPU.Runtime.UI.CharacterSelection
{
    /// <summary>
    /// Controller for a button in the character selection.
    /// </summary>
    public class CharacterSelectionButton : MenuItem
    {
        /// <summary>
        /// Character that this button selects.
        /// </summary>
        [OnValueChanged(nameof(OnCharacterSet))]
        public CharacterData Character;

        /// <summary>
        /// Reference to the character sprite.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private Image Sprite;

        /// <summary>
        /// Cached reference to the attached transform.
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
        /// Set the button as nothing selected.
        /// </summary>
        protected override void OnEnable()
        {
            OnCharacterSet();
            OnDeselect();
            base.OnEnable();
        }

        /// <summary>
        /// Called on selected.
        /// </summary>
        public override void OnSelect()
        {
            base.OnSelect();

            Transform.DOKill();
            Transform.DOScale(new Vector3(1.1f, 1.1f, 1), .1f);

            Sprite.DOKill();
            Sprite.DOColor(Color.white, .1f);
        }

        /// <summary>
        /// Called on deselected.
        /// </summary>
        public override void OnDeselect()
        {
            base.OnDeselect();

            Transform.DOKill();
            Transform.DOScale(Vector3.one, .1f);

            Sprite.DOKill();
            Sprite.DOColor(Color.gray, .1f);
        }

        /// <summary>
        /// Called whe the character is set.
        /// </summary>
        [Button("Refresh")]
        private void OnCharacterSet()
        {
            if (Character == null) return;

            Sprite.sprite = Character.CharacterType.FrontSprite;
        }
    }
}