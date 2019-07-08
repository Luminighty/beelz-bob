using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Inventory : MonoBehaviour
{

	public bool isOpen;

	public int hovering;
	public int HeldItem;
	public GameObject HeldObject;

	public SpeakLine Default_Cannot_Craft;

	public List<Item> items = new List<Item> ();
	GameObject[] Slots;
	GameObject TextBubble;
	Animator anim;
	PlayerMovement playermov;

	public bool hasItem (Item item)
	{
		foreach (Item invItem in items)
			if (invItem == item)
				return true;
		return false;
	}

	public int ItemCount (Item item)
	{
		int i = 0;
		foreach (Item invItem in items)
			if (invItem == item)
				i++;
		return i;
	}

	public void AddItem (Item item)
	{
		int pos = getIndex (null);
		if (pos == -1) {
			Debug.LogError ("Can't find empty slot!");
			return;
		}
		SetItem (pos, item);
		SortItems ();
	}

	public void RemoveItem (Item item)
	{
		for (int i = 0; i < items.Count; i++)
			if (items [i] == item) {
				SetItem (i, null);
				SortItems ();
				return;
			}
	}

	void SortItems ()
	{
		for (int i = 1; i < items.Count; i++) {
			if (items [i] != null && items [i - 1] == null) {
				SetItem (i - 1, items [i]);
				SetItem (i, null);
			}
		}

	}

	void Awake ()
	{
		TextBubble = GameObject.Find ("DefaultTextBubble");
		anim = GetComponent<Animator> ();
		SetupSlots ();
		playermov = FindObjectOfType<PlayerMovement> ();
	}

	void SetupSlots ()
	{
		Slots = new GameObject [items.Count];
		for (int i = 0; i < transform.childCount; i++) {
			Slots [i] = transform.GetChild (i).gameObject;
			Slots [i].GetComponent<ItemSlot> ().index = i;
			SetItem (i, items [i]);
		}
		SortItems ();
	}

	public void Click (int index)
	{
		if (index == -1) {
			HeldItem = -1;
			return;
		}
		if (items [index] == null || index == HeldItem) {
			HeldItem = -1;
			return;
		}
		if (HeldItem != -1) {
			Craft (items [index], items [HeldItem]);
			HeldItem = -1;
			return;
		}
		HeldItem = index;
	}

	public void RightClick (int index)
	{
		if (!isOpen)
			return;
		if (HeldItem != -1) {
			HeldItem = -1;
			return;
		}
		if (index < items.Count && items [index] != null) {
			StopAllCoroutines ();
			StartCoroutine (ShowText (items [index].investigate));
		}
	}

	public void Craft (Item item1, Item item2)
	{
		if (!item1.canCraftedWith (item2)) {
			StopCoroutine ("ShowText");
			StartCoroutine (ShowText (Default_Cannot_Craft));
			return;
		}
		Craft craft = item1.CraftWith (item2);
		if (craft.isCraftable) {
			SetItem (getIndex (item1), null);
			SetItem (getIndex (item2), null);
			AddItem (craft.newItem);
		} else {

			Slots [HeldItem].transform.GetChild (0).GetComponent<Image> ().color = Color.white;
		}
		if (craft.onCraft != null) {
			StopCoroutine ("ShowText");
			StartCoroutine (ShowText (craft.onCraft));

		}
		HeldItem = -1;
		saveHeldItem = -1;
		if (craft.closeInventory) {
			Scroll (false);
		}
		SortItems ();
	}

	IEnumerator ShowText (SpeakLine line)
	{
		if (line.Actor == null)
			line.Actor = playermov.gameObject;
		TextBubble.SetActive (true);
		playermov.isInteracting = true;
		TextBubble.GetComponent<Text> ().text = line.text;
		Vector3 pos = Input.mousePosition;
		pos.y += 100;
		TextBubble.transform.position = pos;
		float lenght = (float)getLineLenght (line);
		float now = Time.unscaledTime;
		bool skip = false;
		bool isMouseUp = !Input.GetMouseButton (0);
		if (line.Actor.GetComponent<Animator> () != null) {
			line.Actor.GetComponent<Animator> ().SetBool ("isSpeaking", false);
		}
		while (Time.unscaledTime < now + lenght && !skip) {		
			if (!Input.GetMouseButton (0))
				isMouseUp = true;
			if (Input.GetMouseButton (0) && isMouseUp) {
				//Debug.Log ("Pressed");
				skip = true;
			}
			yield return null;
		}


			
		if (line.Actor.GetComponent<Animator> () != null) {
			if (line.animation != "") {
				line.Actor.GetComponent<Animator> ().SetTrigger (line.animation);
			} else {
				line.Actor.GetComponent<Animator> ().SetBool ("isSpeaking", false);
			}
		}

		playermov.isInteracting = false;
		TextBubble.SetActive (false);
	}

	float getLineLenght (SpeakLine line)
	{
		if (line.audio != null) {
			return line.audio.length;
		}
		return Mathf.Max (line.text.Length * 0.05f, 1.5f);

	}

	public void StopHover ()
	{
		isOpen = false;
		anim.SetBool ("isOpen", false);
	}

	void Update ()
	{
		float scroll = Input.mouseScrollDelta.y;
		if (scroll <= -1 && !isOpen)
			Scroll (true);
		if (scroll >= 1 && isOpen)
			Scroll (false);
		ShowHeldItem ();
		if (Input.GetMouseButtonDown (1))
			RightClick (hovering);

		if (Input.GetMouseButtonDown (2) || Input.GetKeyDown (KeyCode.E))
			Scroll (!isOpen);
	}



	void ShowHeldItem ()
	{
		if (saveHeldItem != -1 && items [saveHeldItem] == null)
			saveHeldItem = -1;
		if (HeldItem == -1) {
			if (saveHeldItem != -1)
				Slots [saveHeldItem].transform.GetChild (0).GetComponent<Image> ().color = Color.white;
			HeldObject.GetComponent<Image> ().color = Color.clear;
			saveHeldItem = HeldItem;
			return;
		}
		HeldObject.GetComponent<Image> ().color = Color.white;
		HeldObject.GetComponent<Image> ().sprite = items [HeldItem].Icon;
		HeldObject.transform.position = Input.mousePosition;
		if (saveHeldItem != HeldItem) {
			if (saveHeldItem != -1)
				Slots [saveHeldItem].transform.GetChild (0).GetComponent<Image> ().color = Color.white;
			saveHeldItem = HeldItem;
			Slots [saveHeldItem].transform.GetChild (0).GetComponent<Image> ().color = Color.clear;
		}
	}

	int saveHeldItem = -1;

	void Scroll (bool open)
	{
		if (playermov.isInteracting || !playermov.enabled || playermov.MoveByTrigger)
			return;
		isOpen = open;
		anim.SetBool ("isOpen", open);
	}


	/// <summary>
	/// Creates an item to a given position
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="item">Item.</param>
	void SetItem (int pos, Item item)
	{
		items [pos] = item;
		Image im = Slots [pos].transform.GetChild (0).GetComponent<Image> ();
		if (item == null) {
			im.color = Color.clear;
			im.sprite = null;
		} else {
			im.color = Color.white;
			im.sprite = item.Icon;
		}
	}

	int getIndex (Item item)
	{
		for (int i = 0; i < items.Count; i++)
			if (items [i] == item)
				return i;
		return -1;
	}

}
