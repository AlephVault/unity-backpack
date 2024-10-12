using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlephVault.Unity.BackPack
{
	using Authoring.Behaviours.Inventory.ManagementStrategies.SpatialStrategies;

    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                /// <summary>
                ///   <para>
                ///     Stacks are live instances of inventory items. They are created by them
                ///       and have their own strategies (which are related to the strategies in
                ///       the item - the only difference is that there are no spatial strategies
                ///       in the stack but just its position).
                ///   </para>
                ///   <para>
                ///     Stacks are alive: they can be used, changed, split, merged, ...
                ///   </para>
                /// </summary>
                public class Stack
                {
                    /// <summary>
                    ///   The current quantity of this stack.
                    /// </summary>
                    public int Quantity { get; private set; }
                    
                    private UsageStrategies.StackUsageStrategy[] usageStrategies;
                    private Dictionary<Type, UsageStrategies.StackUsageStrategy> usageStrategiesByType;
                    /// <summary>
                    ///   The main usage strategy of this stack. It will be related to the underlying
                    ///     item's main usage strategy, and identified by the inventory's main usage
                    ///     strategy.
                    /// </summary>
                    public UsageStrategies.StackUsageStrategy MainUsageStrategy
                    {
                        get; private set;
                    }

                    private RenderingStrategies.StackRenderingStrategy[] renderingStrategies;
                    private Dictionary<Type, RenderingStrategies.StackRenderingStrategy> renderingStrategiesByType;
                    /// <summary>
                    ///   The main rendering strategy of this stack. It will be related to the underlying
                    ///     item's main rendering strategy, and identified by the inventory's rendering
                    ///     strategy.
                    /// </summary>
                    public RenderingStrategies.StackRenderingStrategy MainRenderingStrategy
                    {
                        get; private set;
                    }
                    
                    /// <summary>
                    ///   The position this stack has on its current container. This means this position
                    ///     will be according to THAT specific container (and in particular: its spatial
                    ///     strategy), and just ONE of the underlying item's spatial strategy.
                    /// </summary>
                    public InventorySpatialManagementStrategy.QualifiedStackPosition QualifiedPosition
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   The underlying item.
                    /// </summary>
                    public Authoring.ScriptableObjects.Inventory.Items.Item Item
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   Gets a usage strategy of type <typeparamref name="T"/>.
                    /// </summary>
                    /// <typeparam name="T">The given type to check</typeparam>
                    /// <returns>An attached usage strategy of that type</returns>
                    public T GetUsageStrategy<T>() where T : UsageStrategies.StackUsageStrategy
                    {
                        return usageStrategiesByType[typeof(T)] as T;
                    }

                    /// <summary>
                    ///   Gets a rendering strategy of type <typeparamref name="T"/>.
                    /// </summary>
                    /// <typeparam name="T">The given type to check</typeparam>
                    /// <returns>An attached rendering strategy of that type</returns>
                    public T GetRenderingStrategy<T>() where T : RenderingStrategies.StackRenderingStrategy
                    {
                        return renderingStrategiesByType[typeof(T)] as T;
                    }

                    /// <summary>
                    ///   Creates a stack. You will never manually instantiate a stack but instead invoke
                    ///    <see cref="ScriptableObjects.Inventory.Items.Item.Create(object, object)"/>.
                    /// </summary>
                    /// <param name="item">The item to refer from this stack</param>
                    /// <param name="usageStrategies">Many stack usage strategies</param>
                    /// <param name="mainUsageStrategy">A main stack usage strategy among the ones in <paramref name="usageStrategies"/></param>
                    /// <param name="renderingStrategies">Many stack rendering strategies</param>
                    /// <param name="mainRenderingStrategy">A main stack rendering strategy among the ones in <paramref name="renderingStrategies"/></param>
                    public Stack(Authoring.ScriptableObjects.Inventory.Items.Item item,
                                 UsageStrategies.StackUsageStrategy[] usageStrategies,
                                 UsageStrategies.StackUsageStrategy mainUsageStrategy,
                                 RenderingStrategies.StackRenderingStrategy[] renderingStrategies,
                                 RenderingStrategies.StackRenderingStrategy mainRenderingStrategy)
                    {
                        Item = item;
                        this.usageStrategies = usageStrategies;
                        this.renderingStrategies = renderingStrategies;
                        MainUsageStrategy = mainUsageStrategy;
                        MainRenderingStrategy = mainRenderingStrategy;

                        usageStrategiesByType = new Dictionary<Type, UsageStrategies.StackUsageStrategy>();
                        renderingStrategiesByType = new Dictionary<Type, RenderingStrategies.StackRenderingStrategy>();

                        /*
                         * Initializing the stack strategies.
                         */
                        foreach (UsageStrategies.StackUsageStrategy strategy in usageStrategies)
                        {
                            usageStrategiesByType[strategy.GetType()] = strategy;
                            strategy.Initialize(this);
                        }
                        foreach (RenderingStrategies.StackRenderingStrategy strategy in renderingStrategies)
                        {
                            renderingStrategiesByType[strategy.GetType()] = strategy;
                            strategy.Initialize(this);
                        }
                    }

                    /// <summary>
                    ///   Serializes its content into a tuple (item, quantity, arbitrary data).
                    /// </summary>
                    /// <returns>A tuple representation of this stack: (item, quantity, arbitrary data)</returns>
                    public Tuple<Authoring.ScriptableObjects.Inventory.Items.Item, int, object> Dump()
                    {
                        return new Tuple<Authoring.ScriptableObjects.Inventory.Items.Item, int, object>(Item, Quantity, MainUsageStrategy.Export());
                    }

                    /**
                     * All the public methods go here.
                     */

                    /// <summary>
                    ///   Clones the stack with a different quantity, and the new stack is not attached to any inventory container.
                    /// </summary>
                    /// <returns>The cloned stack</returns>
                    public Stack Clone(int quantity)
                    {
                        UsageStrategies.StackUsageStrategy[] clonedUsageStrategies = new UsageStrategies.StackUsageStrategy[usageStrategies.Length];
                        UsageStrategies.StackUsageStrategy clonedMainUsageStrategy = null;
                        RenderingStrategies.StackRenderingStrategy[] clonedRenderingStrategies = new RenderingStrategies.StackRenderingStrategy[renderingStrategies.Length];
                        RenderingStrategies.StackRenderingStrategy clonedMainRenderingStrategy = null;
                        int index = 0;
                        foreach (UsageStrategies.StackUsageStrategy strategy in usageStrategies)
                        {
                            clonedUsageStrategies[index] = strategy.Clone();
                            if (MainUsageStrategy == strategy)
                            {
                                // We know that MainUsageStrategy will enter this condition at least once.
                                // Otherwise, we should need to call clonedMainUsageStrategy = MainUsageStrategy.Clone()
                                //   but outside the loop.
                                clonedMainUsageStrategy = clonedUsageStrategies[index];
                            }
                            index++;
                        }

                        index = 0;
                        foreach (RenderingStrategies.StackRenderingStrategy strategy in renderingStrategies)
                        {
                            clonedRenderingStrategies[index] = strategy.Clone();
                            if (MainRenderingStrategy == strategy)
                            {
                                // We know that MainRenderingStrategy will enter this condition at least once.
                                // Otherwise, we should need to call clonedMainRenderingStrategy = MainRenderingStrategy.Clone()
                                //   but outside the loop.
                                clonedMainRenderingStrategy = clonedRenderingStrategies[index];
                            }
                            index++;
                        }

                        Stack item = new Stack(
                            Item, clonedUsageStrategies, clonedMainUsageStrategy,
                            clonedRenderingStrategies, clonedMainRenderingStrategy
                        ) {
                            Quantity = quantity
                        };
                        return item;
                    }

                    /// <summary>
                    ///   Clones the stack completely, but the new stack is not attached to any inventory container.
                    /// </summary>
                    /// <returns>The cloned stack</returns>
                    public Stack Clone()
                    {
                        return Clone(Quantity);
                    }
                    
                    /// <summary>
                    ///   Checks whether this stack has an allowed (in-constraints) nonzero
                    ///     quantity. Stacks not being able to satisfy this condition will not
                    ///     be added to an inventory.
                    /// </summary>
                    /// <returns>Whether the quantity is allowed and non-zero</returns>
                    public bool IsAllowedNonZeroQuantity()
                    {
                        return Quantity <= Item.MaxStackQuantity && !IsEmpty();
                    }

                    /// <summary>
                    ///   Takes some quantity from the current stack, creating another stack.
                    ///   If quantity is null, the entire stack is taken.
                    /// </summary>
                    /// <param name="quantity">Quantity to take, or null to take everything</param>
                    /// <param name="disallowEmpty">Whether we allow, or not, taking the whole stack</param>
                    /// <returns>The new (taken) stack. The old one is reduced in size</returns>
                    /// <remarks>
                    ///   It will return null if either the quantities are invalid or <paramref name="disallowEmpty"/>
                    ///     is true and the quantity to be taken was null or all
                    /// </remarks>
                    public Stack Take(int? quantity, bool disallowEmpty)
                    {
                        if (quantity == null)
                        {
                            // We are taking all the quantity.
                            // BUT if disallowEmpty is null, we just leave.
                            if (disallowEmpty) return null;
                            quantity = Quantity;
                        }

                        if ((!disallowEmpty || quantity.Value != Quantity) && ChangeQuantityBy((int)-quantity.Value))
                        {
                            return Clone(quantity.Value);
                        }
                        return null;
                    }

                    /**
                     * Tries to merge a stack into another.
                     * 
                     * Please consider the following notes: This method does not affect the source stack
                     *   but instead affects the target stack. This means: without the cares of manually
                     *   handling the source stack later, you could end with twice the expected amount
                     *   somewhere.
                     * 
                     * The result of the merge may be:
                     * 1. Denied: This may occur because the underlying item of the stacks is not the
                     *    same in both cases, the usage strategies are not mergeable (either by their
                     *    nature or by their circumstance), the quantity of either stacks is invalid
                     *    (i.e. non-positive) or the quantity of the destination stack is full.
                     * 2. Partial: The merge was successful but not with the whole quantity. This means
                     *    that the destination stack filled up and 
                     * 
                     * As an output parameter, you get the quantity left on the source stack. You should
                     *   explicitly set such value in the source stack by calling the following method:
                     *   -> source.ChangeQuantityTo(quantityLeft)
                     * However this will vary depending on your needs.
                     */
                    public enum MergeResult { Denied, Partial, Total }

                    /// <summary>
                    ///   <para>
                    ///     Tries to merge a stack into another.
                    ///   </para>
                    ///   <para>
                    ///     Please consider the following notes: This method does not affect the source
                    ///     stack but instead affects the target stack. This means: without the cares of
                    ///     manually handling the source stack later, one could end with twice the expected
                    ///     amount somewhere.
                    ///   </para>
                    ///   <para>
                    ///     The result of the merge may be:
                    ///     <list type="number">
                    ///       <item>
                    ///         <term>Denied</term>
                    ///         <description>
                    ///           The merge was denied. Most likely because items do not match, usage
                    ///             strategies cannot be merged, quantities are invalid, or destination
                    ///             quantity is full.
                    ///         </description>
                    ///       </item>
                    ///       <item>
                    ///         <term>Partial</term>
                    ///         <description>
                    ///           The merge was successful but not with the whole quantity. This means
                    ///           that the destination quantity filled up and there is still some
                    ///           quantity as difference, which will remain on the source stack.
                    ///         </description>
                    ///       </item>
                    ///       <item>
                    ///         <term>Total</term>
                    ///         <description>
                    ///           The source was completely merged in the destination.
                    ///         </description>
                    ///       </item>
                    ///     </list>
                    ///   </para>
                    ///   <para>
                    ///     As an output parameter, returns the quantity left on the source stack. The
                    ///     caller should explicitly set such value in the source stack by calling the
                    ///     following method: <c>source.ChangeQuantityTo(quantityLeft)</c>.
                    ///     However, this will vary depending on the needs.
                    ///   </para>
                    /// </summary>
                    /// <param name="source">The source stack to merge in this one</param>
                    /// <param name="quantityLeft">The remaining quantity that was not merged</param>
                    /// <returns>An enumerated value according to the notes</returns>
                    public MergeResult Merge(Stack source, out int quantityLeft)
                    {
                        // preset to null so we can leave control safely.
                        quantityLeft = 0;

                        // this one would tell the quantity effectively added to, and final in, the stack.
                        int quantityAdded;
                        int finalQuantity;

                        if (Item != source.Item || IsFull() || !IsAllowedNonZeroQuantity() || !source.IsAllowedNonZeroQuantity())
                        {
                            return MergeResult.Denied;
                        }

                        // We test saturation to know which quantities to add
                        bool saturates = WillOverflow(Quantity, out finalQuantity, out quantityAdded, out quantityLeft);

                        /*
                         * This will happen now:
                         * 1. The spatial strategy will not be affected.
                         * 2. The rendering strategies will not be affected.
                         * 3. The usage strategies will behave differently:
                         */

                        // Now we compute the interpolations for each UsageStrategy (stacks will have them
                        //   in the same order) by manually zipping everything.
                        int index = 0;
                        Action[] interpolators = new Action[usageStrategies.Length];
                        foreach(UsageStrategies.StackUsageStrategy usageStrategy in usageStrategies)
                        {
                            Action interpolator = usageStrategy.Interpolate(
                                source.usageStrategies[index], Quantity, quantityAdded
                            );
                            if (interpolator == null)
                            {
                                // If at least an interpolator fails, we abort everything.
                                return MergeResult.Denied;
                            }
                            interpolators[index++] = interpolator;
                        }

                        // Now we run all the interpolators.
                        foreach(Action interpolator in interpolators)
                        {
                            interpolator();
                        }

                        // We reached this point because all the interpolators have been found.
                        // If you coded the saturation method appropriately, this will work.
                        if (finalQuantity > 0) ChangeQuantityTo(finalQuantity);

                        // We are ok with this.
                        return saturates ? MergeResult.Partial : MergeResult.Total;
                    }

                    /// <summary>
                    ///   Tells whether this stack "equals" the other stack by comparing their
                    ///     usage strategies. See <see cref="UsageStrategies.StackUsageStrategy.Equals(UsageStrategies.StackUsageStrategy)"/>
                    ///     for more details.
                    /// </summary>
                    /// <param name="otherStack">The stack to compare</param>
                    /// <returns>Whether both stacks equal or not (in terms of usage strategies)</returns>
                    public bool Equals(Stack otherStack)
                    {
                        if (Item == otherStack.Item)
                        {
                            int index = 0;
                            foreach(UsageStrategies.StackUsageStrategy usageStrategy in usageStrategies)
                            {
                                if (!usageStrategy.Equals(otherStack.usageStrategies[index++]))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                        return false;
                    }
                    
                    /// <summary>
                    ///   Tells whether the stack is full.
                    /// </summary>
                    /// <returns>Whether the stack is full</returns>
                    public bool IsFull()
                    {
                        return Item.MaxStackQuantity != 0 && Quantity == Item.MaxStackQuantity;
                    }

                    /// <summary>
                    ///   Tells whether the stack is empty.
                    /// </summary>
                    /// <returns>Whether the stack is empty</returns>
                    public bool IsEmpty()
                    {
                        return Quantity == 0;
                    }

                    /// <summary>
                    ///   Modifies the quantity of the stack.
                    /// </summary>
                    /// <param name="quantity">The quantity delta to apply</param>
                    /// <returns>Whether the change could be performed</returns>
                    public bool ChangeQuantityBy(int quantity)
                    {
                        return ChangeQuantityTo(Quantity + quantity);
                    }

                    /// <summary>
                    ///   Tells whether the stack would overflow its allowed maximum quantity when trying to add a
                    ///   certain quantity.
                    /// </summary>
                    /// <param name="quantity">The quantity to add</param>
                    /// <param name="finalQuantity">The quantity that would be final to the stack</param>
                    /// <param name="quantityAdded">The quantity that would be effectively added from the given quantity</param>
                    /// <param name="quantityLeft">The quantity that would not be added from the given quantity</param>
                    /// <returns>Whether the quantity would overflow the maximum</returns>
                    public bool WillOverflow(int quantity, out  int finalQuantity, out int quantityAdded, out int quantityLeft)
                    {
                        int maxQuantity = Item.MaxStackQuantity;
                        if (Item.MaxStackQuantity == 0)
                        {
                            finalQuantity = Quantity + quantity;
                            quantityAdded = quantity;
                            quantityLeft = 0;
                            return false;
                        }
                        int potentialQuantity = Quantity + quantity;
                        if (potentialQuantity > Item.MaxStackQuantity)
                        {
                            finalQuantity = Item.MaxStackQuantity;
                            quantityAdded = maxQuantity - Quantity;
                            quantityLeft = potentialQuantity - maxQuantity;
                            return true;
                        }
                        finalQuantity = potentialQuantity;
                        quantityAdded = quantity;
                        quantityLeft = 0;
                        return false;
                    }

                    /// <summary>
                    ///   Ensures the stack's quantity is set to the maximum. See <see cref="QuantifyingStrategies.StackQuantifyingStrategy.Saturate"/>
                    ///     for more details.
                    /// </summary>
                    /// <returns>Whether the saturation could be done</returns>
                    public bool Saturate()
                    {
                        if (Item.MaxStackQuantity == 0) return false;
                        Quantity = Item.MaxStackQuantity;
                        return true;
                    }

                    /// <summary>
                    ///   Changes the stack's quantity to a specific value.
                    /// </summary>
                    /// <param name="quantity">The new quantity to set</param>
                    /// <returns>Whether the quantity could be set</returns>
                    public bool ChangeQuantityTo(int quantity)
                    {
                        if (quantity < 0) return false;

                        if (Item.MaxStackQuantity == 0 || quantity <= Item.MaxStackQuantity)
                        {
                            Quantity = quantity;
                            return true;
                        }
                        return false;
                    }
                    
                    /// <summary>
                    ///   Tells whether this stack has an allowed quantity or not.
                    /// </summary>
                    public bool HasAllowedQuantity()
                    {
                        return Quantity <= Item.MaxStackQuantity;
                    }
                }
            }
        }
    }
}
