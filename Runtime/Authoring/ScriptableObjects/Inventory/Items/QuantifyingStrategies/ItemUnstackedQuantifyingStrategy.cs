﻿using UnityEngine;

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
                        ///   Unstacked strategies do not make a stack of items.
                        ///   This actually means: they only make stacks of ONE item.
                        /// </summary>
                        [CreateAssetMenu(fileName = "NewInventoryItemUnstackedQuantifyingStrategy", menuName = "Aleph Vault/BackPack/Inventory/Item Strategies/Quantifying/Unstacked", order = 101)]
                        public class ItemUnstackedQuantifyingStrategy : ItemQuantifyingStrategy
                        {
                            /// <summary>
                            ///   Instantiates an unstacked quantifying stack strategy.
                            /// </summary>
                            /// <returns>An unstacked quantifying stack strategy</returns>
                            public override StackQuantifyingStrategy CreateStackStrategy(object quantity)
                            {
                                return new StackUnstackedQuantifyingStrategy(this);
                            }
                        }
                    }
                }
            }
        }
    }
}
