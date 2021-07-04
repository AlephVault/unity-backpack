﻿using System;
using UnityEngine;

namespace GameMeanMachine.Unity.BackPack
{
    namespace Authoring
    {
        namespace ScriptableObjects
        {
            namespace Inventory
            {
                namespace Items
                {
                    namespace UsageStrategies
                    {
                        using Types.Inventory.Stacks.UsageStrategies;

                        /// <summary>
                        ///   This strategy is for items that cannot be used.This means:
                        ///     items that, when you attempt to use them, do nothing.One
                        ///     useful example is critical objects.
                        /// </summary>
                        [CreateAssetMenu(fileName = "NewInventoryItemNullUsageStrategy", menuName = "Wind Rose/Inventory/Item Strategies/Usage/Null Usage (e.g. tokens, critical objects)", order = 101)]
                        public class ItemNullUsageStrategy : ItemUsageStrategy
                        {
                            /// <summary>
                            ///   Instantiates a null usage stack strategy.
                            /// </summary>
                            /// <returns>A null usage stack strategy</returns>
                            public override StackUsageStrategy CreateStackStrategy()
                            {
                                return new StackNullUsageStrategy(this);
                            }
                        }
                    }
                }
            }
        }
    }
}
