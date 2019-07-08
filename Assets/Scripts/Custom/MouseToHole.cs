using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseToHole : Trigger
{

	public override void onTrigger ()
	{
		GetComponent<Animator> ().SetTrigger ("trigger");
	}
}
