using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PreventSteal : MonoBehaviour
{

	public Inventory inv;
	public Interact interact;
	public Item removeItem;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0) && !inv.isOpen) {
			interact.onInteract ();
			GameObject player = GameObject.Find ("Player");
			player.GetComponent<NavMeshAgent> ().SetDestination (player.transform.position);
			inv.RemoveItem (removeItem);
		}
	}



}
