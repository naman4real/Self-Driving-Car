using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheelScript : MonoBehaviour {
	public float maxRotationAngle = 40;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion newRot = transform.rotation;
		newRot.z = Mathf.SmoothStep (0, -40*maxRotationAngle * Input.GetAxis ("Horizontal"), 0.01f);

		transform.rotation = newRot;
	}
}
