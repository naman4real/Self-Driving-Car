using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircuitFollowScript : MonoBehaviour
{

	public carController car;
	public TrackBuilder trackBuilder;


	public float offset;
	public float speedFactor, distanceFactor, minSpeed;
	public float maxSpeed = 10;

	private Rigidbody carRigidbody;
	private int finalIndex, carPositionIndex;
	private Vector3 finalPosition;

	// Use this for initialization
	void Awake ()
	{
		carRigidbody = car.GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		MoveTarget ();
	}

	void MoveTarget ()
	{
		float newFinalIndex = 0;
		carPositionIndex = trackBuilder.GetPositionOnTrack (car.gameObject);

		newFinalIndex = carPositionIndex + offset * Mathf.Clamp (speedFactor * (car.currSpeed / car.topSpeed), 1, 10);

		finalIndex = (int)newFinalIndex % trackBuilder.newWaypoints.Length;
		finalPosition = trackBuilder.newWaypoints [finalIndex];

		Vector3 nextPosition = trackBuilder.newWaypoints [(int)(newFinalIndex + 1) % trackBuilder.newWaypoints.Length];

		float delta = Mathf.Clamp (speedFactor * car.currSpeed + distanceFactor * (1 / Vector3.Distance (transform.position, car.transform.position)), minSpeed, maxSpeed);

		transform.position = Vector3.MoveTowards (transform.position, finalPosition, delta);
		transform.LookAt (nextPosition);
	}

	public int GetTargetPositionIndex ()
	{
		return finalIndex;
	}
}
