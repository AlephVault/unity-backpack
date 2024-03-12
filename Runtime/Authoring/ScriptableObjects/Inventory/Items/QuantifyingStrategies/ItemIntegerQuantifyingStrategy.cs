﻿using UnityEngine;
using AlephVault.Unity.Support.Utils;

namespace AlephVault.Unity.BackPack
{
	using Types.Inventory.Stacks.QuantifyingStrategies;

    namespace Authoring
    {
        namespace ScriptableObjects
        {
            namespace Inventory
            {
                namespace Items
                {
                    namespace QuantifyingStrategies
                    {
                        /// <summary>
                        ///   <para>
                        ///     Integer strategies are the most common. They make
                        ///       stacks in integer quantities.
                        ///   </para>
                        ///   <para>
                        ///     Quite often a maximum amount is specified to
                        ///       limit the stacks.
                        ///   </para>
                        /// </summary>
                        [CreateAssetMenu(fileName = "NewInventoryItemIntegerQuantifyingStrategy", menuName = "Aleph Vault/WindRose/Inventory/Item Strategies/Quantifying/Integer-Stacked", order = 101)]
                        public class ItemIntegerQuantifyingStrategy : ItemQuantifyingStrategy
                        {
                            /// <summary>
                            ///   The maximum amount to allow in stacks.
                            /// </summary>
                            [SerializeField]
                            private int max = 0;

                            /// <summary>
                            ///   See <see cref="max"/>.
                            /// </summary>
                            public int Max
                            {
                                get { return max; }
                            }

                            private void Awake()
                            {
                                max = Values.Max(0, max);
                            }

                            /// <summary>
                            ///   Instantiates an integer quantifying stack strategy.
                            /// </summary>
                            /// <returns>An integer quantifying stack strategy</returns>
                            public override StackQuantifyingStrategy CreateStackStrategy(object quantity)
                            {
                                return new StackIntegerQuantifyingStrategy(this, quantity);
                            }
                        }
                    }
                }
            }
        }
    }
}
