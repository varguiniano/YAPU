using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Varguiniano.YAPU.Runtime.Input;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DataStructures;
using Zenject;

namespace Varguiniano.YAPU.Runtime.UI.Input
{
    /// <summary>
    /// Behaviour to display an icon of an input depending on the active controller.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InputIcon : WhateverBehaviour<InputIcon>
    {
        /// <summary>
        /// List of icons to display depending on the input.
        /// </summary>
        [SerializeField]
        private SerializableDictionary<string, Sprite> Icons;

        /// <summary>
        /// Index to use in case it's not supported.
        /// </summary>
        [FormerlySerializedAs("DefaultIndex")]
        [SerializeField]
        private string Default = "Xbox Controller";

        /// <summary>
        /// Reference to the input manager.
        /// </summary>
        [Inject]
        private IInputManager inputManager;

        /// <summary>
        /// Subscribe to the input manager.
        /// </summary>
        private void OnEnable()
        {
            OnDeviceUpdated(inputManager.GetCurrentInputDevice());

            inputManager.SubscribeToInputDeviceChanged(OnDeviceUpdated);
        }

        /// <summary>
        /// Unsubscribe from the input manager.
        /// </summary>
        private void OnDisable() => inputManager.UnsubscribeToInputDeviceChanged(OnDeviceUpdated);

        /// <summary>
        /// Update the icon with the given input device.
        /// </summary>
        /// <param name="newDevice">Name of the input device.</param>
        private void OnDeviceUpdated(string newDevice)
        {
            if (!Icons.ContainsKey(newDevice))
            {
                Logger.Warn("Unsupported input device: " + newDevice + ", using default: " + Default + ".");
                newDevice = Default;
            }

            GetCachedComponent<Image>().sprite = Icons[newDevice];
        }
    }
}