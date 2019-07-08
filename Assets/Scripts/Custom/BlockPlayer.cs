using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlayer : Trigger
{
	public bool isenabled;
	public bool isPlayerMov = true;

	public override void onTrigger ()
	{
		if (!isPlayerMov) {
			FindObjectOfType<PlayerMovement> ().MoveByTrigger = !isenabled;
		} else {
			FindObjectOfType<PlayerMovement> ().enabled = isenabled;

		}
	}
}
