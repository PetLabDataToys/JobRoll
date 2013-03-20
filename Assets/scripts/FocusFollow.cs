using UnityEngine;
using System.Collections;

public class FocusFollow : MonoBehaviour {
	
	public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 2f);
	}
}
