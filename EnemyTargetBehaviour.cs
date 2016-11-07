using UnityEngine;
using System.Collections;

public class EnemyTargetBehaviour : MonoBehaviour {

    // Use this for initialization
    public float offSet = -2f;
    private float xBoundary;


	// Update is called once per frame
	void FixedUpdate () {
        xBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;
        Vector3 pos = transform.position;
        pos.x = xBoundary + offSet;
        transform.position = pos;
	}
}
