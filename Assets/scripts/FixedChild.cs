using UnityEngine;
using System.Collections;

public class FixedChild : MonoBehaviour {
	
	Vector3 offset;
	Vector3 lookVector;

	// Use this for initialization
	void Start () {
		offset = transform.localPosition;
		lookVector = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.parent.position + offset;
		transform.rotation = Quaternion.LookRotation(lookVector);
	}
}
