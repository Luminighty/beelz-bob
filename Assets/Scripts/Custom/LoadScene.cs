using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : Trigger
{
	public int SceneInt;
	SceneManager sceneMan;

	public override void onTrigger ()
	{
		SceneManager.LoadScene (SceneInt);
	}
}
