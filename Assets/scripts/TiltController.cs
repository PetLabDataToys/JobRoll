using UnityEngine;
using System.Collections;

public class TiltController : MonoBehaviour {
	
	Quaternion inputRot;
	Quaternion targetRot;
	const float tiltRange = 45f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float x = 1f - ( Input.mousePosition.x / (Screen.height / 2f) );
		float y = 1f - ( Input.mousePosition.y / (Screen.width / 2f) );
	//	Debug.Log(x + "," + y);
		inputRot = Quaternion.Euler( Mathf.Clamp( y * -tiltRange, -tiltRange, tiltRange), 0f, Mathf.Clamp(x * tiltRange, -tiltRange, tiltRange) );
		targetRot = Quaternion.Slerp(targetRot, inputRot, Time.deltaTime * 3f);
	//	transform.rotation = targetRot;
	}
	
	void FixedUpdate () {
		rigidbody.MoveRotation(targetRot);
	}
}
