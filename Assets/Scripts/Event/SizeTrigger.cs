using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeTrigger : Trigger
{
	public int id;
	[TextArea]
	public string TriggerDescription;
	public List<SizeFitter> Toggle = new List<SizeFitter> ();
	public List<SizeFitter> Disable = new List<SizeFitter> ();
	public List<SizeFitter>	Enable = new List<SizeFitter> ();

	public override void onTrigger ()
	{
		foreach (SizeFitter tog in Toggle)
			tog.enabled = !tog.enabled;
		foreach (SizeFitter tog in Enable)
			tog.enabled = true;
		foreach (SizeFitter tog in Disable)
			tog.enabled = false;
	}

}
