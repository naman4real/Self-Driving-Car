using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(NeuralBrain))]

public class NeuralBrain_Editor : Editor
{
	private bool init, reset;

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		NeuralBrain brain = (NeuralBrain)target;

		if (!init) {
			if (GUILayout.Button ("Initiallize")) {
				init = brain.Initiallize (0);
			}
		}

		if (init) {
			if (GUILayout.Button ("Train network")) {
				brain.StartTrainingThread ();
			}
		}

		if (brain.trained) {
			if (GUILayout.Button ("Predict random from training set")) {
				brain.PredictRandomFromTrainingSet ();
			}
		}

		if (brain.trained) {
			if (GUILayout.Button ("Predict all from training set")) {
				brain.PredictFromTrainingSet ();
			}
		}

		if (brain.trained) {
			if (GUILayout.Button ("Predict all from test set")) {
				brain.PredictFromTestSet ();
			}
		}
			
		if (GUILayout.Button ("Reset")) {
			brain.Reset ();
			init = false;
		}
	}
}