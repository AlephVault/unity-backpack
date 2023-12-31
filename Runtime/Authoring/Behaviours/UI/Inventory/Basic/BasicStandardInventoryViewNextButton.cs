﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlephVault.Unity.BackPack
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace UI
            {
                namespace Inventory
                {
                    namespace Basic
                    {
                        /// <summary>
                        ///   This is a marker behaviour so the <see cref="BasicStandardInventoryView" />
                        ///     ancestor can identify it and add the corresponding click event to move one
                        ///     page forward in the view.
                        /// </summary>
                        [RequireComponent(typeof(Button))]
                        public class BasicStandardInventoryViewNextButton : MonoBehaviour { }
                    }
                }
            }
        }
    }
}
