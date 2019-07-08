using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{

	Vector3 goTo;
	public Interact inter;
	public float speed;
	private NavMeshAgent agent;
	public Inventory inventory;
	[System.NonSerialized]
	public bool isInteracting;
	public bool MoveByTrigger;
	Animator anim;

	void Awake ()
	{
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
		inventory = FindObjectOfType<Inventory> ();
	}

	void Update ()
	{
		Move ();
		onPathFinish ();
	}

	void onPathFinish ()
	{

		if (MoveByTrigger)
			return;
		if (Vector3.Distance (agent.destination, transform.position) > 0.1f) {
			anim.SetBool ("isWalking", true);
			return;
		}
		anim.SetBool ("isWalking", false);
		if (inter != null) {
			if (inventory.HeldItem > -1)
				inter.interactItem = inventory.items [inventory.HeldItem];

			if (transform.position.x < inter.transform.position.x) {
				transform.GetChild (0).GetComponent<SpriteRenderer> ().flipX = false;
				isLookRight = true;
			}
			if (transform.position.x > inter.transform.position.x) {
				transform.GetChild (0).GetComponent<SpriteRenderer> ().flipX = true;
				isLookRight = false;
			}

			inter.onInteract ();

			inter = null;
		}
	}

	void interact (RaycastHit hit)
	{
		inter = hit.transform.GetComponent<Interact> ();
		agent.SetDestination (inter.InteractPosition.bounds.center);
	}

	void Look ()
	{
		if (Vector3.Distance (agent.destination, transform.position) < 0.1f)
			return;
		if (isLookRight && agent.destination.x < transform.position.x) {
			transform.GetChild (0).GetComponent<SpriteRenderer> ().flipX = true;
			isLookRight = false;
		}
		if (!isLookRight && agent.destination.x > transform.position.x) {
			transform.GetChild (0).GetComponent<SpriteRenderer> ().flipX = false;
			isLookRight = true;
		}

	}

	bool isLookRight = true;

	void Move ()
	{
		transform.eulerAngles = new Vector3 (90, 0, 0);

		if (inventory.isOpen || isInteracting) {		
			if (MoveByTrigger)
				return;
			if (inventory.isOpen)
				inter = null;
			if (Vector3.Distance (agent.destination, transform.position) > 1)
				agent.SetDestination (transform.position);
			return;
		} else {
			Look ();
			if (MoveByTrigger)
				return;
		}
		if (Input.GetMouseButton (0)) {
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100)) {
				if (hit.transform.tag == "Interactable") {
					transform.eulerAngles = new Vector3 (90, 0, 0);
					interact (hit);
					return;
				} else {
					inter = null;
				}
				agent.SetDestination (hit.point);
			}
		}

		if (Input.GetMouseButton (1)) {
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100)) {
				if (hit.transform.tag == "Interactable") {
					hit.transform.GetComponent<Interact> ().onInvestigate ();
					agent.SetDestination (transform.position);
					transform.eulerAngles = new Vector3 (90, 0, 0);
					return;
				}
			}
		}


	}

}
