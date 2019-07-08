using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{

	public Image im;

	public Events onEnd;
	public bool isFadeIn;

	void OnEnable ()
	{
		if (im == null)
			im = GetComponent<Image> ();
		StartCoroutine (startCutscene ());

	}

	IEnumerator startCutscene ()
	{
		float alpha = (isFadeIn) ? 1 : 0;
		im.color = new Color (0, 0, 0, alpha);
		for (int i = 0; i < 30; i++) {
			float a = im.color.a;
			if (isFadeIn) {
				a -= 1 / 30.0f;
			} else {
				a += 1 / 30.0f;
			}
			im.color = new Color (im.color.r, im.color.g, im.color.b, a);
			for (int j = 0; j < 2; j++)
				yield return new WaitForEndOfFrame ();
		}
		if (onEnd != null)
			onEnd.Trigger ();
	}
}
