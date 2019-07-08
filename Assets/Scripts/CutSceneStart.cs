using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneStart : Trigger
{

	public Cutscene cutscene;

	public override void onTrigger ()
	{
		cutscene.enabled = true;
	}

}
