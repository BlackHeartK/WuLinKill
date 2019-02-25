using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotSelf : MonoBehaviour {

	public float fac = 1.0f;

	void Update () {
		transform.Rotate (Vector3.up, Time.deltaTime * Time.time * fac, Space.Self);
	}
}
