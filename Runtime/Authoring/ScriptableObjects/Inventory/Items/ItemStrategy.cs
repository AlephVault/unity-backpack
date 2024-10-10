using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlephVault.Unity.BackPack
{
    namespace Authoring
    {
        namespace ScriptableObjects
        {
            namespace Inventory
            {
                namespace Items
                {
                    /// <summary>
                    ///   Item strategies are initialized against their item
                    ///     and will instantiate stack strategies, which will
                    ///     ba called from the item.
                    /// </summary>
                    /// <typeparam name="T">
                    ///   Related stack strategy type for this item strategy type
                    /// </typeparam>
                    public abstract class ItemStrategy<T> : ScriptableObject where T : class
                    {
                    }
                }
            }
        }
    }
}
