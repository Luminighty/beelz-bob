using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCatch : MonoBehaviour
{

	public GameObject DoneTrap;
	public GameObject newTrap;

	void OnTriggerStay (Collider col)
	{
		if (col.tag != "Player")
			return;

		if (DoneTrap.activeSelf) {
			StartCoroutine (Freeze ());
		}

	}



	IEnumerator Freeze ()
	{
		newTrap.SetActive (true);
		DoneTrap.SetActive (false);
		yield return new WaitForSeconds (0.5f);
		GetComponent<Interact> ().onInteract ();
	}

}
