using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjects : Trigger
{

	public float Delay = 0.0f;
	public Events NextEvent;

	public GameObject[] enable;
	public GameObject[] disable;
	public GameObject[] toggle;

	public override void onTrigger ()
	{
		StartCoroutine (Wait ());
	}

	IEnumerator Wait ()
	{
		yield return new WaitForSeconds (Delay);

		foreach (GameObject en in enable)
			en.SetActive (true);
		foreach (GameObject dis in disable)
			dis.SetActive (false);
		foreach (GameObject tog in toggle)
			tog.SetActive (!tog.activeSelf);
		if (NextEvent != null)
			NextEvent.Trigger ();

	}

}
