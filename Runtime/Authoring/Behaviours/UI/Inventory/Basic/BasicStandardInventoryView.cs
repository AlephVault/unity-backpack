﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameMeanMachine.Unity.BackPack
{
    namespace Authoring
    {
        using ScriptableObjects.Inventory.Items;
        using ScriptableObjects.Inventory.Items.RenderingStrategies;

        namespace Behaviours
        {
            namespace UI
            {
                namespace Inventory
                {
                    namespace Basic
                    {
                        using AlephVault.Unity.Layout.Utils;

                        /// <summary>
                        ///   Adds the notion of an element being selected in the view. The selection
                        ///     will exist in only one page, only one slot, and will be reflected
                        ///     appropriately since child elements in the hierarchy now also account
                        ///     for the chance of being selected, and now also a label will exist
                        ///     telling the name/caption of the selected item.
                        /// </summary>
                        [RequireComponent(typeof(Image))]
                        public class BasicStandardInventoryView : StandardInventoryView
                        {
                            /**
                             * Instances of this class will have children objects.
                             * 
                             * 1. A Back button.
                             * 2. A Next button.
                             * 3. Several (6, 8 or 10) SimpleInventoryViewItem objects.
                             * 4. A label telling the current page number and max number.
                             * 5. A label telling the currently selected item.
                             * 
                             * This element is also the UI selecting the item and dropping
                             *   the item (right now, the entire stack by Drop(position)).
                             * 
                             * Methods:
                             * - Select(position) : If the position is among the selected
                             *     items, mark the item as the selected one and refresh the
                             *     whole inventory view.
                             * - Unselect() : Remove the index of selected one, and refresh
                             *     the whole inventory view.
                             * - Drop() : Invoke Drop(selected) on the SimpleInventory, if there
                             *     is a selected item. The inventory WILL refresh.
                             */

                            private BasicStandardInventoryViewPageLabel pageLabel;
                            private BasicStandardInventoryViewSelectedItemLabel selectedItemLabel;
                            public int? SelectedPosition { get; private set; }

                            protected override void Awake()
                            {
                                base.Awake();
                                pageLabel = Behaviours.RequireComponentInChildren<BasicStandardInventoryViewPageLabel>(this);
                                selectedItemLabel = Behaviours.RequireComponentInChildren<BasicStandardInventoryViewSelectedItemLabel>(this);
                                Behaviours.RequireComponentInChildren<BasicStandardInventoryViewNextButton>(this).GetComponent<Button>().onClick.AddListener(delegate () { Next(); });
                                Behaviours.RequireComponentInChildren<BasicStandardInventoryViewPrevButton>(this).GetComponent<Button>().onClick.AddListener(delegate () { Prev(); });
                            }

                            /// <summary>
                            ///   Tries to change the selection of an element in the view.
                            /// </summary>
                            /// <param name="position">The position to change the selection to</param>
                            public void Select(int position)
                            {
                                Tuple<Item, object> element;
                                if (elements.TryGetValue(position, out element))
                                {
                                    if (position == SelectedPosition) return;

                                    int? positionToUnselect = SelectedPosition;
                                    SelectedPosition = position;
                                    // Go to that page (useful if automatically selected)
                                    Go(PageFor(position));
                                    // Force refresh on general components as well
                                    AfterRefresh();
                                }
                            }

                            /// <summary>
                            ///   Tries to remove the selection of an element in the view.
                            /// </summary>
                            public void Unselect()
                            {
                                if (SelectedPosition != null)
                                {
                                    int positionToUnselect = SelectedPosition.Value;
                                    SelectedPosition = null;
                                    // Force refresh on general components as well
                                    AfterRefresh();
                                }
                            }

                            /**
                             * This method is called on refresh (when you force to update everything).
                             */
                            protected override void AfterRefresh()
                            {
                                pageLabel.SetPaginationLabel(Page, MaxPage());
                                // display the selection square over the appropriate slot, if the page is matched.
                                if (SelectedPosition != null)
                                {
                                    ItemIconTextRenderingStrategy strategy = (ItemIconTextRenderingStrategy)(elements[SelectedPosition.Value].Item1.MainRenderingStrategy);
                                    selectedItemLabel.SetCaption(SelectedPosition != null ? strategy.Caption : "");
                                    int slot = SlotFor(SelectedPosition.Value);
                                    for (int iSlot = 0; iSlot < items.Length; iSlot++)
                                    {
                                        ((BasicStandardInventoryViewItem)items[iSlot]).SetSelection(iSlot == slot);
                                    }
                                }
                                else
                                {
                                    for (int iSlot = 0; iSlot < items.Length; iSlot++)
                                    {
                                        ((BasicStandardInventoryViewItem)items[iSlot]).SetSelection(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
