using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{

	public Interact inter;
	[System.NonSerialized]
	public Dialog dial;

	public void OnClick ()
	{
		inter.StartCoroutine (inter.DoDialog (dial, false));
	}

}
