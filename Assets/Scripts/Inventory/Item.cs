using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Item", menuName = "Item", order = 1)]
public class Item : ScriptableObject
{
	
	public string name;
	public Sprite Icon;
	public SpeakLine investigate;

	public Craft[] craftablewith;

	/// <summary>
	/// Checks if the item can be crafted with another.
	/// </summary>
	/// <returns><c>true</c>, if can be crafted, <c>false</c> otherwise.</returns>
	/// <param name="item2">Item2.</param>
	public bool canCraftedWith (Item item2)
	{
		foreach (Craft craft in craftablewith)
			if (craft.ingredient == item2)
				return true;
		foreach (Craft craft in item2.craftablewith)
			if (craft.ingredient == this)
				return true;
		return false;
	}

	public Craft CraftWith (Item item2)
	{
		for (int i = 0; i < this.craftablewith.Length; i++)
			if (this.craftablewith [i].ingredient == item2)
				return this.craftablewith [i];
		
		for (int i = 0; i < item2.craftablewith.Length; i++)
			if (item2.craftablewith [i].ingredient == this)
				return item2.craftablewith [i];
		return null;
	}

}

[System.Serializable]
public class Craft
{

	/// <summary>
	/// Ingredient, the ingredient doesn't have to have this item as a craftable thing;
	/// </summary>
	public Item ingredient;
	/// <summary>
	/// What will be created?
	/// </summary>
	public Item newItem;
	/// <summary>
	/// False = Use it only for audio; True = will create a new Item
	/// </summary>
	public bool isCraftable;
	/// <summary>
	/// Should the inventory be closed if the crafting is done?
	/// </summary>
	public bool closeInventory;
	/// <summary>
	/// What should the player say when it's crafted;
	/// </summary>
	public SpeakLine onCraft;
}

