using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerfabS))]
public class PerfabS : Editor 
{
	[MenuItem("Tools/GetPerfabType")]
	static void GetPerfabType()
	{
		GameObject[] prefabs = Selection.gameObjects;

		Debug.Log (PrefabUtility.GetPrefabType (prefabs [0]));
	}
}
