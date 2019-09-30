using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TrainingDataRecorder : MonoBehaviour
{
	public Transform sensorLocal, dirLeft, dirLeftReset;
	public float maxSensorRange, sensorRotationAmount, dataCaptureFrequency;
	public bool writeCheck;
	public int escapeVectorSize;
	[HideInInspector]public int size, esv;

	private carController cc;
	private float theta, f;
	[HideInInspector]public double[] hValues;
	[HideInInspector]public double[] escapeVectors;
	private StreamWriter writer;
	private int writeCount, e = 0;
	[HideInInspector]public Vector3 obstacleSpawnLocation;

	void Awake ()
	{
		cc = GetComponent<carController> ();

		theta = Vector3.Angle (dirLeftReset.position - sensorLocal.position, sensorLocal.forward);

		size = (int)(2 * theta / sensorRotationAmount + 1);
		hValues = new double[size];
		esv = (int)(hValues.Length / escapeVectorSize) + 1;
		escapeVectors = new double[esv];
	}

	void Update ()
	{
		StoreCurrentScenario ();
		WriteDataToFile ();
	}

	float CalculateAvoidanceFactor (RaycastHit x)
	{
		float distance = Vector3.Distance (sensorLocal.position, x.point);
		Vector3 hitDirection = (x.point - sensorLocal.position).normalized;
		float angle = Vector3.SignedAngle (sensorLocal.forward.normalized, hitDirection, sensorLocal.up);
		return 1 / angle * distance;
	}

	public double[] StoreCurrentScenario ()
	{
		f = 0;
		int i = 0, eCount = 0;
		float rotation = 0;
		dirLeft.LookAt (dirLeftReset);
		float x = -theta;

		//for (int k = 0; k < escapeVectors.Length; k++)
		//escapeVectors [k] = 0;

		for (int k = 0; k < hValues.Length; k++)
			hValues [k] = 0;

		while (rotation < 2 * theta) {
			Vector3 dir = dirLeft.forward;
			RaycastHit hit;
			//i++;
			//Debug.DrawRay (dirLeft.position, dirLeft.forward);
			//e = i / escapeVectorSize;
			if (Physics.Raycast (dirLeft.position, dirLeft.forward, out hit, maxSensorRange) && hit.transform.tag == "CAS_obstacle") {
				Debug.DrawLine (dirLeft.position, hit.point, Color.red);
				//f = CalculateAvoidanceFactor (hit);
				//hValues [i++] = 1;// * Mathf.Sign (x);
				//hValues[i++] = f;
				hValues [i++] = x;
			} else {
				//escapeVectors [e]++;
				//hValues [i++] = 0;//maxSensorRange;
				hValues [i++] = 0;
				Debug.DrawLine (dirLeft.position, dirLeft.position + dirLeft.forward.normalized * 100, Color.green);
			}
			eCount = 0;
			dirLeft.Rotate (dirLeft.up, sensorRotationAmount);
			rotation = rotation + sensorRotationAmount;
			x += sensorRotationAmount;
		}

		//for (int v = 0; v < escapeVectors.Length; v++)
		//escapeVectors [v] /= escapeVectorSize;

		return hValues;
	}

	bool WriteCondition ()
	{
		return (Mathf.Abs (Mathf.Round (cc.steerAngle)) > 0) && (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D));
	}

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireSphere (obstacleSpawnLocation, 1);
		Gizmos.DrawLine (transform.position, obstacleSpawnLocation);
	}

	void WriteDataToFile ()
	{
		if (writer == null) {
			
			String path = "C:\\Users\\Naman Gupta\\Downloads\\Compressed\\minor project\\testProject0_5.4.2\\Assets\\CAS_trainingData\\trainingData";
			writer = new StreamWriter (path);
		}
		if (writer != null && writeCount <= 40000 && Time.frameCount % dataCaptureFrequency == 0 && cc.currSpeed > 20 && writeCheck) {
			if (this.isActiveAndEnabled && WriteCondition ()) {
				writer.Write (Mathf.Round (cc.steerAngle));

				for (int i = 0; i < hValues.Length; i++)
					writer.Write ("," + hValues [i]);

				writer.Write ("\n");
				writeCount++;

				Debug.Log ("Writing" + "," + writeCount);
			} else
				Debug.Log ("not writing");
		}

		if (writeCount > 40000)
			writer.Close ();
	}

	void OnQuitApplication ()
	{
		writer.Close ();
	}
}
//Jo bhi he, Agniv Bhattacharya chutiya he.