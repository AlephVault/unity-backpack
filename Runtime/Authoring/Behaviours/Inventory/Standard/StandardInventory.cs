﻿using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.BackPack
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Inventory
            {
                namespace Standard
                {
                    using Types.Inventory.Stacks;
                    using ManagementStrategies.SpatialStrategies;
                    using System;
                    using System.Linq;

                    // using World.Layers.Drop;

                    /// <summary>
                    ///   <para>
                    ///     A standard inventory has 3 characteristics: single container, 1D indexed
                    ///       elements inside the container, each represented by an icon and its
                    ///       caption.
                    ///   </para>
                    ///   <para>
                    ///     They are tightly related to <see cref="InventoryManagementStrategyHolder"/>
                    ///       and <see cref="InventoryStandardRenderingManagementStrategy"/>.
                    ///   </para>
                    /// </summary>
                    [RequireComponent(typeof(InventorySinglePositioningManagementStrategy))]
                    [RequireComponent(typeof(Inventory1DIndexedSpatialManagementStrategy))]
                    [RequireComponent(typeof(InventoryManagementStrategyHolder))]
                    [RequireComponent(typeof(InventoryStandardRenderingManagementStrategy))]
                    public class StandardInventory : MonoBehaviour
                    {
                        private InventoryManagementStrategyHolder inventoryHolder;

                        /// <summary>
                        ///   The underlying spatial strategy, which is an indexed one
                        ///     and can be either finite or infinite.
                        /// </summary>
                        public Inventory1DIndexedSpatialManagementStrategy SpatialStrategy { get; private set; }

                        /// <summary>
                        ///   The underlying rendering strategy, which is a standard
                        ///     rendering strategy with its broadcaster for views.
                        /// </summary>
                        public InventoryStandardRenderingManagementStrategy RenderingStrategy { get; private set; }

                        /**
                         * Awake/Start pre-register the listeners (if they are set).
                         */

                        void Awake()
                        {
                            inventoryHolder = GetComponent<InventoryManagementStrategyHolder>();
                            SpatialStrategy = GetComponent<Inventory1DIndexedSpatialManagementStrategy>();
                            RenderingStrategy = GetComponent<InventoryStandardRenderingManagementStrategy>();
                        }

                        /**
                         * Proxy calls to inventory holder methods.
                         */

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.StackPairs(object, bool)"/>.
                        /// </summary>
                        public IEnumerable<Tuple<int, Stack>> StackPairs(bool reverse = false)
                        {
                            return from tuple in inventoryHolder.StackPairs(Position.Instance, reverse) select new Tuple<int, Stack>((int)tuple.Item1, tuple.Item2);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Find(object, object)"/>.
                        /// </summary>
                        public Stack Find(int position)
                        {
                            return inventoryHolder.Find(Position.Instance, position);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindAll(object, Func{Tuple{object, Stack}, bool}, bool)"/>.
                        /// </summary>
                        public IEnumerable<Stack> FindAll(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                        {
                            return inventoryHolder.FindAll(Position.Instance, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.Item1, tuple.Item2)); }, reverse);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindAll(object, ScriptableObjects.Inventory.Items.Item, bool)"/>.
                        /// </summary>
                        public IEnumerable<Stack> FindAll(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                        {
                            return inventoryHolder.FindAll(Position.Instance, item, reverse);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.First(object)"/>.
                        /// </summary>
                        public Stack First()
                        {
                            return inventoryHolder.First(Position.Instance);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Last(object)"/>.
                        /// </summary>
                        public Stack Last()
                        {
                            return inventoryHolder.Last(Position.Instance);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindOne(object, Func{Tuple{object, Stack}, bool}, bool)"/>.
                        /// </summary>
                        public Stack FindOne(Func<Tuple<int, Stack>, bool> predicate, bool reverse = false)
                        {
                            return inventoryHolder.FindOne(Position.Instance, delegate (Tuple<object, Stack> tuple) { return predicate(new Tuple<int, Stack>((int)tuple.Item1, tuple.Item2)); }, reverse);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.FindOne(object, ScriptableObjects.Inventory.Items.Item, bool)"/>.
                        /// </summary>
                        public Stack FindOne(ScriptableObjects.Inventory.Items.Item item, bool reverse = false)
                        {
                            return inventoryHolder.FindOne(Position.Instance, item, reverse);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Put(object, object, Stack, out object, bool?)"/>.
                        /// </summary>
                        public bool Put(int? position, Stack stack, out int? finalPosition, bool? optimalPutOnNullPosition = null)
                        {
                            bool result = inventoryHolder.Put(Position.Instance, position, stack, out var finalOPosition, optimalPutOnNullPosition);
                            finalPosition = (int?)finalOPosition;
                            return result;
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Remove(object, object)"/>.
                        /// </summary>
                        public bool Remove(int position)
                        {
                            return inventoryHolder.Remove(Position.Instance, position);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Merge(object, object, object)"/>.
                        /// </summary>
                        public bool Merge(int? destinationPosition, int sourcePosition)
                        {
                            return inventoryHolder.Merge(Position.Instance, destinationPosition, sourcePosition);
                        }

                        // The other version of `Merge` has little use here.

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Take(object, object, object, bool)"/>.
                        /// </summary>
                        public Stack Take(int position, int? quantity, bool disallowEmpty)
                        {
                            return inventoryHolder.Take(Position.Instance, position, quantity, disallowEmpty);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Split(object, object, object, object, object, out object)"/>.
                        /// </summary>
                        public bool Split(int sourcePosition, int quantity,
                                          int newPosition, int? finalNewPosition)
                        {
                            object finalNewOPosition;
                            bool result = inventoryHolder.Split(Position.Instance, sourcePosition, quantity,
                                                                Position.Instance, newPosition, out finalNewOPosition);
                            finalNewPosition = finalNewOPosition == null ? null : (int?)finalNewOPosition;
                            return result;
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Use(object, object)"/>.
                        /// </summary>
                        public bool Use(int sourcePosition)
                        {
                            return inventoryHolder.Use(Position.Instance, sourcePosition);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Use(object, object, object)"/>.
                        /// </summary>
                        public bool Use(int sourcePosition, object argument)
                        {
                            return inventoryHolder.Use(Position.Instance, sourcePosition, argument);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Clear"/>.
                        /// </summary>
                        public void Clear()
                        {
                            inventoryHolder.Clear();
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Blink(object)"/>.
                        /// </summary>
                        public void Blink()
                        {
                            inventoryHolder.Blink(Position.Instance);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Blink(object, object)"/>.
                        /// </summary>
                        public void Blink(int position)
                        {
                            inventoryHolder.Blink(Position.Instance, position);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Import(Types.Inventory.SerializedInventory)"/>.
                        /// </summary>
                        public void Import(BackPack.Types.Inventory.SerializedInventory serializedInventory)
                        {
                            inventoryHolder.Import(serializedInventory);
                        }

                        /// <summary>
                        ///   Convenience method. See <see cref="InventoryManagementStrategyHolder.Export"/>.
                        /// </summary>
                        public BackPack.Types.Inventory.SerializedInventory Export()
                        {
                            return inventoryHolder.Export();
                        }
                    }
                }
            }
        }
    }
}
