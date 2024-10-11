# Unity BackPack

This is the BackPack project, which allows the developer to create inventory rulesets and models, and also some default UI implementation.

# Install

This package is not available in any UPM server. You must install it in your project like this:

1. In Unity, with your project open, open the Package Manager.
2. Either refer this GitHub project: https://github.com/AlephVault/unity-backpack.git or clone it locally and refer it from disk.
3. Also, the following packages are dependencies you need to install accordingly (in the same way and also ensuring all the recursive dependencies are satisfied):

     - https://github.com/AlephVault/unity-support.git

# Usage

The main purpose of this package is to implement features related to backpack / inventory maintaining. This is done in two aspects:

1. The true logic and inventory objects.
2. The visual aspects / UI of the objects.

While this package does not implement MMORPG features themselves, having it separate as logic / UI makes it easier for MMORPG games.

So both feature groups will be discussed in this document.

## Assets: Items and registries

The first thing for a game with inventory is to actually define the objects that can be held in the inventory, and also a way to register those objects for their access.

So, to start, there's a concept of `Item` and `ItemRegistry` that will be addressed here.

### Assets: Inventory items

Items are the core of this package, but _they're actually made of several components_ since an item involves several aspects:

1. How is it _positioned_ inside an inventory.
2. How is it _displayed_ to the end user.
3. How is it _quantified_: most of them have integer quantities (e.g. potions), and some of them are, instead, non-stacked / one-only (e.g. a sword).
4. And finally but not least: what's their usage logic.

Items are game assets, and they're created by clicking the following menu option:

```
Assets/Create/Aleph Vault/BackPack/Inventory/Item
```

This action creates a new item in the current project explorer's folder. This item is an object with the following properties and methods:

Serializable / inspector properties are:

1. `Registry`: Stands for an `ItemRegistry` (described in this section, later) assigned to this item to have it registered.
2. `Key`: Stands for a non-zero `uint` assigned s key. It must be unique among items assigned to the sme registry for it to be registered.
3. `Quantifying Strategy`: Stands for an instance of `AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.QuantifyingStrategies.ItemQuantifyingStrategy`.
   Strategies will be described later. This one in particular is related to how the item is quantified. It is mandatory. Typically, there's no need to create custom child
   classes of quantifying strategies, since most games use numeric amounts or non-stackable objects.
4. `Spatial Strategies`: Stands for an array of instances of `AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.SpatialStrategies.ItemSpatialStrategy`.
   Strategies will be described later. These in particular are related to how the item is positioned. It is mandatory to have at least one spatial strategy, but only one
   instance per spatial strategy class is allowed. Having different spatial strategies will allow stacks of this item to be placed inside more types of inventories.
5. `Usage Strategies`: Stands for an array of instances of `AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.UsageStrategies.ItemUsageStrategy`.
   Strategies will be described later. These instances are interrelated to each one in a directed acyclic graph. It is mandatory to have at least one usage strategy, and
   it is also mandatory to also add their dependencies (which means: usage strategies might depend on other usage strategy instances that must, in that case, be added also
   to this array).
6. `Main Usage Strategy`: Stands for a particular usage strategy deemed as the main one. The element selected here must belong to the `Usage Strategies` list.
7. `Rendering Strategies`: Stands for an array of instances of `AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.RenderingStrategies.ItemRenderingStrategy`.
   Strategies will be described later. These instances provide data that will be properly parsed and used to render the in-inventory objects properly. It is mandatory to have
   at least one rendering strategy and also, similar to the usage strategies, satisfying the dependencies by adding all of them in the same array.
8. `Main Rendering Strategy`: Stands for a particular rendering strategy deemed as the main one. The element selected here must belong to the `Rendering Strategies` list.

Also, public properties and methods are:

1. `public bool Registered { get; }`: Tells that this item is registered in a registry. For this, it must have a Registry assigned to it and a non-zero key.
2. `public ItemRegistry Registry { get; }`: Tells the assigned Registry to this item.
3. `public uint Key { get; }`:  Returns the assigned key.
4. `public AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.QuantifyingStrategies.ItemQuantifyingStrategy QuantifyingStrategy`: Returns the assigned quantifying strategy.
5. `public T GetSpatialStrategy<T>() where T : AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.SpatialStrategies.ItemSpatialStrategy`: Returns a spatial strategy instance by its class. Returns `null` if not found.
6. `public AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.SpatialStrategies.ItemSpatialStrategy GetSpatialStrategy(Type type)`: Returns a spatial strategy instance but by passing the type as value.
7. `public T GetUsageStrategy<T>() where T : AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.UsageStrategies.ItemUsageStrategy`: Returns a usage strategy instance by its class. Returns `null` if not found.
8. `public AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.UsageStrategies.ItemUsageStrategy GetUsageStrategy(Type type)`: Returns a usage strategy instance but by passing the type as value.
9. `public AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.UsageStrategies.ItemUsageStrategy MainUsageStrategy`: Returns the assigned main strategy.
10. `public T GetRenderingStrategy<T>() where T : AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.RenderingStrategies.ItemRenderingStrategy`: Returns a rendering strategy instance by its class. Returns `null` if not found.
11. `public AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.RenderingStrategies.ItemRenderingStrategy GetRenderingStrategy(Type type)`: Returns a rendering strategy instance but by passing the type as value.
12. `public AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.RenderingStrategies.ItemRenderingStrategy MainRenderingStrategy`: Returns the assigned main strategy.

This is quite important for the items themselves. While they are _passive_ objects, they provide all the underlying elements for the _active_ objects to exist.

_Again: strategies will be described later_.

An example to understand how an item is defined would look like this:

1. Sword:
   - Quantifying strategy: Non-stacked.
   - Usage strategies:
     - Code that harms the player in front (also, the main usage strategy).
   - Spatial Strategies:
     - A single position in a very basic inventory settings (almost all games match this).
   - Rendering Strategies:
     - A single rendering strategy displaying image (a sword icon) and text ("Sword").
2. Apple:
   - Quantifying strategy: Stacked (at most: 100 items).
   - Usage strategies:
     - Code that lowers user's "hunger" by 10 points (also, the main usage strategy).
   - Spatial Strategies:
     - A single position in a very basic inventory settings (almost all games match this).
   - Rendering Strategies:
     - A single rendering strategy displaying image (an apple icon) and text ("Apple").

### Assets: Inventory registries

Registries are also an important part and are also created as assets. This time, with the menu:

```
Assets/Create/Aleph Vault/BackPack/Inventory/Item Registry
```

They're, essentially, a _dictionary of items_. Its serializable / inspector properties are:

1. `Key`: An internal string to name this registry.

Also, public properties and methods are:

1. `public static AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.ItemRegistry GetRegistry(string key)`: Gets a registry instance by their key. Returns `null` if not found.
2. `public static System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.ItemRegistry>> Registries()`: Gets the list of registered registries with their keys.
3. `public AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.Item GetItem(uint key)`: Gets one item in this registry by its key. Returns `null` if not found.
4. `public System.Collections.Generic.IEnumerable<KeyValuePair<uint, AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.Item>> Items()`: Gets the list of items in this registry with their keys.
5. `public static AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.Item GetItem(string registryKey, uint itemKey)`: Returns an item by specifying the registry key and the item key as well. Returns `null` if not found.
6. `public string Key`: The registry key, if any.
7. `public bool Contains(uint key)`: Tells whether an item with the specified key is present in this registry.
8. `public bool AddItem(AlephVault.Unity.BackPack.Authoring.ScriptableObjects.Inventory.Items.Item item)`: Tries to add an item by its key. If the key is 0 or is already in use by an object already registered, then returns `false`. Otherwise, it adds the object and returns `true`.

With this in mind, items can be registered into these registries. Still, there's something yet to be described: strategies.

### Assets: Inventory item strategies

Strategies are assets that act as subcomponents that can (and will) be added to the `Item` assets.

Some strategy assets are shared among many items, to the point that perhaps only one strategy instance is needed.

#### Quantifying Strategies

The quantifying strategies are those that define how will this object be stacked in a single place (e.g. potions being stacked up to 100).

There are three subtypes:

##### Quantifying Strategy: Unstacked

This quantifying strategy is for objects that cannot be grouped together. Typically, weapons or big items.

They can be created by clicking this menu option:

```
Assets/Create/Aleph Vault/BackPack/Inventory/Item Strategies/Quantifying/Unstacked
```

This asset requires no configuration. It can be directly used as a quantifying strategy on any one or more inventory item.

Only one item of this type is needed.

##### Quantifying Strategy: Integer-Stacked

This quantifying strategy is for objects that can be grouped together. Typically, consumable items.

They can be created by clicking this menu options:

```
Assets/Create/Aleph Vault/BackPack/Inventory/Item Strategies/Quantifying/Integer-Stacked
```

This asset requires a configuration: a `Max` integer value. E.g. `100`. Many items may share this quantifying strategy.

One is needed for each maximum quantity that needs to be defined (e.g. one for 100, one for 25, ...).

##### Quantifying Strategy: Float-Stacked

This is less common: Works the same as Integer-Stacked but with float values.

```
Assets/Create/Aleph Vault/BackPack/Inventory/Item Strategies/Quantifying/Float-Stacked
```

#### Spatial Strategies

Spatial strategies tell how an item is stored in an inventory. Each item needs at least one spatial strategy, and each
spatial strategy determines which type of inventory will be able to accept the item. Examples:

1. One item without spatial strategy cannot be used as an item.
2. One item with "simple" spatial strategy can be stored in "simple" (linear) inventories.
3. One item with "rectangled" spatial strategy can be stored in a "rectangled" inventory.
   E.g. like this is done in games like Diablo or Neverwinter Nights.
4. One item with "simple" spatial strategy and "rectangled" strategy can be stored in both
   "simple" and "rectangled" inventories.

This means: each spatial strategy allows being stored on different inventory types.

_just in case, "rectangled" inventories are not supported in this version_.

_Also: users can define new types of spatial strategies and their corresponding inventory strategies as needed_.

##### Spatial Strategy: 1D-indexed

This is a "simple" spatial strategy for items: The item occupies a single place inside a 1-dimensional list given by
its index.

In order to create this strategy object, this is the menu option:

```
Assets/Create/Aleph Vault/BackPack/Inventory/Item Strategies/Quantifying/Float-Stacked
```

Only one asset of this type is needed, and can be added to as many items as needed.

#### Usage Strategies

Usage strategies are the more complex objects to understand here, since they need a way to define the behaviour that
will be triggered when the object is somehow _used_.

These are per-game items, so almost no usage strategy class is provided by this package but only a _dummy_ or _null_
one in which no logic is performed at all. Users must define their own usage strategy classes.

##### Usage Strategy: Null

This usage strategy does nothing. It's useful for objects which are not meant to be used (objects that are critical,
can be traded, or some sort of special tokens).

It can be created by clicking this menu:

```
Assets/Create/Aleph Vault/BackPack/Inventory/Item Strategies/Usage/Null Usage (e.g. tokens, critical objects)
```

As with others, only one asset of this type is needed to be shared by all the objects that need no usage execution.

#### Rendering Strategies

Rendering strategies are a different type of strategies: they don't describe the _logic_ of the inventory (position,
stacking or usage) but instead describe how will the object be rendered in a _view_. Rendering strategies provide all
the data that can be used by a _view_ (but not necessarily each view _must_ use all the provided data by the item's
rendering strategy).

## Behaviours: Inventories and strategies

## Data types: Stacks and strategies

## UI
