using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFitter : MonoBehaviour
{

	public Vector2 Min;
	public Vector2 Max;
	public GameObject Canvas;

	void Awake ()
	{
		Vector2 pos = Canvas.GetComponent<RectTransform> ().position;
		Vector2 size = Canvas.GetComponent<RectTransform> ().sizeDelta;
		Min.x = pos.x - (size.x / 2.0f);
		Max.x = pos.x + (size.x / 2.0f);
		Min.y = pos.y - (size.y / 2.0f);
		Max.y = pos.y + (size.y / 2.0f);
		Vector2 thisSize = GetComponent<RectTransform> ().sizeDelta;

		Min.y += thisSize.y / 2.0f;
		Max.y -= thisSize.y / 2.0f;		
		Min.x += thisSize.x / 2.0f;
		Max.x -= thisSize.x / 2.0f;

	}


	void Update ()
	{
		Vector3 pos = transform.position;

		pos.x = Mathf.Max (Min.x, pos.x);
		pos.x = Mathf.Min (Max.x, pos.x);

		pos.y = Mathf.Max (Min.y, pos.y);
		pos.y = Mathf.Min (Max.y, pos.y);

		transform.position = pos;
	}

	public void NewPos ()
	{

		Vector3 pos = transform.position;

		pos.x = Mathf.Max (Min.x, pos.x);
		pos.x = Mathf.Min (Max.x, pos.x);

		pos.y = Mathf.Max (Min.y, pos.y);
		pos.y = Mathf.Min (Max.y, pos.y);

		transform.position = pos;
	}

}
