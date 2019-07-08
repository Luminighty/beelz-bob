using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCredits : Trigger
{

	public GameObject anim;

	public override void onTrigger ()
	{
		anim.SetActive (true);

	}


}
