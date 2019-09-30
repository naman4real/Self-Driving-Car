using UnityEngine;
using System.IO;

public class CarAiController : MonoBehaviour
{
	public Transform target;
	public CircuitFollowScript cfs;
	public Transform track;

	public float bias;
	public float steerSensitivity;
	public float brakeSensitivty;
	public float throttleOffset;
	public float obstacleAvoidanceSteerWeight, obstacleAvoidanceBrakeWeight;

	private float directionShift;
	private float distanceToTarget;
	private float distanceToCorner, velocityToCorner, cornerAngle;
	private float factor;
	private float angleToTarget;
	private float headingShift;
	private Transform[] trackPoints;
	private InputManager inputManager;
	private carController car;
	private Rigidbody carRigidbody;
	private TrackBuilder trackBuilder;
	private NeuralBrain brain;
	private TrainingDataRecorder tdr;

	[SerializeField]private float throttle, brake, steer;

	[HideInInspector]public float steerOverride;

	void Awake ()
	{
		inputManager = GetComponent<InputManager> ();
		trackPoints = new Transform[track.childCount];

		for (int i = 0; i < trackPoints.Length; i++) {
			trackPoints [i] = track.GetChild (i);
		}
		car = GetComponent<carController> ();
		carRigidbody = GetComponent<Rigidbody> ();
		trackBuilder = track.GetComponent<TrackBuilder> ();

		tdr = GetComponent<TrainingDataRecorder> ();
		brain = GetComponent<NeuralBrain> ();
	}

	void Update ()
	{
		SetParameters ();
		SetSteerInput ();
		SetThrottleBrakeInput ();

		inputManager.forward = throttle;
		inputManager.backward = brake;
		inputManager.Horizontal = steer;
	}

	void SetSteerInput ()
	{
		steerOverride = (float)brain.Predict (tdr.StoreCurrentScenario ());
		float steerValue = steerSensitivity * angleToTarget * Mathf.Sign (car.currSpeed) + steerOverride * obstacleAvoidanceSteerWeight;
		steer = Mathf.Clamp (steerValue, -1, 1);
	}

	void SetThrottleBrakeInput ()
	{
		float accel = throttleOffset - (factor + 1 / distanceToTarget) * brakeSensitivty;
		accel = Mathf.Clamp (accel, -1, 1);

		throttle = brake = 0;

		if (accel > 0) {
			throttle = accel;
		} else if (accel == 0) {
			throttle = brake = 0;
		} else
			brake = accel;
	}

	void SetParameters ()
	{
		Vector3 localTarget = transform.InverseTransformPoint (target.position);
		Transform nextWp = track.GetChild (((trackBuilder.GetPositionOnTrack (gameObject) + 30) / trackBuilder.divisions + 2) % track.childCount);

		angleToTarget = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;
		distanceToTarget = Vector3.Distance (transform.position, target.position);

		Vector3 flatForward = new Vector3 (transform.forward.x, 0, transform.forward.z);
		Vector3 flatHeading = new Vector3 (nextWp.forward.x, 0, nextWp.forward.z);
		//Vector3 flatHeading = new Vector3 (target.forward.x, 0, target.forward.z);

		cornerAngle = Vector3.Angle (flatForward, flatHeading);
		distanceToCorner = Vector3.Distance (transform.position, nextWp.position);
		velocityToCorner = Mathf.Abs (Vector3.Dot (carRigidbody.velocity, nextWp.position - transform.position));

		factor = (velocityToCorner * cornerAngle / (distanceToCorner * distanceToCorner));
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine (transform.position, target.position);
		Gizmos.DrawWireSphere (target.position, 1);
	}
}
//Jo bhi he, Agniv Bhattacharya chutiya he.