using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace Varguiniano.YAPU.Runtime.UI.Profile
{
    /// <summary>
    /// Text that shows the versions on the profile screen.
    /// </summary>
    [RequireComponent(typeof(LocalizedTextMeshPro))]
    public class VersionText : HidableUiElement<VersionText>
    {
        /// <summary>
        /// YAPU version.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private YAPUInstaller.Runtime.Version YAPUVersion;

        /// <summary>
        /// YAPU Assets version.
        /// </summary>
        [FoldoutGroup("References")]
        [SerializeField]
        private YAPUInstaller.Runtime.Version YAPUAssetsVersion;

        /// <summary>
        /// Application version.
        /// </summary>
        [Inject]
        private Version version;

        /// <summary>
        /// Set the value.
        /// </summary>
        private void OnEnable() =>
            GetCachedComponent<LocalizedTextMeshPro>()
               .SetValue("Menu/Profile/Version",
                         false,
                         version.FullVersion,
                         YAPUVersion.FullVersion,
                         YAPUAssetsVersion.FullVersion);
    }
}