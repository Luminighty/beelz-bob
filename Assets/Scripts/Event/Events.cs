using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{

	public enum eventTrigger
	{
		Instant,
		Collision,
		onInteract,
		Custom
	}

	private Collider col;

	public eventTrigger trigger;
	public Interact interact;
	public int Id;

	public delegate void onTrigger ();

	public event onTrigger triggered;

	void Awake ()
	{
		if (GetComponent<Collider> () != null) {
			col = GetComponent<Collider> ();
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "Player" && trigger == eventTrigger.Collision) {
			Debug.Log ("Trigger");
			Trigger ();
		}
	}

	// Use this for initialization
	void Start ()
	{
		playerMov = FindObjectOfType<PlayerMovement> ();
		if (trigger == eventTrigger.Instant) {
			Trigger ();
			/*if (triggered.GetInvocationList ().Length > 0)
				triggered ();*/
		}
	}

	PlayerMovement playerMov;

	public void Trigger ()
	{
		playerMov.inter = null;
		//triggered ();
		DialogEvents ();
		foreach (Trigger trig in triggers) {
			if (trig.GetType () == typeof(SizeTrigger)) {
				SizeTrigger sizetrig = (SizeTrigger)trig;
				if (sizetrig.id != Id)
					continue;
			}
			trig.onTrigger ();
		}
		if (interact != null)
			interact.onInteract ();
	}

	void DialogEvents ()
	{

		foreach (Dialog d in dialogs) {
			d.isAvailable = !d.isAvailable;
			Debug.Log (d.optionName + " is set to" + d.isAvailable);
		}
	}

	[System.NonSerialized]
	public List<Dialog> dialogs = new List<Dialog> ();

	public List<Trigger> triggers = new List<Trigger> ();
}
