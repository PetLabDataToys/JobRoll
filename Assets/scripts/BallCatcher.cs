using UnityEngine;
using System.Collections;

public class BallCatcher : MonoBehaviour {
	
	Vector3 respawn;
	public GUISkin skin;

	// Use this for initialization
	void Start () {
		respawn = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -10f)
			Respawn();
	}
	
	void Respawn () {	
		transform.position = respawn;
		rigidbody.AddForce(-rigidbody.velocity * 10f);
	}
	
	void OnGUI () {
		GUI.skin = skin;
		
		if (GUI.Button(new Rect(Screen.width / 2f - 80f, 0f, 160f, 32f), "RESPAWN (HELP!)") )
			Respawn();
	}
}
