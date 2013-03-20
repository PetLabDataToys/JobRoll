using UnityEngine;
using System.Collections;

public class TiltControllerBall : MonoBehaviour {
	
	Quaternion inputRot;
	Quaternion targetRot;
	const float tiltRange = 45f;
	
	Vector3 moveDirection;
	float force = 50f;
	float nextJumpTimeCooldown = 0f;
	const float cooldownDuration = 1f;
	
	public GUIText text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		float x = 1f - ( Input.mousePosition.x / (Screen.height / 2f) );
//		float y = 1f - ( Input.mousePosition.y / (Screen.width / 2f) );

//		inputRot = Quaternion.Euler( Mathf.Clamp( y * -tiltRange, -tiltRange, tiltRange), 0f, Mathf.Clamp(x * tiltRange, -tiltRange, tiltRange) );
//		targetRot = Quaternion.Slerp(targetRot, inputRot, Time.deltaTime * 3f);
		Vector3 fore = Vector3.Normalize(new Vector3 (Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z));
		Vector3 right = Vector3.Normalize(new Vector3 (Camera.main.transform.right.x, 0f, Camera.main.transform.right.z));
		moveDirection = Input.GetAxis("Vertical") * fore + Input.GetAxis("Horizontal") * right;
		if (Input.GetButtonDown("Jump") && Time.time > nextJumpTimeCooldown) {
			moveDirection += Vector3.up * force * 0.3f;
			nextJumpTimeCooldown = Time.time + cooldownDuration;
		}
		
	#if UNITY_ANDROID
		if (!Application.isEditor) {
			moveDirection = Input.acceleration.y * fore + Input.acceleration.x * right;
			moveDirection *= 1.6f;
			if (Input.acceleration.magnitude > 2f && Time.time > nextJumpTimeCooldown) { // if (shake), then jump a bit
				moveDirection += Vector3.up * force * 0.2f;
				nextJumpTimeCooldown = Time.time + cooldownDuration;
			}
			if (WorldGen.showSliders) {
				text.enabled = true;
				text.text = "ACCELERATION: " + Input.acceleration.ToString() + ", magnitude: " + Input.acceleration.magnitude.ToString();
			} else {
				text.enabled = false;
			}
		}
	#endif
	}
	 
	void FixedUpdate () {
//		rigidbody.MoveRotation(targetRot);
		
//		rigidbody.AddForce(Physics.gravity, ForceMode.Acceleration);
		rigidbody.AddForce(moveDirection * force * Time.fixedDeltaTime, ForceMode.VelocityChange);
		
	}
	
//	void OnGUI () {
//		if (WorldGen.showSliders) {
//			GUI.Label(new Rect(0f, Screen.height - 64f, Screen.width, 32f), "ROLL FORCE (" + force.ToString() + ")" );
//			force = GUI.HorizontalSlider(new Rect(0f, Screen.height - 32f, Screen.width, 32f), force, 1f, 20f);
//		}
//	}
}
