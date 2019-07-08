using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour {

	Inventory inv;
	public int index;

	void Awake() {
		inv = transform.parent.GetComponent<Inventory> ();
	}

	public void Hover() {
		inv.hovering = index;
	}

	public void Click() {
		inv.Click (index);
	}
}
