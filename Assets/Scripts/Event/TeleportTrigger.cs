using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportTrigger : Trigger
{

	public GameObject actor;
	public Transform position;

	public override void onTrigger ()
	{
		actor.transform.position = position.position;
		if (actor.GetComponent<PlayerMovement> () != null) {
			actor.GetComponent<NavMeshAgent> ().enabled = false;
			actor.transform.position = position.position;
			actor.GetComponent<PlayerMovement> ().inter = null;
			actor.GetComponent<NavMeshAgent> ().enabled = true;
			actor.GetComponent<NavMeshAgent> ().SetDestination (actor.transform.position);
		}
	}

}
