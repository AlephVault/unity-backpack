﻿using UnityEngine;
using UnityEngine.UI;


namespace GameMeanMachine.Unity.BackPack
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
                        ///     ancestor can identify the label to put the selected item's caption.
                        /// </summary>
                        [RequireComponent(typeof(Text))]
                        public class BasicStandardInventoryViewSelectedItemLabel : MonoBehaviour
                        {
                            /**
                             * Updates the content of the item into its text.
                             */

                            private Text textComponent;

                            void Awake()
                            {
                                textComponent = GetComponent<Text>();
                            }

                            public void SetCaption(string caption)
                            {
                                textComponent.text = caption;
                            }
                        }
                    }
                }
            }
        }
    }
}
