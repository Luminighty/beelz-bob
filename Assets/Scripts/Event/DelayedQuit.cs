using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedQuit : Trigger
{

	public float Wait;

	public override void onTrigger ()
	{
		StartCoroutine (Delay ());
	}

	IEnumerator Delay ()
	{
		yield return new WaitForSeconds (Wait);
		Application.Quit ();
	}
}
