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

## Items and registries

The first thing for a game with inventory is to actually define the objects that can be held in the inventory, and also a way to register those objects for their access.

So, to start, there's a concept of `Item` and `ItemRegistry` that will be addressed here.

### Inventory items

Items are the core of this package, but _they're actually made of several components_ since an item involves several aspects:

1. How is it _positioned_ inside an inventory.
2. How is it _displayed_ to the end user.
3. How is it _quantified_: most of them have integer quantities (e.g. potions), and some of them are, instead, non-stacked / one-only (e.g. a sword).
4. And finally but not least: what's their usage logic.

Items are game assets, and they're created by clicking the following menu option:

```
Assets/Create/Aleph Vault/WindRose/Inventory/Item
```

This action creates a new item in the current project explorer's folder. This item is an object with the following properties and methods:

