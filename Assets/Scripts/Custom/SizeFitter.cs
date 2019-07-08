using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeFitter : MonoBehaviour
{

	public float yMax;
	public float MaxSize;

	public float yMin;
	public float MinSize;

	private Transform trans;
	private Vector2 vector;
	private float xMultiplier;
	private float yMultiplier;
	private float offset;

	void Start ()
	{
		trans = GetComponent<Transform> ();
		float A = yMax - yMin;
		float B = MaxSize - MinSize;
		xMultiplier = (float)-B;
		yMultiplier = (float)A;
		offset = xMultiplier * yMin + yMultiplier * MinSize;
	}


	void Update ()
	{
		SetSize (trans.position.z);
	}

	void SetSize (float pos)
	{
		pos = Mathf.Max (yMin, pos);
		pos = Mathf.Min (yMax, pos);

		float size = (float)offset - (xMultiplier * pos);
		size *= 1.0f / yMultiplier;
		trans.localScale = new Vector3 (size, size, size);
	}

}
