using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustController : MonoBehaviour
{

	public ParticleSystem[] exhaustPipes;
	public Light leftNitro, rightNitro;
	public carController car;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (car.isBoosting) {
			for (int i = 0; i < 4; i++) {
				exhaustPipes [i].Play ();
			}
			leftNitro.intensity = rightNitro.intensity = 1.5f;
		} else {
			for (int i = 0; i < 4; i++) {
				exhaustPipes [i].Stop ();
			}
			leftNitro.intensity = rightNitro.intensity = 0;
		}

	}
}
