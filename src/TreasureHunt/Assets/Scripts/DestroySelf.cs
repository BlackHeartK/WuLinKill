using UnityEngine;

public class DestroySelf : MonoBehaviour {

    public float destoryDelayTime = 1;

	void Start () {
        Destroy(gameObject, destoryDelayTime);
	}
	
	void OnDestroy () {
		
	}
}
