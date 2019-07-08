using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPosTrigger : Trigger
{

	PlayerMovement player;
	NavMeshAgent agent;

	public Transform position;

	void Start ()
	{
		player = GameObject.Find ("Player").GetComponent<PlayerMovement> ();
		agent = player.GetComponent<NavMeshAgent> ();
	}

	public override void onTrigger ()
	{

		player.MoveByTrigger = true;
		agent.SetDestination (position.position);

	}

	void Update ()
	{
		if (Vector3.Distance (agent.destination, player.transform.position) > 0.5f)
			return;

		player.MoveByTrigger = false;

	}
}
