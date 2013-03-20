using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {
	
	float speed = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += Time.deltaTime * speed * ( transform.right * Input.GetAxis("Horizontal") + 
												new Vector3( transform.forward.x, 0f, transform.forward.z) * Input.GetAxis("Vertical") );
	}
}
